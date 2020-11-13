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
        /// <summary>
        /// Storage service for storing reservation and travel data
        /// </summary>
        private readonly IMemoryStorage memoryStorage;

        /// <summary>
        /// Prefix for the travel log of a user
        /// </summary>
        private readonly string TRAVEL_PREFIX = "travel_log_";

        public BoekingController(IMemoryStorage storage)
        {
            memoryStorage = storage;

#if DEBUG // Dummy data when in debug
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

        /// <summary>
        /// Returns a web page where you can track the current location of the monorail
        /// </summary>
        /// <param name="id">Customer Id</param>
        /// <returns></returns>
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
            var locationUrl = $"http://localhost:44352/api/boeking/reis/locatie/{id}";
#else
            var locationUrl = $"http://monorail.codes:5000/api/boeking/reis/locatie/{id}";
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

        /// <summary>
        /// Endpoint for placing a reservation from our website
        /// </summary>
        /// <param name="customerInfo">Customer info to be stored</param>
        /// <returns>A redirect to the status page where you can see your information</returns>
        [HttpPost("reserveren")]
        public IActionResult PostInfo([FromForm] CustomerInfo customerInfo)
        {
            var userID = GenID(max: 8);

            memoryStorage.AddItem($"{TRAVEL_PREFIX}{userID}", new TravelInfo { Id = userID, CurrentLocation = customerInfo.StartLocation });
            memoryStorage.AddItem(userID, customerInfo);

            return RedirectToAction("Confirm", new { id = userID });
        }

        /// <summary>
        /// Endpoitn for updating a customers location
        /// </summary>
        /// <param name="id">Customers id</param>
        /// <param name="nloc">The new location</param>
        [HttpGet("reis/update/locatie/{id}/{nloc}")]
        public void UpdateTravel(string id, int? nloc)
        {
            var log = memoryStorage.GetItem<TravelInfo>($"{TRAVEL_PREFIX}{id}");

            if (log == null) return;

            log.CurrentLocation = nloc.GetValueOrDefault();

            memoryStorage.UpdateItem($"{TRAVEL_PREFIX}{id}", log);
        }

        /// <summary>
        /// Endpoint for requesting a customers travel log 
        /// </summary>
        /// <param name="id">Customers id</param>
        /// <returns>The customers travel log or nan if it can't be found</returns>
        [HttpGet("reis/log/{id}")]
        public object GetTravelLog(string id)
        {
            var log = memoryStorage.GetItem<TravelInfo>($"{TRAVEL_PREFIX}{id}");

            if (log == null) return "nan";

            return log;
        }

        /// <summary>
        /// </summary>
        /// <returns>A list of all the travel logs</returns>
        [HttpGet("reis/alles")]
        public object GetAllTravelLogs()
        {
            return memoryStorage.GetAllItems<TravelInfo>();
        }

        /// <summary>
        /// </summary>
        /// <returns>A list of all the customers</returns>
        [HttpGet("reserveringen/alles")]
        public object GetAllReservations()
        {
            return memoryStorage.GetAllItems<CustomerInfo>();
        }

        /// <summary>
        /// </summary>
        /// <param name="id">Travel id</param>
        /// <returns>The current location of the customer or 0 if not found</returns>
        [HttpGet("reis/locatie/{id}")]
        public int GetTravelLogLocation(string id)
        {
            var log = memoryStorage.GetItem<TravelInfo>($"{TRAVEL_PREFIX}{id}");

            if (log == null) return 0;

            return log.CurrentLocation;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id">Customer id</param>
        /// <returns>A webpage displaying the customers reservation info</returns>
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
            var statusUrl = $"http://monorail.codes:5000/api/boeking/reis/status/{id}";
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

        /// <summary>
        /// </summary>
        /// <param name="min">Minial size</param>
        /// <param name="max">Maximal size</param>
        /// <returns>A random ID</returns>
        private string GenID(int min = 0, int max = 16)
        {
            return Guid.NewGuid().ToString().Substring(min, max).Replace("-", "");
        }
    }
}
