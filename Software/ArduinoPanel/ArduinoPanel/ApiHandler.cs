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
        private readonly HttpClient client = new HttpClient();
#if DEBUG
        private readonly string BASE_URL = @"https://localhost:44352/api/boeking/";
#else
        private readonly string BASE_URL = @"http://monorail.codes:5000/api/boeking/";
#endif

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
    }
}
