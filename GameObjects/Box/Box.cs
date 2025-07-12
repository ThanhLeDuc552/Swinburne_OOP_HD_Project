using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SplashKitSDK;

namespace Swinburne_OOP_HD
{
    public class Box : SolidObject
    {
        private Bitmap _box;
        private int _mass; // mass of the box

        public Box(DotTiled.Object obj)
        {
            Name = obj.Name;
            Position = SplashKit.VectorTo(obj.X, obj.Y - obj.Height);
            _box = SplashKit.LoadBitmap(Name, @"tiles\objects\other\crate.jpg");
        }

        public override void Draw()
        {
            _box.Draw(Position.X, Position.Y);
        }

        public override void Update()
        {
            // Implement later
        }

        public override Rectangle GetAABB()
        {
            return SplashKit.RectangleFrom(Position.X, Position.Y, _box.Width, _box.Height);
        }
    }
}
