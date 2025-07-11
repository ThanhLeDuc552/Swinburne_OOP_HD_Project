using System;
using System.Collections.Generic;
using SplashKitSDK;

namespace Swinburne_OOP_HD 
{
    public class ExitDoorFactory : IGameObjectFactory 
    {
        private Dictionary<string, Func<DotTiled.Object, ExitDoor>> _exitDoorCreators;

        public ExitDoorFactory() 
        {
            _exitDoorCreators = new Dictionary<string, Func<DotTiled.Object, ExitDoor>>()
            {
                {"ExitDoorFire", (objData) => new FireExitDoor(objData)},
                {"ExitDoorWater", (objData) => new WaterExitDoor(objData)},
            };
        }

        public GameObject CreateGameObject(string name, Point2D position, DotTiled.Object objData = null) 
        {
            if (_exitDoorCreators.ContainsKey(name)) {
                return _exitDoorCreators[name](objData);
            }
            throw new Exception($"Exit door type {name} not found");
        }
    }
} 