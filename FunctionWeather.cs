using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net.Http;

namespace FunctionAppAzureDeployment
{
    public static class FunctionWeather
    {
        [FunctionName("FunctionWeather")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            string city = req.Query["city"];
            if (city == null || city.Length == 0)
                city = "Toronto";

            
            string apiKey =  Environment.GetEnvironmentVariable("weatherapi"); 
            string apiCall = "http://api.openweathermap.org/data/2.5/weather?q=" + city + "&appid=" + apiKey;
           

            HttpClient client = new HttpClient();
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, apiCall);
            HttpResponseMessage response = await client.SendAsync(request);
            var jsonResult = await response.Content.ReadAsStringAsync();
            Root root = JsonConvert.DeserializeObject<Root>(jsonResult);
            string result = "City: " + city +"\n" +
                "Temperature: " + getCFromK(root.main.temp) ;

            return new OkObjectResult(result);
        }

        private static String getCFromK(double c)
        {
            return Convert.ToInt32(c - 273.15) + "°C";
        }
    }
}
