using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SplashKitSDK;

namespace Swinburne_OOP_HD
{
    public class LavaHazard : Hazards
    {
        public LavaHazard(Point2D position) : base("LavaHazard", "fire", "sprites/lavahazard.png", position)
        {
            // Fire-specific initialization if needed
        }
    }
}
