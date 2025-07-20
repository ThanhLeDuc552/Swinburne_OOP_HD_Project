using SplashKitSDK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Swinburne_OOP_HD
{
    public static class CharacterExtensions
    {
        public static void ApplyJumpForce(this Character character, double jumpForce)
        {
            character.Velocity = SplashKit.VectorTo(character.Velocity.X, jumpForce);
        }

        public static void SetGroundedState(this Character character, SolidObject ground, double penetrationDepth)
        {
            // Adjust position to be on top of the ground
            Rectangle groundBounds = ground.GetAABB();
            character.Position = SplashKit.VectorTo(
                character.Position.X,
                SplashKit.RectangleTop(groundBounds) - character.HalfHeight
            );

            character.Velocity = SplashKit.VectorTo(character.Velocity.X, 0);
            character.IsGrounded = true;
            character.CanJump = true;
        }

        public static void HandleCeilingCollision(this Character character, SolidObject ceiling, double penetrationDepth)
        {
            Rectangle ceilingBounds = ceiling.GetAABB();
            character.Position = SplashKit.VectorTo(
                character.Position.X,
                SplashKit.RectangleBottom(ceilingBounds) + character.HalfHeight - character.HairHeight
            );

            character.Velocity = SplashKit.VectorTo(character.Velocity.X, 0);
        }

        public static void HandleWallCollision(this Character character, SolidObject wall, double penetrationDepth, bool isLeftWall)
        {
            Rectangle wallBounds = wall.GetAABB();
            double newX;
            if (isLeftWall)
            {
                newX = SplashKit.RectangleLeft(wallBounds) - character.HalfWidth;
            }
            else
            {
                newX = SplashKit.RectangleRight(wallBounds) + character.HalfWidth;
            }

            character.Position = SplashKit.VectorTo(newX, character.Position.Y);
            character.Velocity = SplashKit.VectorTo(0, character.Velocity.Y);
        }
    }
}
