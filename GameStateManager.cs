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
        private Level _level1;
        private bool _levelEnded;
        private int _finalScore;

        public GameStateManager(Window window)
        {
            _window = window;
            _level1 = new Level("Level1.tmx");
            _levelEnded = false;
            _finalScore = 0;
        }

        public void Run()
        {
            while (!_window.CloseRequested && !_level1.IsGameOver && !_level1.IsLevelCompleted)
            {
                SplashKit.ClearScreen();
                SplashKit.ProcessEvents();

                _level1.Update();
                _level1.Draw();

                SplashKit.DrawInterface();
                SplashKit.RefreshScreen();
            }

            // Calculate and store final score
            _finalScore = _level1.CalculateScore();
            _levelEnded = true;

            // Handle end game states and display score
            SplashKit.ClearScreen(SplashKitSDK.Color.Black);
            if (_level1.IsGameOver)
            {
                SplashKit.DrawText("GAME OVER!", SplashKitSDK.Color.Red, "Arial", 32, 250, 200);
                SplashKit.DrawText($"Score: {_finalScore}", SplashKitSDK.Color.White, "Arial", 24, 270, 240);
                SplashKit.DrawText("Press any key to exit", SplashKitSDK.Color.White, "Arial", 16, 230, 280);
            }
            else if (_level1.IsLevelCompleted)
            {
                SplashKit.DrawText("LEVEL COMPLETED!", SplashKitSDK.Color.Green, "Arial", 32, 180, 200);
                SplashKit.DrawText($"Score: {_finalScore}", SplashKitSDK.Color.White, "Arial", 24, 270, 240);
                SplashKit.DrawText("Press any key to exit", SplashKitSDK.Color.White, "Arial", 16, 200, 280);
            }
            SplashKit.RefreshScreen();
            SplashKit.Delay(2000); // Show message for 2 seconds

            SplashKit.FreeAllSprites();
        }
    }
}