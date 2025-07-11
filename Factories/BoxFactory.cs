using System;
using System.Collections.Generic;
using SplashKitSDK;

namespace Swinburne_OOP_HD
{
    public class BoxFactory : IGameObjectFactory
    {
        private Dictionary<string, Func<DotTiled.Object, Box>> _boxCreators;

        public BoxFactory()
        {
            _boxCreators = new Dictionary<string, Func<DotTiled.Object, Box>>()
            {
                {"Box", (objData) => new Box(objData)},
            };
        }

        public GameObject CreateGameObject(string name, Point2D position, DotTiled.Object objData = null)
        {
            if (_boxCreators.ContainsKey(name))
            {
                return _boxCreators[name](objData);
            }

            throw new Exception($"Box type {name} not found");
        }
    }
}