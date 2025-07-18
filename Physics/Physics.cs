using System;
using SplashKitSDK;

namespace Swinburne_OOP_HD 
{
    public static class Physics 
    {
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
                // movewithcolliisoncheck x-axis
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
                // movewithcolliisoncheck y-axis
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