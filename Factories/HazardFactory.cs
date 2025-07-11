using System;
using System.Collections.Generic;
using SplashKitSDK;

namespace Swinburne_OOP_HD 
{
    public class HazardFactory : IGameObjectFactory 
    {
        private Dictionary<string, Func<Point2D, Hazards>> _hazardCreators;

        public HazardFactory()
        {
            _hazardCreators = new Dictionary<string, Func<Point2D, Hazards>>() 
            {
                {"MudHazard", (position) => new MudHazard(position)},
                {"WaterHazard", (position) => new WaterHazard(position)},
                {"LavaHazard", (position) => new LavaHazard(position)},
            };
        }

        public GameObject CreateGameObject(string name, Point2D position, DotTiled.Object objData = null) 
        {
            if (_hazardCreators.ContainsKey(name)) 
            {
                return _hazardCreators[name](position);
            }
            
            throw new Exception($"Hazard type {name} not found");
        }
    }
} 