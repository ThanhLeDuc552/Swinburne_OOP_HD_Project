using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SplashKitSDK;
using DotTiled;

namespace Swinburne_OOP_HD
{
    public class WaterExitDoor : ExitDoor
    {
        public WaterExitDoor(DotTiled.Object obj) : base(obj, "sprites/waterexitdoor.png")
        {
            // Water-specific initialization if needed
        }
    }
} 