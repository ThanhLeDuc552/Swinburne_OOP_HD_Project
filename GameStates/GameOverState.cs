using SplashKitSDK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Swinburne_OOP_HD
{
    public class GameOverState : IGameState
    {
        private GameStateManager _manager;
        private Level _level;
        private string _levelName;
        private int _finalScore;
        private int _timer = 180;

        public GameOverState(GameStateManager manager, Level level, string levelName, int finalScore)
        {
            _manager = manager;
            _level = level;
            _levelName = levelName;
            _finalScore = finalScore;
        }

        public void Update()
        {
            _timer--;
            if (_timer <= 0)
            {
                _manager.LevelManager.ClearLevelResource(_level);
                _manager.ChangeState(new MenuState(_manager));
            }
        }

        public void Draw()
        {
            SplashKit.ClearScreen(SplashKitSDK.Color.Black);
            if (_level.IsGameOver)
            {
                SplashKit.DrawText("GAME OVER!", SplashKitSDK.Color.Red, "Arial", 32, 250, 200);
                SplashKit.DrawText($"Score: {_finalScore}", SplashKitSDK.Color.White, "Arial", 24, 270, 240);
                SplashKit.DrawText("Returning to menu...", SplashKitSDK.Color.White, "Arial", 16, 230, 280);
            }
            else if (_level.IsLevelCompleted)
            {
                SplashKit.DrawText("LEVEL COMPLETED!", SplashKitSDK.Color.Green, "Arial", 32, 180, 200);
                SplashKit.DrawText($"Score: {_finalScore}", SplashKitSDK.Color.White, "Arial", 24, 270, 240);
                SplashKit.DrawText("Returning to menu...", SplashKitSDK.Color.White, "Arial", 16, 200, 280);
            }
        }

        public void HandleInput() { }
    }
}
