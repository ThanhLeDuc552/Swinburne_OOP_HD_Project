using SplashKitSDK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Swinburne_OOP_HD
{
    public class CollisionInfo
    {
        public bool HasCollision { get; set; }
        public Vector2D CollisionNormal { get; set; } // normal vector pointing away from the collision
        public double PenetrationDepth { get; set; } // amount of overlap
        public CollisionSide Side { get; set; } // overlapping side
    }
}
