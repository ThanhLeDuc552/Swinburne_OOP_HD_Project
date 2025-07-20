using System;
using SplashKitSDK;
using DotTiled;
using DotTiled.Serialization;
using DotTiled.Serialization.Tmx;

namespace Swinburne_OOP_HD
{
    public class GameStateManager
    {
        private Window _window;
        private LevelManager _levelManager;
        private IGameState _currentState;
        private Menu _menu;
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
}