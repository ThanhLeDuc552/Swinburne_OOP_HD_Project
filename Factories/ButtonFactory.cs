using SplashKitSDK;

namespace Swinburne_OOP_HD 
{
    public class ButtonFactory : IGameObjectFactory 
    {
        private Dictionary<string, Func<DotTiled.Object, Button>> _buttonCreators;

        public ButtonFactory() 
        {
            _buttonCreators = new Dictionary<string, Func<DotTiled.Object, Button>>() 
            {
                {"Button", (objData) => new Button(objData)},
            };
        }

        public GameObject CreateGameObject(string name, Point2D position, DotTiled.Object objData = null) 
        {
            if (_buttonCreators.ContainsKey(name)) 
            {
                return _buttonCreators[name](objData);
            }

            throw new Exception($"Button type {name} not found");
        }
    }
}