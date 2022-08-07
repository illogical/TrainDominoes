using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Models
{
    public class ObjectGroup<T>
    {
        private Dictionary<string, T> _gameObjects;

        public List<T> Objects
        {
            get
            {
                return new List<T>(_gameObjects.Values);
            }
        }

        public ObjectGroup()
        {
            _gameObjects = new Dictionary<string, T>();
        }

        public ObjectGroup(Dictionary<string, T> objects)
        {
            _gameObjects = objects;
        }

        public int Count
        {
            get
            {
                return _gameObjects.Count;
            }
        }

        public T GetObjectByKey(string key)
        {
            return _gameObjects[key];
        }

        public List<string> GetKeys()
        {
            return new List<string>(_gameObjects.Keys);
        }

        public void Add(string id, T obj)
        {
            _gameObjects.Add(id, obj);
        }

        public void Remove(string key)
        {
            _gameObjects.Remove(key);
        }
    }
}
