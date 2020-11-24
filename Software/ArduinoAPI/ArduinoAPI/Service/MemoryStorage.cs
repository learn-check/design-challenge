using System.Collections.Generic;
using System.Linq;

namespace ArduinoAPI.Service
{
    /// <summary>
    /// See <see cref="IMemoryStorage"/>
    /// </summary>
    public class MemoryStorage : IMemoryStorage
    {
        private readonly Dictionary<string, object> Items;

        public MemoryStorage()
        {
            Items = new Dictionary<string, object>();
        }

        public bool DeleteItem(string key)
        {
            return Items.Remove(key);
        }

        public void UpdateItem(string key, object value)
        {
            if (Items.ContainsKey(key))
            {
                Items[key] = value;
            }
        }

        public bool AddItem(string key, object value)
        {
            if (Items.ContainsKey(key) || value == null) return false;

            return Items.TryAdd(key, value);
        }

        public T GetItem<T>(string key)
        {
            if (Items.ContainsKey(key))
                return (T)Items[key];
            else
                return default;
        }

        public List<T> GetAllItems<T>()
        {
            var items = new List<T>();

            foreach (var item in Items.Values)
            {
                if (item.GetType().Equals(typeof(T)))
                    items.Add((T)item);
            }

            return items;
        }
    }
}
