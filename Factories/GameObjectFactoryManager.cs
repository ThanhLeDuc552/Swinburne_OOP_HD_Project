using System;
using System.Collections.Generic;
using SplashKitSDK;

namespace Swinburne_OOP_HD 
{
    public class GameObjectFactoryManager 
    {
        private Dictionary<string, IGameObjectFactory> _factories;

        public GameObjectFactoryManager() 
        {
            _factories = new Dictionary<string, IGameObjectFactory>() 
            {
                {"Hazard", new HazardFactory()},
                {"ExitDoor", new ExitDoorFactory()},
                {"Diamond", new DiamondFactory()},
                {"Platform", new PlatformFactory()},
                {"Lever", new LeverFactory()},
                {"Button", new ButtonFactory()},
                {"Box", new BoxFactory()},
            };
        }

        public IGameObjectFactory GetFactory(string type) 
        {
            if (_factories.ContainsKey(type)) 
            {
                return _factories[type];
            }
            throw new Exception($"Factory type {type} not found");
        }
    }
}