namespace Swinburne_OOP_HD
{
    public class CompositeObjectManager
    {
        private List<IObjectManagerBase> _managers;

        public CompositeObjectManager()
        {
            _managers = new List<IObjectManagerBase>();
        }

        // Registers an object manager to be managed by this composite manager
        public void RegisterManager<T>(ObjectManager<T> manager) where T : GameObject
        {
            _managers.Add(manager);
        }

        // Calls DrawAll() on all registered managers
        public void DrawAll()
        {
            foreach (var manager in _managers)
            {
                manager.DrawAll();
            }
        }

        public void UpdateAll()
        {
            foreach (var manager in _managers)
            {
                manager.UpdateAll();
            }
        }
    }
}