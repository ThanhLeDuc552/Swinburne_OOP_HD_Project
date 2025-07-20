using System;
using System.Collections.Generic;
using System.Linq;
using SplashKitSDK;
using DotTiled;

namespace Swinburne_OOP_HD
{
    public class Level : IDisposable
    {
        // Object managers - managed by LevelLoader
        private ObjectManager<Character> _characterManager;
        private ObjectManager<Hazards> _hazardManager;
        private ObjectManager<ExitDoor> _exitDoorManager;
        private ObjectManager<Diamond> _diamondManager;
        private ObjectManager<Platform> _platformManager;
        private ObjectManager<Lever> _leverManager;
        private ObjectManager<Button> _buttonManager;
        private ObjectManager<Box> _boxManager;
        private CompositeObjectManager _compositeManager;

        // Level rendering data
        private LevelData _levelData;
        private Dictionary<uint, DrawingOptions> _rotationOptions;
        private Bitmap _background;

        // Game state tracking
        private bool _gameOver;
        private bool _levelCompleted;
        private bool _fireExitReached;
        private bool _waterExitReached;

        // Timer for level score calculation
        private SplashKitSDK.Timer _levelTimer;

        // Physics and collision systems
        private PhysicsSystem _physicsSystem;
        private CollisionManager _collisionManager;

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

            // Create all object managers using LevelLoader
            var managers = LevelLoader.CreateObjectManagers();
            _characterManager = managers.characters;
            _hazardManager = managers.hazards;
            _exitDoorManager = managers.exitDoors;
            _diamondManager = managers.diamonds;
            _platformManager = managers.platforms;
            _leverManager = managers.levers;
            _buttonManager = managers.buttons;
            _boxManager = managers.boxes;
            _compositeManager = managers.composite;

            // Create game objects from TMX data using LevelLoader
            if (_levelData.ObjectLayer != null)
            {
                LevelLoader.CreateGameObjects(
                    _levelData.ObjectLayer,
                    _characterManager,
                    _hazardManager,
                    _exitDoorManager,
                    _diamondManager,
                    _platformManager,
                    _leverManager,
                    _buttonManager,
                    _boxManager
                );
            }

            // Load background image
            if (!string.IsNullOrEmpty(_levelData.BackgroundImagePath))
            {
                _background = new Bitmap("Background", _levelData.BackgroundImagePath);
            }

            // Initialize game systems
            _physicsSystem = new PhysicsSystem();
            _collisionManager = new CollisionManager();

            // Start level timer
            _levelTimer = SplashKit.CreateTimer("LevelTimer");
            SplashKit.StartTimer(_levelTimer);
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
            // Update platforms first (moving animation)
            _platformManager.UpdateAll();
            
            // Draw all objects
            _compositeManager.DrawAll();
        }

        public void Update()
        {
            UpdateCharacters();
            CheckCollisions();
            CheckLevelCompletion();
        }

        private void UpdateCharacters()
        {
            foreach (Character character in _characterManager.Objects)
            {
                character.Update();
                _physicsSystem.UpdateCharacter(character, this);
            }
        }

        private void CheckCollisions()
        {
            var characters = _characterManager.Objects;

            // Hazard collisions
            foreach (Character character in characters)
            {
                _collisionManager.CheckHazardCollisions(character, _hazardManager.Objects, ref _gameOver);
            }

            // Diamond interactions
            foreach (Character character in characters)
            {
                List<Diamond> diamondsToRemove = _collisionManager.CheckDiamondInteraction(character, _diamondManager.Objects);
                foreach (Diamond diamond in diamondsToRemove)
                {
                    _diamondManager.Remove(diamond);
                }
            }

            // Platform interactions
            foreach (Character character in characters)
            {
                _collisionManager.CheckPlatformInteractions(character, _platformManager.Objects, _physicsSystem);
            }

            // Lever interactions
            foreach (Character character in characters)
            {
                _collisionManager.CheckLeverInteractions(character, _leverManager.Objects, _platformManager.Objects);
            }

            // Button interactions
            _collisionManager.CheckButtonInteractions(characters, _buttonManager.Objects, _platformManager.Objects);

            // Box interactions
            _collisionManager.CheckBoxInteractions();

            // Exit door interactions
            if (_characterManager.Count >= 2)
            {
                _collisionManager.CheckExitDoorInteractions(
                    _characterManager.Objects[0], 
                    _characterManager.Objects[1], 
                    _exitDoorManager.Objects, 
                    ref _fireExitReached, 
                    ref _waterExitReached
                );
            }
        }

        private void CheckLevelCompletion()
        {
            if (_fireExitReached && _waterExitReached && !_levelCompleted)
            {
                _levelCompleted = true;
            }
        }

        public int CalculateScore(int allowedTimeSeconds = 40)
        {
            if (_gameOver) return 0;

            int score = 3;

            // Check time
            double elapsedSeconds = SplashKit.TimerTicks(_levelTimer) / 1000.0;
            if (elapsedSeconds > allowedTimeSeconds)
                score--;

            // Check if all diamonds collected
            if (_diamondManager.Count > 0)
                score--;

            // Clamp score between 0 and 3
            return Math.Max(0, Math.Min(3, score));
        }

        // Properties for game state
        public bool IsGameOver => _gameOver;
        public bool IsLevelCompleted => _levelCompleted;
        public bool FireExitReached => _fireExitReached;
        public bool WaterExitReached => _waterExitReached;

        // Map properties (for physics system compatibility)
        public uint[] MapData => _levelData.MapData ?? new uint[0];
        public uint MapWidth => _levelData.MapWidth;
        public uint MapHeight => _levelData.MapHeight;

        public void Dispose()
        {
            // Dispose bitmaps and clear managers
            _background?.Free();
            
            foreach (var tile in _levelData.Tiles.Values)
            {
                tile?.Free();
            }
            _levelData.Tiles.Clear();

            _characterManager.Clear();
            _hazardManager.Clear();
            _exitDoorManager.Clear();
            _diamondManager.Clear();
            _platformManager.Clear();
            _leverManager.Clear();
            _buttonManager.Clear();
            _boxManager.Clear();
            _levelTimer.Stop();
            SplashKit.FreeTimer(_levelTimer);
        }
    }
}
