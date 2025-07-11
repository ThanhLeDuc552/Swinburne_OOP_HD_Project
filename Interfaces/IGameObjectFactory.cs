using SplashKitSDK;

namespace Swinburne_OOP_HD 
{
    public interface IGameObjectFactory 
    {
        GameObject CreateGameObject(string name, Point2D position, DotTiled.Object objData = null);
    }
}