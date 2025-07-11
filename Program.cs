using System;
using SplashKitSDK;

namespace Swinburne_OOP_HD
{
    public class Program
    {
        public static void Main()
        {
            Window newWindow = new Window("FireBoy and WaterGirl", 624, 464);
            GameStateManager gameStateManager = new GameStateManager(newWindow);
            gameStateManager.Run();
        }
    }


}
