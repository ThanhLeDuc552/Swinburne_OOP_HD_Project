using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SplashKitSDK;
using DotTiled;

namespace Swinburne_OOP_HD
{
    public class FireExitDoor : ExitDoor
    {
        public FireExitDoor(DotTiled.Object obj) : base(obj, "sprites/fireexitdoor.png")
        {
            // Fire-specific initialization if needed
        }
    }
} 