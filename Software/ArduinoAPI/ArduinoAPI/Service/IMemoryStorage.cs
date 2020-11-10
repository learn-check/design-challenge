using ArduinoAPI.Data;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ArduinoAPI.Service
{
    public interface IMemoryStorage
    {
        public bool AddItem(string name, object value);

        public void UpdateItem(string id, object obj);

        public bool DeleteItem(string name);

        public T GetItem<T>(string name);

        public List<T> GetAllItems<T>();
    }
}
