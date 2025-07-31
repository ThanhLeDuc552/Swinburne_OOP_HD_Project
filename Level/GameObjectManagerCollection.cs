using System;

namespace Swinburne_OOP_HD
{
    public class GameObjectManagerCollection : IDisposable
    {
        public ObjectManager<Character> Characters { get; private set; }
        public ObjectManager<Hazards> Hazards { get; private set; }
        public ObjectManager<ExitDoor> ExitDoors { get; private set; }
        public ObjectManager<Diamond> Diamonds { get; private set; }
        public ObjectManager<Platform> Platforms { get; private set; }
        public ObjectManager<Lever> Levers { get; private set; }
        public ObjectManager<Button> Buttons { get; private set; }
        public ObjectManager<Box> Boxes { get; private set; }
        public CompositeObjectManager Composite { get; private set; }

        public GameObjectManagerCollection()
        {
            InitializeManagers();
        }

        private void InitializeManagers()
        {
            var managers = LevelLoader.CreateObjectManagers();
            Characters = managers.characters;
            Hazards = managers.hazards;
            ExitDoors = managers.exitDoors;
            Diamonds = managers.diamonds;
            Platforms = managers.platforms;
            Levers = managers.levers;
            Buttons = managers.buttons;
            Boxes = managers.boxes;
            Composite = managers.composite;
        }

        public void InitializeGameObjects(LevelData levelData)
        {
            if (levelData.ObjectLayer != null)
            {
                LevelLoader.CreateGameObjects(
                    levelData.ObjectLayer,
                    Characters,
                    Hazards,
                    ExitDoors,
                    Diamonds,
                    Platforms,
                    Levers,
                    Buttons,
                    Boxes
                );
            }
        }

        public void UpdateAllPlatforms()
        {
            Platforms.UpdateAll();
        }

        public void Dispose()
        {
            Hazards?.Clear();
            ExitDoors?.Clear();
            Diamonds?.Clear();
            Platforms?.Clear();
            Levers?.Clear();
            Buttons?.Clear();
            Boxes?.Clear();
        }
    }
}
