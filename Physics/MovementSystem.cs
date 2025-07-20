using SplashKitSDK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Swinburne_OOP_HD
{
    public class MovementSystem
    {
        private readonly ICollisionDetector _collisionDetector;

        public MovementSystem(ICollisionDetector collisionDetector)
        {
            _collisionDetector = collisionDetector;
        }

        // Moves on the maze with collision checking (the main physics)
        public void MoveWithCollisionCheck<T>(T obj, Level level, double deltaX, double deltaY) where T : SolidObject
        {
            if (deltaX == 0 && deltaY == 0) return;

            Vector2D originalPosition = obj.Position;
            Vector2D targetPosition = SplashKit.VectorTo(
                originalPosition.X + deltaX,
                originalPosition.Y + deltaY
            );

            obj.Position = targetPosition;

            if (_collisionDetector.HasCollision(obj, level))
            {
                double safeDistance = FindMaxSafeDistance(obj, level, originalPosition, deltaX, deltaY);

                obj.Position = SplashKit.VectorTo(
                    originalPosition.X + (deltaX * safeDistance),
                    originalPosition.Y + (deltaY * safeDistance)
                );

                HandleCollisionResponse(obj, deltaX, deltaY);
            }
        }

        public void HandleJump(Character character, bool jumpPressed)
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

        private double FindMaxSafeDistance<T>(T movingObject, Level level, Vector2D startPos, double deltaX, double deltaY) where T : SolidObject
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

                if (_collisionDetector.HasCollision(movingObject, level))
                {
                    break; // Found collision, use previous safe distance
                }

                safeDistance = currentDistance / totalDistance;
                currentDistance += step;
            }

            return safeDistance;
        }

        private void HandleCollisionResponse<T>(T obj, double deltaX, double deltaY) where T : SolidObject
        {
            // Stop velocity in the direction of collision
            if (deltaX != 0)
            {
                obj.Velocity = SplashKit.VectorTo(0, obj.Velocity.Y);
            }
            if (deltaY != 0)
            {
                obj.Velocity = SplashKit.VectorTo(obj.Velocity.X, 0);
                if (deltaY > 0) // Was falling down
                {
                    obj.IsGrounded = true;
                    if (obj is Character character)
                    {
                        // Reset jump state for characters
                        character.CanJump = true;
                    }
                }
            }
        }
    }
}
