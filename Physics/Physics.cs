using System;
using SplashKitSDK;

namespace Swinburne_OOP_HD 
{
    public static class Physics 
    {
        public static void ApplyGravity(SolidObject obj) 
        {
            // Falling
            if (!obj.IsGrounded) 
            {
                obj.Velocity = SplashKit.VectorTo(
                    obj.Velocity.X,
                    obj.Velocity.Y + GameConstants.GRAVITY_STRENGTH
                );

                if (obj.Velocity.Y > GameConstants.TERMINAL_VELOCITY) 
                {
                    obj.Velocity = SplashKit.VectorTo(
                        obj.Velocity.X,
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

        
        public static void FixStandingOnObject(Character character, SolidObject obj) 
        {
            if (SplashKit.RectanglesIntersect(character.GetAABB(), obj.GetAABB())) 
            {
                Rectangle playerAABB = character.GetAABB();
                Rectangle tileBounds = obj.GetAABB();
                
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


        public static void HandleObjectCollision(Character character, Platform platform)
        {
            // prevent phasing
            if (SplashKit.RectanglesIntersect(character.GetAABB(), platform.GetAABB()))
            {
                character.Position = SplashKit.VectorTo(character.Position.X - character.Velocity.X, character.Position.Y - character.Velocity.Y);

                // Horizontal movement
                Vector2D originalPosition = character.Position;
                Vector2D targetPosition = SplashKit.VectorTo(character.Position.X + character.Velocity.X, character.Position.Y);
                character.Position = targetPosition;

                if (SplashKit.RectanglesIntersect(character.GetAABB(), platform.GetAABB()))
                {
                    double safeDistanceX = FindMaxSafeDistance(character, platform, originalPosition, character.Velocity.X, 0);
                    character.Position = SplashKit.VectorTo(originalPosition.X + (character.Velocity.X * safeDistanceX), originalPosition.Y);

                    if (character.Velocity.X != 0) character.Velocity = SplashKit.VectorTo(0, character.Velocity.Y);
                }

                // Vertical movement
                originalPosition = character.Position;
                targetPosition = SplashKit.VectorTo(character.Position.X, character.Position.Y + character.Velocity.Y);
                character.Position = targetPosition;

                if (SplashKit.RectanglesIntersect(character.GetAABB(), platform.GetAABB()))
                {
                    double safeDistanceY = FindMaxSafeDistance(character, platform, originalPosition, 0, character.Velocity.Y);
                    character.Position = SplashKit.VectorTo(originalPosition.X, originalPosition.Y + (character.Velocity.Y * safeDistanceY));

                    if (character.Velocity.Y != 0)
                    {
                        double deltaY = character.Velocity.Y;
                        character.Velocity = SplashKit.VectorTo(character.Velocity.X, 0);

                        if (deltaY > 0) // Was falling down
                        {
                            character.IsGrounded = true;
                            character.CanJump = true;
                        }
                    }
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


        public static void MoveWithCollisionCheck<T>(T movingObject, Level level, double deltaX, double deltaY) where T : SolidObject
        {
            // If no movement, return early
            if (deltaX == 0 && deltaY == 0) return;

            Vector2D originalPosition = movingObject.Position;
            Vector2D targetPosition = SplashKit.VectorTo(
                originalPosition.X + deltaX,
                originalPosition.Y + deltaY
            );

            // Move to target position
            movingObject.Position = targetPosition;

            // Check if the new position causes any collisions
            if (HasAnyCollision(movingObject, level))
            {
                // If collision detected, try to find the maximum safe movement
                double safeDistance = FindMaxSafeDistance(movingObject, level, originalPosition, deltaX, deltaY);

                // Move to the safe position
                movingObject.Position = SplashKit.VectorTo(
                    originalPosition.X + (deltaX * safeDistance),
                    originalPosition.Y + (deltaY * safeDistance)
                );

                // Stop velocity in the direction of collision
                if (deltaX != 0)
                {
                    movingObject.Velocity = SplashKit.VectorTo(0, movingObject.Velocity.Y);
                }
                if (deltaY != 0)
                {
                    movingObject.Velocity = SplashKit.VectorTo(movingObject.Velocity.X, 0);
                    if (deltaY > 0) // Was falling down
                    {
                        movingObject.IsGrounded = true;
                        if (movingObject is Character character)
                        {
                            // Reset jump state for characters
                            character.CanJump = true;
                        }
                    }
                }
            }
        }


        private static bool HasAnyCollision<T>(T movingObject, Level level) where T : SolidObject
        {
            Rectangle movingObjectAABB = movingObject.GetAABB();

            int leftTile = (int)(SplashKit.RectangleLeft(movingObjectAABB) / GameConstants.TILE_SIZE);
            int rightTile = (int)(SplashKit.RectangleRight(movingObjectAABB) / GameConstants.TILE_SIZE);
            int topTile = (int)(SplashKit.RectangleTop(movingObjectAABB) / GameConstants.TILE_SIZE);
            int bottomTile = (int)(SplashKit.RectangleBottom(movingObjectAABB) / GameConstants.TILE_SIZE);

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

                                if (SplashKit.RectanglesIntersect(movingObject.GetAABB(), tileBounds))
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


        private static double FindMaxSafeDistance<T>(T movingObject, Level level, Vector2D startPos, double deltaX, double deltaY) where T : SolidObject
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
                movingObject.Position = SplashKit.VectorTo(
                    startPos.X + (stepX * currentDistance / step),
                    startPos.Y + (stepY * currentDistance / step)
                );

                if (HasAnyCollision(movingObject, level))
                {
                    break; // Found collision, use previous safe distance
                }

                safeDistance = currentDistance / totalDistance;
                currentDistance += step;
            }

            return safeDistance;
        }


        private static void MoveWithCollisionCheck<T>(T movingObject, SolidObject otherObject, double deltaX, double deltaY) where T : SolidObject
        {
            // If no movement, return early
            if (deltaX == 0 && deltaY == 0) return;

            Vector2D originalPosition = movingObject.Position;
            Vector2D targetPosition = SplashKit.VectorTo(
                originalPosition.X + deltaX,
                originalPosition.Y + deltaY
            );

            // Move to target position
            movingObject.Position = targetPosition;

            // Check if the new position causes any collisions
            if (SplashKit.RectanglesIntersect(movingObject.GetAABB(), otherObject.GetAABB()))
            {
                // If collision detected, try to find the maximum safe movement
                double safeDistance = FindMaxSafeDistance(movingObject, otherObject, originalPosition, deltaX, deltaY);
                
                // Move to the safe position
                movingObject.Position = SplashKit.VectorTo(
                    originalPosition.X + (deltaX * safeDistance),
                    originalPosition.Y + (deltaY * safeDistance)
                );

                // Stop velocity in the direction of collision
                if (deltaX != 0)
                {
                    movingObject.Velocity = SplashKit.VectorTo(0, movingObject.Velocity.Y);
                }
                if (deltaY != 0)
                {
                    movingObject.Velocity = SplashKit.VectorTo(movingObject.Velocity.X, 0);
                    if (deltaY > 0) // Was falling down
                    {
                        movingObject.IsGrounded = true;

                        if (movingObject is Character character)
                        {
                            // Reset jump state for characters
                            character.CanJump = true;
                        }
                    }
                }
            }
        }


        private static double FindMaxSafeDistance<T>(T movingObject, SolidObject otherObject, Vector2D startPos, double deltaX, double deltaY) where T : SolidObject
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
                movingObject.Position = SplashKit.VectorTo(
                    startPos.X + (stepX * currentDistance / step),
                    startPos.Y + (stepY * currentDistance / step)
                );

                if (SplashKit.RectanglesIntersect(movingObject.GetAABB(), otherObject.GetAABB()))
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