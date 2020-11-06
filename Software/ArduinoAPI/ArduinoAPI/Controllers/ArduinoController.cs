using ArduinoAPI.Data;
using ArduinoAPI.Service;
using Microsoft.AspNetCore.Mvc;
using System;

namespace ArduinoAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ArduinoController : ControllerBase
    {
        private readonly IMemoryStorage memoryStorage;
        public ArduinoController(IMemoryStorage storage)
        {
            memoryStorage = storage;
        }

        [HttpPost("post")]
        public string PostInfo([FromForm] CustomerInfo customerInfo)
        {
            var id = Guid.NewGuid().ToString().Replace("-", "");
            memoryStorage.AddItem(id, customerInfo);
            return id;
        }

        [HttpPost("update")]
        public string UpdateInfo([FromBody] UpdateInfo updateInfo)
        {
            return null;
        }

        [HttpGet("get/{id}")]
        public object GetInfo(string id)
        {
            return memoryStorage.GetItem(id).ToString();
        }
    }
}
