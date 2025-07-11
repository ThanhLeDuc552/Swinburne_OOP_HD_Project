using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SplashKitSDK;

namespace Swinburne_OOP_HD
{
    public class MudHazard : Hazards
    {
        public MudHazard(Point2D position) : base("MudHazard", "mud", "sprites/mudhazard.png", position) { }
    }
}
