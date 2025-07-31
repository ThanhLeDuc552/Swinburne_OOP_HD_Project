using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SplashKitSDK;

namespace Swinburne_OOP_HD
{
    public class LevelLogic
    {
        // Singleton pattern
        private static LevelLogic? _instance;

        // Time limits for different difficulty levels
        private Dictionary<string, int> _timeLimits;
        private SplashKitSDK.Timer _levelTimer;

        // Physics and collision systems    
        private PhysicsSystem _physicsSystem;
        private CollisionManager _collisionManager;

        // Game object managers collection
        private GameObjectManagerCollection _managers;

        private LevelLogic()
        {
            _timeLimits = new Dictionary<string, int>
            {
                { "Easy", 40 },
                { "Medium", 35 },
                { "Hard", 30 }
            };

            _physicsSystem = new PhysicsSystem();
            _collisionManager = new CollisionManager();
            _levelTimer = SplashKit.CreateTimer("NewTimer");
            _managers = new GameObjectManagerCollection();
        }

        public static LevelLogic Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new LevelLogic();
                }

                return _instance;
            }
        }

        public void InitializeGameObject(LevelData levelData)
        {
            _managers.InitializeGameObjects(levelData);
        }

        public void UpdateLevel(Level level)
        {
            if (!_levelTimer.IsStarted)
            {
                _levelTimer.Start();
            }
            _managers.UpdateAllPlatforms();
            UpdateCharacter(level);
            CheckCollision(level);
            CheckLevelCompletion(level);
        }

        public int CalculateScore(Level level)
        {
            if (level.IsGameOver) return 0;

            int maxScore = 3;
            int timeLimit = _timeLimits[level.Difficulty];

            double elapsedSeconds = SplashKit.TimerTicks(_levelTimer) / 1000.0;
            if (elapsedSeconds > timeLimit)
                maxScore--;

            // Check if all diamonds collected
            if (_managers.Diamonds.Count > 0)
                maxScore--;

            // Clamp score between 0 and 3
            return Math.Max(0, Math.Min(3, maxScore));
        }

        public void ResetForNewLevel()
        {
            _managers?.Dispose();
            _levelTimer?.Stop();
            _levelTimer?.Reset();
        }
        
        public GameObjectManagerCollection Managers
        {
            get { return _managers; }
        }

        private void CheckCollision(Level level)
        {
            var characters = _managers.Characters.Objects;

            // Hazard collisions
            _collisionManager.CheckHazardCollisions(characters, _managers.Hazards.Objects);
            foreach (Character character in characters)
            {
                if (character.IsDead)
                {
                    level.IsGameOver = true;
                    return;
                }
            }

            // Diamond interactions
            List<Diamond> collectedDiamonds = _collisionManager.CheckDiamondInteraction(characters, _managers.Diamonds.Objects);
            foreach (Diamond diamond in collectedDiamonds)
            {
                _managers.Diamonds.Remove(diamond);
            }

            // Platform interactions
            _collisionManager.CheckPlatformInteractions(characters, _managers.Platforms.Objects, _physicsSystem);

            // Lever interactions
            _collisionManager.CheckLeverInteractions(characters, _managers.Levers.Objects, _managers.Platforms.Objects);

            // Button interactions
            _collisionManager.CheckButtonInteractions(characters, _managers.Buttons.Objects, _managers.Platforms.Objects);

            // Box interactions
            _collisionManager.CheckBoxInteractions();

            // Exit door interactions
            _collisionManager.CheckExitDoorInteractions(
                _managers.Characters.Objects[0],
                _managers.Characters.Objects[1],
                _managers.ExitDoors.Objects,
                level
            );
        }

        private void CheckLevelCompletion(Level level)
        {
            if (level.IsGameOver || (level.FireExitReached && level.WaterExitReached && !level.IsLevelCompleted))
            {
                level.IsLevelCompleted = true;
            }
        }

        private void UpdateCharacter(Level level)
        {
            foreach (Character character  in _managers.Characters.Objects)
            {
                character.Update();
                _physicsSystem.UpdateCharacter(character, level);
            }
        }
    }
}
