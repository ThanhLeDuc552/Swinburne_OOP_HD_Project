using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SplashKitSDK;

namespace Swinburne_OOP_HD
{
    public class CollisionResolver
    {
        private readonly ICollisionDetector _collisionDetector;

        public CollisionResolver(ICollisionDetector collisionDetector)
        {
            _collisionDetector = collisionDetector;
        }

        public void ResolveCollision(Character character, SolidObject obj)
        {
            CollisionInfo collisionInfo = _collisionDetector.GetCollisionInfo(character, obj);
            if (!collisionInfo.HasCollision)
            {
                return;
            }

            // Resolve the collision based on the side of the collision
            switch (collisionInfo.Side)
            {
                case CollisionSide.Top:
                    {
                        if (character.Velocity.Y > 0)
                        {
                            CharacterExtensions.SetGroundedState(character, obj, collisionInfo.PenetrationDepth);
                        }
                        break;
                    }

                case CollisionSide.Bottom:
                    {
                        if (character.Velocity.Y < 0)
                        {
                            CharacterExtensions.HandleCeilingCollision(character, obj, collisionInfo.PenetrationDepth);
                        }
                        break;
                    }

                case CollisionSide.Left:
                    {
                        if (character.Velocity.X < 0)
                        {
                            CharacterExtensions.HandleWallCollision(character, obj, collisionInfo.PenetrationDepth, true);
                        }
                        break;
                    }

                case CollisionSide.Right:
                    {
                        if (character.Velocity.X > 0)
                        {
                            CharacterExtensions.HandleWallCollision(character, obj, collisionInfo.PenetrationDepth, false);
                        }
                        break;
                    }

                default:
                    {
                        break;
                    }
            }
        }

        public void ResolveCollisionWithLevel(SolidObject obj, Level level)
        {
            if (!_collisionDetector.HasCollision(obj, level)) return;

            // Use safe movement to resolve collision
            MovementSystem movementSystem = new MovementSystem(_collisionDetector);
            movementSystem.MoveWithCollisionCheck(obj, level, 0, 0); // This will snap to safe position
        }
    }
}
