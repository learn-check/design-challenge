using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using io = System.IO.File;

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
    }
}
