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
            if (!SplashKit.RectanglesIntersect(character.GetAABB(), obj.GetAABB()))
            {
                return; // No collision
            }

            // Special handling for Platform objects to fix "sticky" behavior (also support object extensions in the future)
            if (obj is Platform platform)
            {
                ResolvePlatformCollision(character, platform);
            }
            else
            {
                // Use the original collision info method for other solid objects
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
        }
        
        // Resolves platform collisions with improved logic to prevent "sticky" behavior.
        // This method allows horizontal movement when standing on platforms.
        private void ResolvePlatformCollision(Character character, Platform platform)
        {
            Rectangle characterAABB = character.GetAABB();
            Rectangle platformAABB = platform.GetAABB();

            // Calculate overlaps on each side
            double overlapLeft = SplashKit.RectangleRight(characterAABB) - SplashKit.RectangleLeft(platformAABB);
            double overlapRight = SplashKit.RectangleRight(platformAABB) - SplashKit.RectangleLeft(characterAABB);
            double overlapTop = SplashKit.RectangleBottom(characterAABB) - SplashKit.RectangleTop(platformAABB);
            double overlapBottom = SplashKit.RectangleBottom(platformAABB) - SplashKit.RectangleTop(characterAABB);

            // Find the smallest overlap to determine the primary collision direction
            double minOverlap = Math.Min(Math.Min(overlapLeft, overlapRight), Math.Min(overlapTop, overlapBottom));

            // Resolve collision based on the primary collision side
            if (minOverlap == overlapTop && character.Velocity.Y > 0)
            {
                // Character landing on top of platform
                ResolvePlatformTopCollision(character, platform, overlapTop);
            }
            else if (minOverlap == overlapBottom && character.Velocity.Y < 0)
            {
                // Character hitting platform from below (ceiling collision)
                ResolvePlatformBottomCollision(character, platform, overlapBottom);
            }
            else if (minOverlap == overlapLeft && character.Velocity.X < 0)
            {
                // Character hitting platform from the right (moving left)
                ResolvePlatformSideCollision(character, platform, overlapLeft, true);
            }
            else if (minOverlap == overlapRight && character.Velocity.X > 0)
            {
                // Character hitting platform from the left (moving right)
                ResolvePlatformSideCollision(character, platform, overlapRight, false);
            }
            else
            {
                // Handle edge case: character is stuck inside platform
                ResolveStuckInPlatform(character, platform);
            }
        }

        // Resolves collision when character lands on top of a platform
        private void ResolvePlatformTopCollision(Character character, Platform platform, double overlapTop)
        {
            // Position character on top of platform
            character.Position = SplashKit.VectorTo(
                character.Position.X,
                SplashKit.RectangleTop(platform.GetAABB()) - character.HalfHeight
            );

            // Stop vertical movement but preserve horizontal movement
            character.Velocity = SplashKit.VectorTo(
                character.Velocity.X, // Keep horizontal velocity!
                0
            );

            // Set grounded state
            character.IsGrounded = true;
            character.CanJump = true;
        }

        /// Resolves collision when character hits platform from below
        private void ResolvePlatformBottomCollision(Character character, Platform platform, double overlapBottom)
        {
            // Position character below platform
            character.Position = SplashKit.VectorTo(
                character.Position.X,
                SplashKit.RectangleBottom(platform.GetAABB()) + character.HalfHeight - character.HairHeight
            );

            // Stop vertical movement but preserve horizontal movement
            character.Velocity = SplashKit.VectorTo(
                character.Velocity.X, // Keep horizontal velocity!
                0
            );
        }

        // Resolves collision when character hits platform from the side
        private void ResolvePlatformSideCollision(Character character, Platform platform, double overlap, bool isLeftSide)
        {
            if (isLeftSide)
            {
                // Character hitting from the right side (moving left)
                character.Position = SplashKit.VectorTo(
                    SplashKit.RectangleLeft(platform.GetAABB()) - character.HalfWidth,
                    character.Position.Y
                );
            }
            else
            {
                // Character hitting from the left side (moving right)
                character.Position = SplashKit.VectorTo(
                    SplashKit.RectangleRight(platform.GetAABB()) + character.HalfWidth,
                    character.Position.Y
                );
            }

            // Stop horizontal movement but preserve vertical movement
            character.Velocity = SplashKit.VectorTo(
                0, // Stop horizontal movement when hitting sides
                character.Velocity.Y
            );
        }

        // Handles the edge case where character gets stuck inside a platform
        private void ResolveStuckInPlatform(Character character, Platform platform)
        {
            // Move character to the nearest safe position
            Rectangle characterAABB = character.GetAABB();
            Rectangle platformAABB = platform.GetAABB();
            
            double centerX = character.Position.X;
            double centerY = character.Position.Y;
            double platformCenterX = SplashKit.RectangleLeft(platformAABB) + (SplashKit.RectangleRight(platformAABB) - SplashKit.RectangleLeft(platformAABB)) / 2;
            double platformCenterY = SplashKit.RectangleTop(platformAABB) + (SplashKit.RectangleBottom(platformAABB) - SplashKit.RectangleTop(platformAABB)) / 2;

            // Determine which direction to push the character out
            double deltaX = centerX - platformCenterX;
            double deltaY = centerY - platformCenterY;

            if (Math.Abs(deltaX) > Math.Abs(deltaY))
            {
                // Push out horizontally
                if (deltaX > 0)
                {
                    // Push right
                    character.Position = SplashKit.VectorTo(
                        SplashKit.RectangleRight(platformAABB) + character.HalfWidth,
                        character.Position.Y
                    );
                }
                else
                {
                    // Push left
                    character.Position = SplashKit.VectorTo(
                        SplashKit.RectangleLeft(platformAABB) - character.HalfWidth,
                        character.Position.Y
                    );
                }
                character.Velocity = SplashKit.VectorTo(0, character.Velocity.Y);
            }
            else
            {
                // Push out vertically
                if (deltaY > 0)
                {
                    // Push up
                    character.Position = SplashKit.VectorTo(
                        character.Position.X,
                        SplashKit.RectangleTop(platformAABB) - character.HalfHeight
                    );
                    character.IsGrounded = true;
                    character.CanJump = true;
                }
                else
                {
                    // Push down
                    character.Position = SplashKit.VectorTo(
                        character.Position.X,
                        SplashKit.RectangleBottom(platformAABB) + character.HalfHeight
                    );
                }
                character.Velocity = SplashKit.VectorTo(character.Velocity.X, 0);
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
