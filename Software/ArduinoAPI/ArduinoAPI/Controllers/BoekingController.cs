using ArduinoAPI.Data;
using ArduinoAPI.Service;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using io = System.IO;

namespace ArduinoAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BoekingController : ControllerBase
    {
        private readonly IMemoryStorage memoryStorage;

        private readonly string TRAVEL_PREFIX = "travel_log_";

        public BoekingController(IMemoryStorage storage)
        {
            memoryStorage = storage;

#if DEBUG
            memoryStorage.AddItem("123", new CustomerInfo {
                Name = "Boris",
                Surname = "Mulder",
                CardType = "Retour",
                EndLocation = 2,
                StartLocation = 1
            });

            memoryStorage.AddItem($"{TRAVEL_PREFIX}123", new TravelInfo { Id = "123", CurrentLocation = 1 });
#endif
        }

        [HttpGet("reis/status/{id}")]
        public async  Task<IActionResult> Status(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return new ContentResult
                {
                    ContentType = "txt/html",
                    StatusCode = (int)HttpStatusCode.NotFound,
                    Content = ""
                };
            }

            var info = memoryStorage.GetItem<CustomerInfo>(id);
            var log = memoryStorage.GetItem<TravelInfo>($"{TRAVEL_PREFIX}{id}");

#if DEBUG
            var locationUrl = $"https://localhost:44352/api/boeking/reis/locatie/{id}";
#else
            var locationUrl = $"https://monorail.code:5000/api/boeking/reis/locatie/{id}";
#endif

            // This can be done beter but not going to even try with the time we have left......
            var bindings = new Dictionary<string, object>() {
                {"[PAGE_TITLE]", info.Surname },
                {"[LOCATION]", log.CurrentLocation },
                {"[API_LOCATION_URL]",locationUrl },
            };

            var page = await io.File.ReadAllTextAsync("Page/api-tracking.html");

            foreach (var pair in bindings)
            {
                page = page.Replace(pair.Key, pair.Value.ToString());
            }

            return Content(page, "text/html", Encoding.UTF8);
        }

        [HttpPost("reserveren")]
        public IActionResult PostInfo([FromForm] CustomerInfo customerInfo)
        {
            var userID = GenID(max: 8);

            memoryStorage.AddItem($"{TRAVEL_PREFIX}{userID}", new TravelInfo { Id = userID, CurrentLocation = customerInfo.StartLocation });
            memoryStorage.AddItem(userID, customerInfo);

            return RedirectToAction("Confirm", new { id = userID });
        }


        // TODO met ruben even kijken
        // Hij moet bij elke peron een update sturen
        [HttpGet("reis/update/locatie/{id}/{nloc}")]
        public void UpdateTravel(string id, int? nloc)
        {
            var log = memoryStorage.GetItem<TravelInfo>($"{TRAVEL_PREFIX}{id}");

            if (log == null) return;

            log.CurrentLocation = nloc.GetValueOrDefault();

            memoryStorage.UpdateItem($"{TRAVEL_PREFIX}{id}", log);
        }

        [HttpGet("reis/log")]
        public object GetTravelLog(string id)
        {
            var log = memoryStorage.GetItem<TravelInfo>($"{TRAVEL_PREFIX}{id}");

            if (log == null) return "nan";

            return log;
        }

        [HttpGet("reis/alles")]
        public object GetAllTravelLogs()
        {
            return memoryStorage.GetAllItems<TravelInfo>();
        }

        [HttpGet("reserveringen/alles")]
        public object GetAllReservations()
        {
            return memoryStorage.GetAllItems<CustomerInfo>();
        }

        [HttpGet("reis/locatie/{id}")]
        public int GetTravelLogLocation(string id)
        {
            var log = memoryStorage.GetItem<TravelInfo>($"{TRAVEL_PREFIX}{id}");

            if (log == null) return 0;

            return log.CurrentLocation;
        }

        [HttpGet("status/{id}")]
        [Produces("text/html")]
        public async Task<IActionResult> Confirm(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return new ContentResult {
                    ContentType = "txt/html",
                    StatusCode = (int) HttpStatusCode.NotFound,
                    Content = ""
                };
            }
                

            var info = memoryStorage.GetItem<CustomerInfo>(id);

#if DEBUG
            var statusUrl = $"https://localhost:44352/api/boeking/reis/status/{id}";
#else
            var statusUrl = $"https://monorail.code:5000/api/boeking/reis/status/{id}";
#endif

            // This can be done beter but not going to even try with the time we have left......
            var bindings = new Dictionary<string, object>() {
                {"[PAGE_TITLE]", info.Surname },
                {"[NAME]", info.Name },
                {"[SURNAME]", info.Surname },
                {"[CARD_TYPE]", info.CardType },
                {"[START_LOCATION]", info.StartLocation },
                {"[END_LOCATION]", info.EndLocation },
                {"[ID]", id },
                {"[TRAVEL_TRACKER_URL]",statusUrl },
            };

            var page = await io.File.ReadAllTextAsync("Page/api-booking.html");
            
            foreach (var pair in bindings)
            {
                page = page.Replace(pair.Key, pair.Value.ToString());
            }

            return Content(page, "text/html", Encoding.UTF8);
        }

        private string GenID(int min = 0, int max = 16)
        {
            return Guid.NewGuid().ToString().Substring(min, max).Replace("-", "");
        }
    }
}
