using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using SplashKitSDK;

namespace Swinburne_OOP_HD
{
    public class WaterGirl : Character
    {
        public WaterGirl() : base("watergirl_movement.txt", "water", "WaterGirl") { }

        public override bool IsMovingLeft() => SplashKit.KeyDown(KeyCode.LeftKey);
        public override bool IsMovingRight() => SplashKit.KeyDown(KeyCode.RightKey);
        public override bool IsJumping() => SplashKit.KeyTyped(KeyCode.UpKey);
        public override bool IsFalling() => false; // Falling is automatic - no manual input
        public override bool IsIdle() => !IsMovingLeft() && !IsMovingRight() && !IsJumping();
    }
}
