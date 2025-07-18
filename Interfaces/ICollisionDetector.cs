using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Swinburne_OOP_HD
{
    public interface ICollisionDetector
    {
        bool HasCollision(SolidObject obj1, SolidObject obj2);
        bool HasCollision(SolidObject movingObject, Level level);
        CollisionInfo GetCollisionInfo(SolidObject obj1, SolidObject obj2);
    }
}
