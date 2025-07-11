using System;
using SplashKitSDK;

namespace Swinburne_OOP_HD 
{
    public static class Physics 
    {
        public static void ApplyGravity(Character character) 
        {
            // Falling
            if (!character.IsGrounded) 
            {
                character.Velocity = SplashKit.VectorTo(
                    character.Velocity.X,
                    character.Velocity.Y + GameConstants.GRAVITY_STRENGTH
                );

                if (character.Velocity.Y > GameConstants.TERMINAL_VELOCITY) 
                {
                    character.Velocity = SplashKit.VectorTo(
                        character.Velocity.X,
                        GameConstants.TERMINAL_VELOCITY
                    );
                }
            }
        }

        public static void Jump(Character character, bool jumpPressed) 
        {
            if (character.IsGrounded && character.CanJump && jumpPressed) 
            {
                character.Velocity = SplashKit.VectorTo(
                    character.Velocity.X,
                    GameConstants.JUMP_FORCE
                );
                character.CanJump = false;
                character.IsGrounded = false;
            }
        }

        public static bool CheckCollision(Character character, Rectangle other) 
        {
            return SplashKit.RectanglesIntersect(character.GetAABB(), other);
        }

        public static void HandleTileCollision(Character character, Rectangle tileBounds) 
        {
            if (CheckCollision(character, tileBounds)) 
            {
                Rectangle playerAABB = character.GetAABB();
                
                double overlapLeft = SplashKit.RectangleRight(playerAABB) - SplashKit.RectangleLeft(tileBounds);   // Player's right edge past platform's left edge
                double overlapRight = SplashKit.RectangleRight(tileBounds) - SplashKit.RectangleLeft(playerAABB);   // Platform's right edge past player's left edge
                double overlapTop = SplashKit.RectangleBottom(playerAABB) - SplashKit.RectangleTop(tileBounds);    // Player's bottom edge past platform's top edge
                double overlapBottom = SplashKit.RectangleBottom(tileBounds) - SplashKit.RectangleTop(playerAABB); // Platform's bottom edge past player's top edge


                // Find the smallest overlap to determine the collision direction
                // The smallest overlap indicates which side the collision occurred on
                double minOverlap = Math.Min(Math.Min(overlapLeft, overlapRight), Math.Min(overlapTop, overlapBottom));

                if (minOverlap == overlapTop && character.Velocity.Y > 0) 
                {
                    character.Position = SplashKit.VectorTo(
                        character.Position.X,
                        SplashKit.RectangleTop(tileBounds) - character.HalfHeight
                    );

                    character.Velocity = SplashKit.VectorTo(
                        character.Velocity.X,
                        0
                    );

                    character.IsGrounded = true;
                    character.CanJump = true;
                }

                else if (minOverlap == overlapBottom && character.Velocity.Y < 0) 
                {
                    character.Position = SplashKit.VectorTo(
                        character.Position.X,
                        SplashKit.RectangleBottom(tileBounds) + character.HalfHeight - character.HairHeight
                    );

                    character.Velocity = SplashKit.VectorTo(
                        character.Velocity.X,
                        0
                    );
                }

                else if (minOverlap == overlapLeft && character.Velocity.X < 0) 
                {
                    character.Position = SplashKit.VectorTo(
                        SplashKit.RectangleLeft(tileBounds) - character.HalfWidth,
                        character.Position.Y
                    );

                    character.Velocity = SplashKit.VectorTo(
                        0,
                        character.Velocity.Y
                    );
                }

                else if (minOverlap == overlapRight && character.Velocity.X > 0) 
                {
                    character.Position = SplashKit.VectorTo(
                        SplashKit.RectangleRight(tileBounds) + character.HalfWidth,
                        character.Position.Y
                    );

                    character.Velocity = SplashKit.VectorTo(
                        0,
                        character.Velocity.Y
                    );
                }
            }
        }

        public static void UpdateCharacter(Character character, Level level) 
        {
            if (character.IsMovingLeft()) 
            {
                character.Velocity = SplashKit.VectorTo(
                    -GameConstants.MOVE_SPEED,
                    character.Velocity.Y
                );
            }
            else if (character.IsMovingRight()) 
            {
                character.Velocity = SplashKit.VectorTo(
                    GameConstants.MOVE_SPEED,
                    character.Velocity.Y
                );
            }
            else 
            {
                character.Velocity = SplashKit.VectorTo(
                    character.Velocity.X * GameConstants.FRICTION,
                    character.Velocity.Y
                );

                if (Math.Abs(character.Velocity.X) < 0.1f) 
                {
                    character.Velocity = SplashKit.VectorTo(
                        0,
                        character.Velocity.Y
                    );
                }
            }

            // Handle jumping BEFORE resetting grounded status
            bool jumpPressed = character.IsJumping();
            Jump(character, jumpPressed);

            // Store original grounded state
            bool wasGrounded = character.IsGrounded;

            // Reset grounded status - will be set to true if collision detected below
            character.IsGrounded = false;

            ApplyGravity(character);

            // Move horizontally with collision checking
            MoveWithCollisionCheck(character, level, character.Velocity.X, 0);

            // Then move vertically with collision checking
            MoveWithCollisionCheck(character, level, 0, character.Velocity.Y);
        }

        private static void MoveWithCollisionCheck(Character character, Level level, double deltaX, double deltaY)
        {
            // If no movement, return early
            if (deltaX == 0 && deltaY == 0) return;

            Vector2D originalPosition = character.Position;
            Vector2D targetPosition = SplashKit.VectorTo(
                originalPosition.X + deltaX,
                originalPosition.Y + deltaY
            );

            // Move to target position
            character.Position = targetPosition;

            // Check if the new position causes any collisions
            if (HasAnyCollision(character, level))
            {
                // If collision detected, try to find the maximum safe movement
                double safeDistance = FindMaxSafeDistance(character, level, originalPosition, deltaX, deltaY);
                
                // Move to the safe position
                character.Position = SplashKit.VectorTo(
                    originalPosition.X + (deltaX * safeDistance),
                    originalPosition.Y + (deltaY * safeDistance)
                );

                // Stop velocity in the direction of collision
                if (deltaX != 0)
                {
                    character.Velocity = SplashKit.VectorTo(0, character.Velocity.Y);
                }
                if (deltaY != 0)
                {
                    character.Velocity = SplashKit.VectorTo(character.Velocity.X, 0);
                    if (deltaY > 0) // Was falling down
                    {
                        character.IsGrounded = true;
                        character.CanJump = true;
                    }
                }
            }
        }

        private static bool HasAnyCollision(Character character, Level level)
        {
            Rectangle playerAABB = character.GetAABB();

            int leftTile = (int)(SplashKit.RectangleLeft(playerAABB) / GameConstants.TILE_SIZE);
            int rightTile = (int)(SplashKit.RectangleRight(playerAABB) / GameConstants.TILE_SIZE);
            int topTile = (int)(SplashKit.RectangleTop(playerAABB) / GameConstants.TILE_SIZE);
            int bottomTile = (int)(SplashKit.RectangleBottom(playerAABB) / GameConstants.TILE_SIZE);

            for (int y = topTile; y <= bottomTile; y++) 
            {
                for (int x = leftTile; x <= rightTile; x++) 
                {
                    if (x >= 0 && x < level.MapWidth && y >= 0 && y < level.MapHeight)
                    {
                        int tileIndex = y * (int)level.MapWidth + x;
                        
                        if (tileIndex >= 0 && tileIndex < level.MapData.Length)
                        {
                            uint tileGID = level.MapData[tileIndex];
                            if (tileGID >= 1 && tileGID <= 7) 
                            {
                                Rectangle tileBounds = SplashKit.RectangleFrom(
                                    x * GameConstants.TILE_SIZE,
                                    y * GameConstants.TILE_SIZE,
                                    GameConstants.TILE_SIZE,
                                    GameConstants.TILE_SIZE
                                );
                                
                                if (CheckCollision(character, tileBounds))
                                {
                                    return true;
                                }
                            }
                        }
                    }
                }
            }
            return false;
        }

        private static double FindMaxSafeDistance(Character character, Level level, Vector2D startPos, double deltaX, double deltaY)
        {
            double safeDistance = 0.0;
            double step = 0.1; // Small step size for precision
            double totalDistance = Math.Sqrt(deltaX * deltaX + deltaY * deltaY);
            
            if (totalDistance == 0) return 0;

            double stepX = (deltaX / totalDistance) * step;
            double stepY = (deltaY / totalDistance) * step;
            double currentDistance = 0;

            while (currentDistance < totalDistance)
            {
                // Test position at current distance
                character.Position = SplashKit.VectorTo(
                    startPos.X + (stepX * currentDistance / step),
                    startPos.Y + (stepY * currentDistance / step)
                );

                if (HasAnyCollision(character, level))
                {
                    break; // Found collision, use previous safe distance
                }

                safeDistance = currentDistance / totalDistance;
                currentDistance += step;
            }

            return safeDistance;
        }
    }
}