using System.Collections.Generic;

namespace ArduinoAPI.Service
{
    public interface IMemoryStorage
    {
        /// <summary>
        /// Adds an item to the dictionary
        /// </summary>
        /// <param name="key">The given key for the value</param>
        /// <param name="value">The given value</param>
        /// <returns></returns>
        public bool AddItem(string key, object value);

        /// <summary>
        /// Updates the given value
        /// </summary>
        /// <param name="key">The given key for the value</param>
        /// <param name="value">The value</param>
        public void UpdateItem(string key, object value);

        /// <summary>
        /// Delete's a given value using its key
        /// </summary>
        /// <param name="key">Key for the given value</param>
        /// <returns></returns>
        public bool DeleteItem(string key);

        /// <summary>
        /// Gets the given value casted as T
        /// </summary>
        /// <typeparam name="T">T can be anything but the value must be able to be casted to T</typeparam>
        /// <param name="key">Key for the given value</param>
        /// <returns>The value casted as T</returns>
        public T GetItem<T>(string key);

        /// <summary>
        /// Gets a list of values casted as T
        /// </summary>
        /// <typeparam name="T">T can be anything but the value must be able to be casted to T</typeparam>
        /// <returns>A List of values casted as T</returns>
        public List<T> GetAllItems<T>();
    }
}
