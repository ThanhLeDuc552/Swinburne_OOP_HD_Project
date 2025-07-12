using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Swinburne_OOP_HD
{
    public class MenuState : IGameState
    {
        private GameStateManager _manager;
        private int _selectedLevel = -1;
        public MenuState(GameStateManager manager) { _manager = manager; }
        public void Update() { }
        public void Draw() { _manager.Menu.Draw(_manager.Window); }
        public void HandleInput()
        {
            if (_selectedLevel == -1)
                _selectedLevel = _manager.Menu.HandleInput(_manager.Window);

            else
            {
                _manager.Menu.Reset();
                _manager.ChangeState(new PlayingState(_manager, _selectedLevel));
            }
        }
    }
}
