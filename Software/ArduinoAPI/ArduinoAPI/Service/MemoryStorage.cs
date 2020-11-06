using ArduinoAPI.Data;
using System.Collections.Generic;

namespace ArduinoAPI.Service
{
    public class MemoryStorage : IMemoryStorage
    {
        private readonly Dictionary<string, CustomerInfo> Items;

        public MemoryStorage()
        {
            Items = new Dictionary<string, CustomerInfo>();
        }

        public bool DeleteItem(string name)
        {
            return Items.Remove(name);
        }

        public bool AddItem(string name, CustomerInfo value)
        {
            if (Items.ContainsKey(name) || value == null) return false;

            return Items.TryAdd(name, value);
        }

        public CustomerInfo GetItem(string name)
        {
            return Items[name];
        }
    }
}
