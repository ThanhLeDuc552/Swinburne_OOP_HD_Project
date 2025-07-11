using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using SplashKitSDK;

namespace Swinburne_OOP_HD
{
    public class Menu
    {
        private Bitmap _background;
        private Bitmap _logo;
        private Bitmap _playButton;
        private FireBoy _fireBoy;
        private WaterGirl _waterGirl;
        private Rectangle _playButtonRect;

        public Menu() 
        {
            _background = SplashKit.LoadBitmap("MenuBackground", @"menu\345.png");
            _logo = SplashKit.LoadBitmap("Logo", @"menu\1.png");
            _playButton = SplashKit.LoadBitmap("PlayButton", @"menu\2.png");
            _playButtonRect = new Rectangle()
            {
                X = 250,
                Y = 250,
                Width = _playButton.Width,
                Height = _playButton.Height
            };

            _fireBoy = new FireBoy();
            _waterGirl = new WaterGirl();
        }

        public void Draw(Window window)
        {
            // Draw background
            _background.Draw(0, 0);
            // Draw logo
            _logo.Draw(60, 50);
            // Draw characters
            
            _fireBoy.Idle.Sprite.Draw(50, 200);
            _waterGirl.Idle.Sprite.Draw(350, 250);

            // Update animations
            _fireBoy.Idle.Sprite.UpdateAnimation();
            _waterGirl.Idle.Sprite.UpdateAnimation();
        }

        public void HandleInput(Window window)
        {
            if (SplashKit.BitmapButton(_playButton, _playButtonRect))
            {
                Console.WriteLine("Play button clicked!");
            }
        }
    }
}
