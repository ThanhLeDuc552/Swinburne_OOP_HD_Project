using System;
using System.Collections.Generic;
using SplashKitSDK;

namespace Swinburne_OOP_HD 
{
    public class PlatformFactory : IGameObjectFactory 
    {
        private Dictionary<string, Func<DotTiled.Object, Platform>> _platformCreators;

        public PlatformFactory() 
        {
            _platformCreators = new Dictionary<string, Func<DotTiled.Object, Platform>>() 
            {
                {"PlatformVertical", (objData) => new PlatformVertical(objData)},
                {"PlatformHorizontal", (objData) => new PlatformHorizontal(objData)},
            };
        }

        public GameObject CreateGameObject(string name, Point2D position, DotTiled.Object objData = null) 
        {
            if (_platformCreators.ContainsKey(name)) 
            {
                return _platformCreators[name](objData);
            }

            throw new Exception($"Platform type {name} not found");
        }
    }
}