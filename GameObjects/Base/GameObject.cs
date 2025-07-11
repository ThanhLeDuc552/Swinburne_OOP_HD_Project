using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SplashKitSDK;

namespace Swinburne_OOP_HD
{
    public abstract class GameObject
    {
        private string _name;
        private string _type;
        
        public string Name 
        { 
            get { return _name; } 
            set { _name = value; } 
        }

        public string Type 
        { 
            get { return _type; } 
            set { _type = value; }
        }

        public abstract void Draw();
        public abstract void Update();
        public abstract Rectangle GetAABB();
        public abstract void ClearResource();
    }
}
