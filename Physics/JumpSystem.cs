using SplashKitSDK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Swinburne_OOP_HD
{
    public class JumpSystem
    {
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
    }
}
