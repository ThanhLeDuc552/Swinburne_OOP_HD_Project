using DotTiled;
using DotTiled.Serialization;
using DotTiled.Serialization.Tmx;
using System.Collections.Generic;
using System.Linq;
using SplashKitSDK;

namespace Swinburne_OOP_HD
{
    public class LevelLoader
    {
        private static Loader _loader = Loader.Default();

        public static Map LoadMap(string filePath)
        {
            return _loader.LoadMap(filePath);
        }
        
        public static LevelData ExtractLevelData(Map map)
        {
            LevelData levelData = new LevelData();

            // Extract map properties
            levelData.TileWidth = map.TileWidth;
            levelData.TileHeight = map.TileHeight;
            levelData.MapWidth = map.Width;
            levelData.MapHeight = map.Height;

            // Extract layers
            foreach (BaseLayer layer in map.Layers)
            {
                if (layer is ObjectLayer objectLayer)
                {
                    levelData.ObjectLayer = objectLayer;
                }
                else if (layer is TileLayer tileLayer)
                {
                    levelData.TileLayer = tileLayer;
                    levelData.MapData = tileLayer.Data.Value.GlobalTileIDs.Value;
                    levelData.FlippingFlags = tileLayer.Data.Value.FlippingFlags.Value;
                }
                else if (layer is ImageLayer imageLayer)
                {
                    levelData.BackgroundImagePath = imageLayer.Image.Value.Source.Value;
                }
            }

            // Load tiles from tilesets
            levelData.Tiles = LoadTiles(map);

            return levelData;
        }

        public static void CreateGameObjects(
            ObjectLayer objectLayer,
            ObjectManager<Character> characterManager,
            ObjectManager<Hazards> hazardManager,
            ObjectManager<ExitDoor> exitDoorManager,
            ObjectManager<Diamond> diamondManager,
            ObjectManager<Platform> platformManager,
            ObjectManager<Lever> leverManager,
            ObjectManager<Button> buttonManager,
            ObjectManager<Box> boxManager)
        {
            GameObjectFactoryManager factoryManager = new GameObjectFactoryManager();

            foreach (DotTiled.Object obj in objectLayer.Objects)
            {
                Point2D position = new Point2D() { X = obj.X, Y = obj.Y };

                switch (obj.Type)
                {
                    case "StartPos":
                        HandleCharacterStartPosition(obj, characterManager, position);
                        break;

                    case "Hazard":
                        Hazards hazard = factoryManager.GetFactory("Hazard").CreateGameObject(obj.Name, position, obj) as Hazards;
                        if (hazard != null) hazardManager.Add(hazard);
                        break;

                    case "ExitDoor":
                        ExitDoor exitDoor = factoryManager.GetFactory("ExitDoor").CreateGameObject(obj.Name, position, obj) as ExitDoor;
                        if (exitDoor != null) exitDoorManager.Add(exitDoor);
                        break;

                    case "Diamond":
                        Diamond diamond = factoryManager.GetFactory("Diamond").CreateGameObject(obj.Name, position, obj) as Diamond;
                        if (diamond != null) diamondManager.Add(diamond);
                        break;

                    case "Platform":
                        Platform platform = factoryManager.GetFactory("Platform").CreateGameObject(obj.Name, position, obj) as Platform;
                        if (platform != null) platformManager.Add(platform);
                        break;

                    case "Lever":
                        Lever lever = factoryManager.GetFactory("Lever").CreateGameObject(obj.Name, position, obj) as Lever;
                        if (lever != null) leverManager.Add(lever);
                        break;

                    case "Button":
                        Button button = factoryManager.GetFactory("Button").CreateGameObject(obj.Name, position, obj) as Button;
                        if (button != null) buttonManager.Add(button);
                        break;

                    case "Box":
                        Box box = factoryManager.GetFactory("Box").CreateGameObject(obj.Name, position, obj) as Box;
                        if (box != null) boxManager.Add(box);
                        break;
                }
            }
        }

        // Handles setting character start positions from TMX start position objects
        private static void HandleCharacterStartPosition(DotTiled.Object obj, ObjectManager<Character> characterManager, Point2D position)
        {
            if (obj.Properties != null && obj.Properties.Count > 0)
            {
                var characterType = (obj.Properties[0] as StringProperty)?.Value;
                if (!string.IsNullOrEmpty(characterType))
                {
                    var character = characterManager.Objects.FirstOrDefault(c => c.Type == characterType);
                    if (character != null)
                    {
                        character.Position = SplashKit.VectorTo(position.X, position.Y);
                    }
                }
            }
        }

        // Loads tile bitmaps from map tilesets
        private static Dictionary<uint, Bitmap> LoadTiles(Map map)
        {
            var tiles = new Dictionary<uint, Bitmap>();

            foreach (Tileset tileset in map.Tilesets)
            {
                if (tileset.Name == "Bricks") // You can extend this to handle multiple tilesets
                {
                    foreach (Tile tile in tileset.Tiles)
                    {
                        string imgPath = tile.Image.Value.Source.Value;
                        string[] pathParts = imgPath.Split('/');
                        string newImgPath = string.Join("/", pathParts.Skip(1)); // Skip first empty part

                        Bitmap bmp = SplashKit.LoadBitmap($"{tileset.Name} ({tile.ID})", newImgPath);
                        bmp.SetCellDetails(16, 16, 1, 1, 1);

                        tiles[tile.ID + tileset.FirstGID] = bmp;
                    }

                    break; // Assuming only one tileset is used for now
                }
            }

            return tiles;
        }

        // Creates the default rotation options for tile rendering
        public static Dictionary<uint, DrawingOptions> CreateRotationOptions()
        {
            return new Dictionary<uint, DrawingOptions>
            {
                { 0u, SplashKit.OptionRotateBmp(0) },
                { 2684354560u, SplashKit.OptionRotateBmp(90) },
                { 3221225472u, SplashKit.OptionRotateBmp(180) },
                { 1610612736u, SplashKit.OptionRotateBmp(270) }
            };
        }

        // Loads and creates all object managers with initial characters
        public static (
            ObjectManager<Character> characters,
            ObjectManager<Hazards> hazards,
            ObjectManager<ExitDoor> exitDoors,
            ObjectManager<Diamond> diamonds,
            ObjectManager<Platform> platforms,
            ObjectManager<Lever> levers,
            ObjectManager<Button> buttons,
            ObjectManager<Box> boxes,
            CompositeObjectManager composite
        ) CreateObjectManagers()
        {
            var characterManager = new ObjectManager<Character>();
            var hazardManager = new ObjectManager<Hazards>();
            var exitDoorManager = new ObjectManager<ExitDoor>();
            var diamondManager = new ObjectManager<Diamond>();
            var platformManager = new ObjectManager<Platform>();
            var leverManager = new ObjectManager<Lever>();
            var buttonManager = new ObjectManager<Button>();
            var boxManager = new ObjectManager<Box>();

            // Add initial characters
            characterManager.Add(new FireBoy());
            characterManager.Add(new WaterGirl());

            // Create composite manager
            var compositeManager = new CompositeObjectManager();
            compositeManager.RegisterManager(boxManager);
            compositeManager.RegisterManager(buttonManager);
            compositeManager.RegisterManager(leverManager);
            compositeManager.RegisterManager(diamondManager);
            compositeManager.RegisterManager(exitDoorManager);
            compositeManager.RegisterManager(characterManager);
            compositeManager.RegisterManager(platformManager);
            compositeManager.RegisterManager(hazardManager);

            return (characterManager, hazardManager, exitDoorManager, diamondManager, 
                   platformManager, leverManager, buttonManager, boxManager, compositeManager);
        }

        public static LevelValidationResult ValidateLevel(Map map)
        {
            var result = new LevelValidationResult { IsValid = true };

            // Check for required layers
            bool hasObjectLayer = map.Layers.OfType<ObjectLayer>().Any();
            bool hasTileLayer = map.Layers.OfType<TileLayer>().Any();
            bool hasImageLayer = map.Layers.OfType<ImageLayer>().Any();

            if (!hasObjectLayer)
            {
                result.IsValid = false;
                result.ErrorMessages.Add("Missing required ObjectLayer");
            }

            if (!hasTileLayer)
            {
                result.IsValid = false;
                result.ErrorMessages.Add("Missing required TileLayer");
            }

            if (!hasImageLayer)
            {
                result.IsValid = false;
                result.ErrorMessages.Add("Missing required ImageLayer for background");
            }

            // Check for required objects (you can expand this as needed)
            var objectLayer = map.Layers.OfType<ObjectLayer>().FirstOrDefault();
            if (objectLayer != null)
            {
                bool hasFireBoyStart = objectLayer.Objects.Any(o => o.Type == "StartPos" && 
                    o.Properties?.OfType<StringProperty>().Any(p => p.Value == "fire") == true);
                bool hasWaterGirlStart = objectLayer.Objects.Any(o => o.Type == "StartPos" && 
                    o.Properties?.OfType<StringProperty>().Any(p => p.Value == "water") == true);

                if (!hasFireBoyStart)
                {
                    result.IsValid = false;
                    result.ErrorMessages.Add("Missing FireBoy start position");
                }

                if (!hasWaterGirlStart)
                {
                    result.IsValid = false;
                    result.ErrorMessages.Add("Missing WaterGirl start position");
                }
            }

            return result;
        }
    }
}