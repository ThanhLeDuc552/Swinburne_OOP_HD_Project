using System;
using SplashKitSDK;
using DotTiled;
using DotTiled.Serialization;
using DotTiled.Serialization.Tmx;

namespace Swinburne_OOP_HD
{
    public interface IGameState
    {
        void Update();
        void Draw();
        void HandleInput();
    }

    public class GameStateManager
    {
        private Window _window;
        private LevelManager _levelManager;
        private IGameState _currentState;
        private Menu _menu;
        private int _levelCount = 3; // Change as needed
        public Window Window => _window;
        public LevelManager LevelManager => _levelManager;
        public Menu Menu => _menu;

        public GameStateManager(Window window)
        {
            _window = window;
            _levelManager = new LevelManager();
            _menu = new Menu(_levelManager);
            ChangeState(new MenuState(this));
        }

        public void ChangeState(IGameState newState)
        {
            _currentState = newState;
        }

        public void Run()
        {
            while (!_window.CloseRequested)
            {
                SplashKit.ClearScreen();
                SplashKit.ProcessEvents();
                _currentState.HandleInput();
                _currentState.Update();
                _currentState.Draw();
                SplashKit.RefreshScreen();
            }
            SplashKit.FreeAllSprites();
        }
    }

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
            if (_selectedLevel != -1)
            {
                _manager.Menu.Reset();
                _manager.ChangeState(new PlayingState(_manager, _selectedLevel));
            }
        }
    }

    public class PlayingState : IGameState
    {
        private GameStateManager _manager;
        private Level _level;
        private string _levelName;
        private int _finalScore;
        private bool _paused = false;

        public PlayingState(GameStateManager manager, int levelIndex)
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

    public class GameOverState : IGameState
    {
        private GameStateManager _manager;
        private Level _level;
        private string _levelName;
        private int _finalScore;
        private int _timer = 120; // ~2 seconds at 60fps

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