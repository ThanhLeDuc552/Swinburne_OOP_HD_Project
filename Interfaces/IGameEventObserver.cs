using SplashKitSDK;
using System.Collections.Generic;

namespace Swinburne_OOP_HD 
{
    public enum GameEventType
    {
        PlayerDied,
        LevelCompleted,
        DiamondCollected,
        // Add more as needed
    }

    public class GameEvent 
    {
        public GameEventType EventType { get; }
        public object Data { get; }
        public GameEvent(GameEventType type, object data = null)
        {
            EventType = type;
            Data = data;
        }
    }

    public interface IGameEventObserver 
    {
        void OnGameEvent(GameEvent gameEvent);
    }

    public class GameEventManager
    {
        private static GameEventManager _instance;
        private List<IGameEventObserver> _observers = new List<IGameEventObserver>();
        private GameEventManager() { }
        public static GameEventManager Instance => _instance ?? (_instance = new GameEventManager());
        public void RegisterObserver(IGameEventObserver observer)
        {
            if (!_observers.Contains(observer))
                _observers.Add(observer);
        }
        public void UnregisterObserver(IGameEventObserver observer)
        {
            if (_observers.Contains(observer))
                _observers.Remove(observer);
        }
        public void Notify(GameEvent gameEvent)
        {
            foreach (var observer in _observers)
                observer.OnGameEvent(gameEvent);
        }
    }
}