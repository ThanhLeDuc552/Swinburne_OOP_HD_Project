using SplashKitSDK;

namespace Swinburne_OOP_HD 
{
    public enum GameEventType
    {
        PlayerDied,
        LevelCompleted,


    }
    // Observer interface for game events
    // To be implemented
    public interface IGameEventObserver 
    {
        void OnGameEvent(GameEvent gameEvent);
    }

    public class GameEvent 
    {
        
    }
}