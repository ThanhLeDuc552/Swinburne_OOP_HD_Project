using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Swinburne_OOP_HD
{
    public class ObjectManager<T> : IObjectManagerBase where T : GameObject
    {
        private List<T> _objects;

        public ObjectManager()
        {
            _objects = new List<T>();
        }

        public IReadOnlyList<T> Objects => _objects.AsReadOnly(); // encapsulation

        public void Add(T obj)
        {
            if (obj != null)
            {
                _objects.Add(obj);
            }
        }

        public bool Remove(T obj)
        {
            return _objects.Remove(obj);
        }

        public void Clear()
        {
            _objects.Clear();
        }

        public void DrawAll()
        {
            foreach (T obj in _objects)
            {
                obj.Draw();
            }
        }

        /// <summary>
        /// Gets the count of objects in the manager
        /// </summary>
        public int Count => _objects.Count;

        /// <summary>
        /// Calls Update() on all objects in the manager
        /// </summary>
        public void UpdateAll()
        {
            foreach (T obj in _objects)
            {
                obj.Update();
            }
        }
    }
}
