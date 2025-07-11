using System;
using SplashKitSDK;

namespace Swinburne_OOP_HD 
{
    public static class Physicsbla 
    {
        public static void MoveWithCollisionCheck<T>(T movingObject, SolidObject otherObject, double deltaX, double deltaY) where T : SolidObject
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

        
        public static void HandleTileCollision(Character character, Platform platform) 
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
    }
}
