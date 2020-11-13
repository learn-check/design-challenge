using System.Collections.Generic;
using ArduinoPanel.data;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System;

namespace ArduinoPanel
{
    public class ApiHandler
    {
        /// <summary>
        /// Client used for getting and posting data from/to the api
        /// </summary>
        private readonly HttpClient client = new HttpClient();

#if DEBUG
        //private readonly string BASE_URL = @"https://localhost:44352/api/boeking/";
        private readonly string BASE_URL = @"http://monorail.codes:5000/api/boeking/";
#else
        private readonly string BASE_URL = @"http://monorail.codes:5000/api/boeking/";
#endif

        /// <summary>
        /// Attempts to get the latest reservations from the api
        /// </summary>
        /// <returns>A list of reservations if success else an empty list on fail</returns>
        public async Task<List<CustomerInfo>> GetAllReservations()
        {
            try
            {
                var response = await client.GetAsync($"{BASE_URL}reserveringen/alles");
                return JsonConvert.DeserializeObject<List<CustomerInfo>>(await response.Content.ReadAsStringAsync());
            }
            catch (Exception e) // Do i really care?, no
            {
                return new List<CustomerInfo>();
            }
        }

        /// <summary>
        /// Sends an location update request for the given customer
        /// </summary>
        /// <param name="customer">The given customer</param>
        /// <param name="location">Thier new location</param>
        /// <returns></returns>
        public async Task UpdateTravelLocation(CustomerInfo customer, int location)
        {
            try
            {
                await client.PostAsync($"{BASE_URL}reis/update/locatie/{customer.Id}/{location}",null);
            }
            catch (Exception e) // Do i really care?, no
            {

            }
        }
    }
}
