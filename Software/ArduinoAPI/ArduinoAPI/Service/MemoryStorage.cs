using System.Collections.Generic;
using System.Linq;

namespace ArduinoAPI.Service
{
    public class MemoryStorage : IMemoryStorage
    {
        private readonly Dictionary<string, object> Items;

        public MemoryStorage()
        {
            Items = new Dictionary<string, object>();
        }

        public bool DeleteItem(string name)
        {
            return Items.Remove(name);
        }

        public void UpdateItem(string id, object obj)
        {
            if (Items.ContainsKey(id))
            {
                Items[id] = obj;
            }
        }

        public bool AddItem(string name, object value)
        {
            if (Items.ContainsKey(name) || value == null) return false;

            return Items.TryAdd(name, value);
        }

        public T GetItem<T>(string name)
        {
            if (Items.ContainsKey(name))
                return (T)Items[name];
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
