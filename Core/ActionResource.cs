using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SplashKitSDK;

namespace Swinburne_OOP_HD
{
    public struct ActionResource
    {
        public string Name { get; set; }
        public Bitmap Bitmap { get; set; }
        public AnimationScript Script { get; set; }
        public Animation Animation { get; set; }
        public DrawingOptions Options { get; set; }
        public Sprite Sprite { get; set; }

        public ActionResource(string name)
        {
            Name = name;
            Bitmap = null;
            Script = null;
            Animation = null;
            Options = default;
            Sprite = null;
        }
    }
}
