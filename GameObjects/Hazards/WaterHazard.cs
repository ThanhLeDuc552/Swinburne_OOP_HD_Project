using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SplashKitSDK;

namespace Swinburne_OOP_HD
{
    public class WaterHazard : Hazards 
    {
        public WaterHazard(Point2D position) : base("WaterHazard", "water", "sprites/waterhazard.png", position) { }
    }
}
