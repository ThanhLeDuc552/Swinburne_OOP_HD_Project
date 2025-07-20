using SplashKitSDK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Swinburne_OOP_HD
{
    public class PlayingState : IGameState
    {
        private GameStateManager _manager;
        private Level _level;
        private string _levelName;
        private int _finalScore;
        private bool _paused = false;

        public PlayingState(GameStateManager manager, int levelIndex) // this one creates the level
        {
            _manager = manager;
            _levelName = $"Level{levelIndex + 1}";
            string levelFile = $"{_levelName}.tmx";
            _level = new Level(levelFile);
            _manager.LevelManager.AddLevel(_level, _levelName);
        }

        public void Update()
        {
            if (!_paused && !_level.IsGameOver && !_level.IsLevelCompleted)
            {
                _level.Update();
            }
        }

        public void Draw()
        {
            if (_paused)
            {
                SplashKit.ClearScreen(SplashKitSDK.Color.Black);
                SplashKit.DrawText("PAUSED", SplashKitSDK.Color.Yellow, "Arial", 32, 220, 150);
                SplashKit.DrawText("Press R to Resume", SplashKitSDK.Color.White, "Arial", 20, 220, 200);
                SplashKit.DrawText("Press Q to Exit Level", SplashKitSDK.Color.White, "Arial", 20, 220, 240);
            }
            else
            {
                _level.Draw();
                SplashKit.DrawInterface();
            }
        }

        public void HandleInput()
        {
            if (_paused)
            {
                if (SplashKit.KeyTyped(KeyCode.RKey))
                    _paused = false;
                else if (SplashKit.KeyTyped(KeyCode.QKey))
                {
                    _manager.LevelManager.ClearLevelResource(_level);
                    _manager.ChangeState(new MenuState(_manager));
                }
                return;
            }

            if (SplashKit.KeyTyped(KeyCode.EscapeKey))
            {
                _paused = true;
                return;
            }

            if (_level.IsGameOver || _level.IsLevelCompleted)
            {
                _finalScore = _level.CalculateScore();
                _manager.LevelManager.SetHighestScore(_levelName, _finalScore);
                _manager.ChangeState(new GameOverState(_manager, _level, _levelName, _finalScore));
            }
        }
    }
}
