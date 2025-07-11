using System;
using SplashKitSDK;
using DotTiled;
using DotTiled.Serialization;
using DotTiled.Serialization.Tmx;

namespace Swinburne_OOP_HD
{
    public class Program
    {
        public static void Main()
        {
            Window newWindow = new Window("FireBoy and WaterGirl", 624, 464);

            Dictionary<string, Level> levels = new Dictionary<string, Level>()
            {
                //{ "Level1", _level1 }
            };

            Level level1 = new Level("Level1.tmx");

            int selected = 0;
            bool levelChosen = false;
            Menu menu = new Menu();

            /*

            while (!newWindow.CloseRequested && !_level1.IsGameOver && !_level1.IsLevelCompleted)
            {
                SplashKit.ClearScreen();
                SplashKit.ProcessEvents();

                // Then check collisions
                _level1.Update();

                // Finally draw everything
                _level1.Draw();

                SplashKit.DrawInterface();
                SplashKit.RefreshScreen();
            }

            // Handle end game states
            if (_level1.IsGameOver)
            {
                SplashKit.DrawText("GAME OVER!", SplashKitSDK.Color.Red, "Arial", 32, 250, 200);
                SplashKit.DrawText("Press any key to exit", SplashKitSDK.Color.White, "Arial", 16, 230, 250);
                SplashKit.RefreshScreen();
                SplashKit.Delay(2000); // Show message for 2 seconds
            }
            else if (_level1.IsLevelCompleted)
            {
                SplashKit.DrawText("LEVEL COMPLETED!", SplashKitSDK.Color.Green, "Arial", 32, 180, 200);
                SplashKit.DrawText("Press any key to exit", SplashKitSDK.Color.White, "Arial", 16, 200, 250);
                SplashKit.RefreshScreen();
                SplashKit.Delay(2000); // Show message for 2 seconds
            }

            SplashKit.FreeAllSprites();
            */


            

            while (!newWindow.CloseRequested)
            {
                SplashKit.ClearScreen();
                SplashKit.ProcessEvents();

                menu.Draw(newWindow);
                menu.HandleInput(newWindow);

                SplashKit.DrawInterface();
                SplashKit.RefreshScreen();
            }
            
        }
    }
}
