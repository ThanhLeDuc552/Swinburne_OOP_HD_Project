using System;
using System.Collections.Generic;
using System.Linq;
using SplashKitSDK;
using DotTiled;

namespace Swinburne_OOP_HD
{
    public class Level : IDisposable
    {
        // Level rendering data
        private LevelData _levelData;
        private Dictionary<uint, DrawingOptions> _rotationOptions;
        private Bitmap? _background;

        // Game state tracking
        private bool _gameOver;
        private bool _levelCompleted;
        private bool _fireExitReached;
        private bool _waterExitReached;
        private string? _difficulty;
        private LevelLogic _levelLogic;

        public Level(string tmxFilePath)
        {
            // Load the TMX file using LevelLoader
            Map map = LevelLoader.LoadMap(tmxFilePath);

            // Validate the level before proceeding
            var validationResult = LevelLoader.ValidateLevel(map);
            if (!validationResult.IsValid)
            {
                throw new ArgumentException($"Invalid level file: {string.Join(", ", validationResult.ErrorMessages)}");
            }

            // Extract level data using LevelLoader
            _levelData = LevelLoader.ExtractLevelData(map);

            // Initialize game state
            _gameOver = false;
            _levelCompleted = false;
            _fireExitReached = false;
            _waterExitReached = false;

            // Create rotation options for tile rendering
            _rotationOptions = LevelLoader.CreateRotationOptions();

            // Load background image
            if (!string.IsNullOrEmpty(_levelData.BackgroundImagePath))
            {
                _background = new Bitmap("Background", _levelData.BackgroundImagePath);
            }

            // Initialize LevelLogic
            _levelLogic = LevelLogic.Instance;
            _levelLogic.InitializeGameObject(_levelData);
        }

        public void Draw()
        {
            DrawBackground();
            DrawTileLayer();
            DrawAllObjects();
        }

        private void DrawBackground()
        {
            _background?.Draw(0, 0);
        }

        private void DrawTileLayer()
        {
            if (_levelData.MapData == null || _levelData.FlippingFlags == null) return;

            // Render tiles based on map data
            for (uint i = 0; i < _levelData.MapHeight; i++)
            {
                for (uint j = 0; j < _levelData.MapWidth; j++)
                {
                    uint gid = _levelData.MapData[i * _levelData.MapWidth + j];
                    if (_levelData.Tiles.ContainsKey(gid))
                    {
                        uint key = (uint)_levelData.FlippingFlags[i * _levelData.MapWidth + j];
                        if (_rotationOptions.ContainsKey(key))
                        {
                            SplashKit.DrawBitmap(
                                _levelData.Tiles[gid], 
                                j * _levelData.TileWidth, 
                                i * _levelData.TileHeight, 
                                _rotationOptions[key]
                            );
                        }
                    }
                }
            }
        }

        private void DrawAllObjects()
        {
            // Draw all objects
            _levelLogic.Managers.Composite.DrawAll();
        }

        public void Update()
        {
            // All logical updates are handled by LevelLogic
            _levelLogic.UpdateLevel(this);
        }

        // Properties for game state
        public bool IsGameOver
        {
            get { return _gameOver; }
            set { _gameOver = value; }
        }

        public bool IsLevelCompleted
        {
            get { return _levelCompleted; }
            set { _levelCompleted = value; }
        }

        public bool FireExitReached
        {
            get { return _fireExitReached; }
            set { _fireExitReached = value; }
        }

        public bool WaterExitReached
        {
            get { return _waterExitReached; }
            set { _waterExitReached = value; }
        }

        public string Difficulty
        {
            get
            {
                if (string.IsNullOrEmpty(_difficulty))
                {
                    // Default to "Easy" if not set
                    _difficulty = "Easy";
                }
                return _difficulty;
            }
        }

        // Map properties (for physics system compatibility)
        public uint[] MapData => _levelData.MapData ?? new uint[0];
        public uint MapWidth => _levelData.MapWidth;
        public uint MapHeight => _levelData.MapHeight;

        // Score calculation
        public int CalculateScore() => _levelLogic.CalculateScore(this);

        public void Dispose()
        {
            _background?.Dispose();
            _levelLogic.ResetForNewLevel();
        }
    }
}
