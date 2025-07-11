using SplashKitSDK;

namespace Swinburne_OOP_HD 
{
    public class DiamondFactory : IGameObjectFactory 
    {
        private Dictionary<string, Func<Point2D, Diamond>> _diamondCreators;

        public DiamondFactory() 
        {
            _diamondCreators = new Dictionary<string, Func<Point2D, Diamond>>() 
            {
                {"BlueDiamond", (position) => new BlueDiamond(position)},
                {"MudDiamond", (position) => new MudDiamond(position)},
                {"RedDiamond", (position) => new RedDiamond(position)},
            };
        }

        public GameObject CreateGameObject(string name, Point2D position, DotTiled.Object objData = null) 
        {
            if (_diamondCreators.ContainsKey(name)) 
            {
                return _diamondCreators[name](position);
            }

            throw new Exception($"Diamond type {name} not found");
        }
    }
}