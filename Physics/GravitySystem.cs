using SplashKitSDK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Swinburne_OOP_HD
{
    public class GravitySystem
    {
        public void ApplyGravity(SolidObject obj)
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
    }
}
