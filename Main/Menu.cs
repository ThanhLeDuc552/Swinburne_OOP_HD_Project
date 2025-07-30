using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using SplashKitSDK;
using System.IO;

namespace Swinburne_OOP_HD
{
    public class Menu
    {
        private Bitmap _background;
        private Bitmap _logo;
        private Bitmap _playButton;
        private (FireBoy fireBoy, WaterGirl waterGirl) _characters;
        private Rectangle _playButtonRect;
        private bool _showLevelSelect = false;
        private int _selectedLevelIndex = -1;
        private LevelManager _levelManager;
        private int _levelCount; // Change as needed

        public Menu(LevelManager levelManager)
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

            _characters = (new FireBoy(), new WaterGirl());
            _levelManager = levelManager;
            _levelCount = CountLevels();
        }

        private int CountLevels()
        {
            return Directory.GetFiles(".", "*.tmx").Length;
        }

        public void Draw(Window window)
        {
            if (!_showLevelSelect)
            {
                _background.Draw(0, 0);
                _logo.Draw(60, 50);
                _characters.fireBoy.Idle.Sprite.Draw(50, 200);
                _characters.waterGirl.Idle.Sprite.Draw(350, 250);
                _characters.fireBoy.Idle.Sprite.UpdateAnimation();
                _characters.waterGirl.Idle.Sprite.UpdateAnimation();
                SplashKit.DrawBitmap(_playButton, _playButtonRect.X, _playButtonRect.Y);
            }
            else
            {
                // Draw level selection
                SplashKit.ClearScreen(SplashKitSDK.Color.Black);
                for (int i = 0; i < _levelCount; i++)
                {
                    string levelName = $"Level{i + 1}";
                    string status = _levelManager.GetLevelStatus(levelName);
                    string display = $"Level {i + 1}: {status}";
                    SplashKit.DrawText(display, SplashKitSDK.Color.White, "Arial", 20, 180, 120 + i * 40);
                }
                SplashKit.DrawText("Click a level to play", SplashKitSDK.Color.Yellow, "Arial", 16, 200, 120 + _levelCount * 40);
            }
        }

        public int HandleInput(Window window)
        {
            if (!_showLevelSelect)
            {
                if (SplashKit.MouseClicked(MouseButton.LeftButton) && SplashKit.PointInRectangle(SplashKit.MousePosition(), _playButtonRect))
                {
                    _showLevelSelect = true;
                }
            }
            else
            {
                // Handle level selection
                for (int i = 0; i < _levelCount; i++)
                {
                    int y = 120 + i * 40;
                    Rectangle levelRect = new Rectangle() { X = 180, Y = y, Width = 300, Height = 30 };
                    if (SplashKit.MouseClicked(MouseButton.LeftButton) && SplashKit.PointInRectangle(SplashKit.MousePosition(), levelRect))
                    {
                        _selectedLevelIndex = i;
                        _characters = (null, null); // Reset characters for new level
                        return _selectedLevelIndex;
                    }
                }
            }
            return -1;
        }

        public void Reset()
        {
            _showLevelSelect = false;
            _selectedLevelIndex = -1;
        }
    }
}
