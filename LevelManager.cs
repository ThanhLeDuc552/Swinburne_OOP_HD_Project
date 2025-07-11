using SplashKitSDK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Swinburne_OOP_HD
{
    public class LevelManager
    {
        private List<Level> _levels;
        private Bitmap _levelState;

        public LevelManager()
        {
            _levels = new List<Level>();
            
        }

        public List<Level> Levels
        {
            get { return _levels; }
        }
        
        
    }
}
