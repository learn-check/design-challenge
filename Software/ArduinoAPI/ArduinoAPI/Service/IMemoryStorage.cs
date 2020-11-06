using ArduinoAPI.Data;
using System.Collections.Generic;

namespace ArduinoAPI.Service
{
    public interface IMemoryStorage
    {
        public bool AddItem(string name, CustomerInfo value);

        public bool DeleteItem(string name);

        public CustomerInfo GetItem(string name);
    }
}
