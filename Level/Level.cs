using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using SplashKitSDK;
using DotTiled;
using DotTiled.Serialization;
using DotTiled.Serialization.Tmx;

namespace Swinburne_OOP_HD
{
    public class Level
    {
        // Objects in the level
        private List<Character> _characters;
        private List<Hazards> _hazards;
        private List<ExitDoor> _exitDoors;
        private List<Diamond> _diamonds;
        private List<Platform> _platforms;
        private List<Lever> _levers;
        private List<Button> _buttons;
        private List<Box> _boxes;
        private Dictionary<uint, Bitmap> _tiles; // Dictionary to hold tile GID, Bitmap
        private Bitmap _background;

        // Layers in the level
        private ObjectLayer _objectLayer;
        private TileLayer _tileLayer;
        private ImageLayer _imageLayer;

        // Map properties
        private uint _tileWidth;
        private uint _tileHeight;
        private uint _mapWidth;
        private uint _mapHeight;
        private Dictionary<uint, DrawingOptions> _rotationOptions;
        private uint[] _mapData;

        // Characters positions
        private List<StartPos> _startPos;

        // Game state tracking
        private bool _gameOver;
        private bool _levelCompleted;
        private bool _fireExitReached;
        private bool _waterExitReached;

        private SplashKitSDK.Timer _levelTimer;

        public Level(string tmxFilePath)
        {
            Loader loader = Loader.Default();
            Map map = loader.LoadMap(tmxFilePath);

            // Initialize game state
            _gameOver = false;
            _levelCompleted = false;
            _fireExitReached = false;
            _waterExitReached = false;

            // Map properties
            foreach (BaseLayer layer in map.Layers) 
            {
                if (layer is ObjectLayer objectLayer) 
                {
                    _objectLayer = objectLayer;
                }

                if (layer is TileLayer tileLayer) 
                {
                    _tileLayer = tileLayer;
                }

                if (layer is ImageLayer imageLayer) 
                {
                    _imageLayer = imageLayer;
                }
            }

            _tileWidth = map.TileWidth;
            _tileHeight = map.TileHeight;
            _mapWidth = map.Width;
            _mapHeight = map.Height;
            _rotationOptions = new Dictionary<uint, DrawingOptions>
            {
                { 0u, SplashKit.OptionRotateBmp(0) },
                { 2684354560u, SplashKit.OptionRotateBmp(90) },
                { 3221225472u, SplashKit.OptionRotateBmp(180) },
                { 1610612736u, SplashKit.OptionRotateBmp(270) }
            };

            _characters = new List<Character>()
            {
                new FireBoy(),
                new WaterGirl()
            };
            _tiles = new Dictionary<uint, Bitmap>();
            _hazards = new List<Hazards>();
            _exitDoors = new List<ExitDoor>();
            _diamonds = new List<Diamond>();
            _startPos = new List<StartPos>();
            _platforms = new List<Platform>();
            _levers = new List<Lever>();
            _buttons = new List<Button>();
            _boxes = new List<Box>();
            _mapData = _tileLayer.Data.Value.GlobalTileIDs.Value;
            
            // Background
            _background = new Bitmap("Background", (map.Layers[0] as ImageLayer).Image.Value.Source.Value);

            _levelTimer = SplashKit.CreateTimer("LevelTimer");
            SplashKit.StartTimer(_levelTimer);

            ProcessTile(map);
            ProcessObject();
        }

        // Process objects in the level
        private void ProcessObject() 
        {
            GameObjectFactoryManager factoryManager = new GameObjectFactoryManager();

            foreach (DotTiled.Object obj in _objectLayer.Objects) 
            {
                Point2D position = new Point2D() { X = obj.X, Y = obj.Y };
                if (obj.Type == "StartPos") 
                {
                    foreach (Character character in _characters)
                    {
                        if (character.Type == (obj.Properties[0] as StringProperty).Value)
                        {
                            character.Position = SplashKit.VectorTo(position.X, position.Y);
                        }
                    }
                }
                else if (obj.Type == "Hazard") 
                {
                    Hazards hazard = factoryManager.GetFactory("Hazard").CreateGameObject(obj.Name, position, obj) as Hazards;
                    _hazards.Add(hazard);
                }
                else if (obj.Type == "ExitDoor") 
                {
                    ExitDoor exitDoor = factoryManager.GetFactory("ExitDoor").CreateGameObject(obj.Name, position, obj) as ExitDoor;
                    _exitDoors.Add(exitDoor);
                }
                else if (obj.Type == "Diamond") 
                {
                    Diamond diamond = factoryManager.GetFactory("Diamond").CreateGameObject(obj.Name, position, obj) as Diamond;
                    _diamonds.Add(diamond);
                }
                else if (obj.Type == "Platform") 
                {
                    Platform platform = factoryManager.GetFactory("Platform").CreateGameObject(obj.Name, position, obj) as Platform;
                    _platforms.Add(platform);
                }
                else if (obj.Type == "Lever") 
                {
                    Lever lever = factoryManager.GetFactory("Lever").CreateGameObject(obj.Name, position, obj) as Lever;
                    _levers.Add(lever);
                }
                else if (obj.Type == "Button") 
                {
                    Button button = factoryManager.GetFactory("Button").CreateGameObject(obj.Name, position, obj) as Button;
                    _buttons.Add(button);
                }
                else if (obj.Type == "Box") 
                {
                    Box box = factoryManager.GetFactory("Box").CreateGameObject(obj.Name, position, obj) as Box;
                    _boxes.Add(box);
                }
            }
        }

        // Process tileset to get the tiles and their corresponding bitmaps
        private void ProcessTile(Map map) 
        {
            foreach (Tileset tileset in map.Tilesets) 
            {
                if (tileset.Name == "Bricks")
                {
                    foreach (Tile tile in tileset.Tiles)
                    {
                        string imgPath = tile.Image.Value.Source.Value;
                        string[] pathParts = imgPath.Split('/');
                        string newImgPath = string.Join("/", pathParts.Skip(1)); // Skip first empty part

                        Bitmap bmp = SplashKit.LoadBitmap($"{tileset.Name} ({tile.ID})", newImgPath);
                        bmp.SetCellDetails(16, 16, 1, 1, 1);

                        _tiles[tile.ID + tileset.FirstGID] = bmp;
                    }

                    break;
                }
            }
        }

        public void Draw()
        {
            DrawImageLayer();
            DrawTileLayer(_tileLayer);
            DrawObjectLayer();
        }

        private void DrawImageLayer()
        {
            _background.Draw(0, 0);
        }

        private void DrawObjectLayer() 
        {
            foreach (Character character in _characters)
            {
                character.Draw();
            }

            foreach (Hazards hazard in _hazards) 
            {
                hazard.Draw();
            }

            foreach (ExitDoor door in _exitDoors) 
            {
                door.Draw();
            }

            foreach (Diamond diamond in _diamonds) 
            {
                diamond.Draw();
            }

            foreach (Platform platform in _platforms) 
            {
                platform.Draw();
                platform.Update();
            }

            foreach (Lever lever in _levers) 
            {
                lever.Draw();
            }

            foreach (Button button in _buttons) 
            {
                button.Draw();
            }

            foreach (Box box in _boxes) 
            {
                box.Draw();
            }
        }

        private void DrawTileLayer(TileLayer tileLayer)
        {
            FlippingFlags[] flags = tileLayer.Data.Value.FlippingFlags.Value;

            // Need to adjust to draw the tile-based objects
            for (uint i = 0; i < _mapHeight; i++)
            {
                for (uint j = 0; j < _mapWidth; j++)
                {
                    uint gid = _mapData[i * _mapWidth + j];
                    if (_tiles.ContainsKey(gid))
                    {
                        uint key = (uint)flags[i * tileLayer.Width + j];
                        SplashKit.DrawBitmap(_tiles[gid], j * _tileWidth, i * _tileHeight, _rotationOptions[key]);
                    }
                }
            }
        }

        public void Update()
        {
            foreach (Character character in _characters)
            {
                character.Update();
                Physics.UpdateCharacter(character, this);
            }

            // Hazard collisions
            foreach (Character character in _characters)
            {
                CollisionManager.CheckHazardCollisions(character, _hazards, ref _gameOver);
            }

            // Exit door interactions
            CollisionManager.CheckExitDoorInteractions(_characters[0], _characters[1], _exitDoors, ref _fireExitReached, ref _waterExitReached);
            if (_fireExitReached && _waterExitReached && !_levelCompleted)
            {
                _levelCompleted = true;
            }

            // Diamond interactions
            foreach (Character character in _characters)
            {
                CollisionManager.CheckDiamondInteraction(character, ref _diamonds);
            }

            // Platform interactions
            foreach (Character character in _characters)
            {
                CollisionManager.CheckPlatformInteractions(character, _platforms);
            }

            // Lever interactions
            foreach (Character character in _characters)
            {
                CollisionManager.CheckLeverInteractions(character, _levers, _platforms);
            }

            // Button interactions
            CollisionManager.CheckButtonInteractions(_characters, _buttons, _platforms);

            // Box interactions
            CollisionManager.CheckBoxInteractions();
            
        }

        public int CalculateScore(int allowedTimeSeconds = 40)
        {
            if (_gameOver)
                return 0;

            int score = 3;

            // Check time
            double elapsedSeconds = SplashKit.TimerTicks(_levelTimer) / 1000.0;
            if (elapsedSeconds > allowedTimeSeconds)
                score--;

            // Check if all diamonds collected
            if (_diamonds.Count > 0)
                score--;

            // Clamp score between 0 and 3
            if (score < 0) score = 0;
            if (score > 3) score = 3;

            return score;
        }

        // Properties for game state
        public bool IsGameOver { get { return _gameOver; } }
        public bool IsLevelCompleted { get { return _levelCompleted; } }
        public bool FireExitReached { get { return _fireExitReached; } }
        public bool WaterExitReached { get { return _waterExitReached; } }

        // Map properties
        public uint[] MapData { get { return _mapData; } }
        public uint MapWidth { get { return _mapWidth; } }
        public uint MapHeight { get { return _mapHeight; } }
    }
}
