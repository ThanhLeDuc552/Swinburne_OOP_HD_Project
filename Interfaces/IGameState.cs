using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Swinburne_OOP_HD
{
    public interface IGameState
    {
        void Update();
        void Draw();
        void HandleInput();
    }
}
