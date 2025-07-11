using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SplashKitSDK;

namespace Swinburne_OOP_HD
{
    public class FireBoy : Character
    {
        public FireBoy() : base("fireboy_movement.txt", "fire", "FireBoy") { }

        public override bool IsMovingLeft() => SplashKit.KeyDown(KeyCode.AKey);
        public override bool IsMovingRight() => SplashKit.KeyDown(KeyCode.DKey);
        public override bool IsJumping() => SplashKit.KeyTyped(KeyCode.WKey);
        public override bool IsFalling() => false; // Falling is automatic - no manual input
        public override bool IsIdle() => !IsMovingLeft() && !IsMovingRight() && !IsJumping();
    }
}
