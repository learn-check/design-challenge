using ArduinoAPI.Data;
using ArduinoAPI.Service;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Net;

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
        public IActionResult PostInfo([FromForm] CustomerInfo customerInfo)
        {
            var userID = GenID(max: 8);

            memoryStorage.AddItem(userID, customerInfo);

            return RedirectToActionPermanent("status", "Boeking", new { id = userID});
        }

        private string GenID(int min = 0, int max = 16)
        {
            return Guid.NewGuid().ToString().Substring(min, max).Replace("-", "");
        }
    }
}
