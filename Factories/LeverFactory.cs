using System;
using System.Collections.Generic;
using SplashKitSDK;

namespace Swinburne_OOP_HD 
{
    public class LeverFactory : IGameObjectFactory 
    {
        private Dictionary<string, Func<DotTiled.Object, Lever>> _leverCreators;

        public LeverFactory() 
        {
            _leverCreators = new Dictionary<string, Func<DotTiled.Object, Lever>>() 
            {
                {"LeverOld", (objData) => new LeverOld(objData)},
                {"LeverNew", (objData) => new LeverNew(objData)},
            };
        }

        public GameObject CreateGameObject(string name, Point2D position, DotTiled.Object objData = null) 
        {
            if (_leverCreators.ContainsKey(name)) 
            {
                return _leverCreators[name](objData);
            }
            throw new Exception($"Lever type {name} not found");
        }
    }
}