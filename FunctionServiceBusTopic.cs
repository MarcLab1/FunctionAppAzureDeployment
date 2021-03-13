using System;
using System.Text.Json;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace FunctionAppAzureDeployment
{
    public static class FunctionServiceBusTopic
    {
        [FunctionName("FunctionServiceBusTopic")]
        public static void Run([ServiceBusTrigger("mytopic", "mysub", Connection = "ServiceBusStandardConnection")]string msg, ILogger log)
        {
            Item item1 = JsonSerializer.Deserialize<Item>(msg);

            log.LogInformation($" ID: {item1.id} Name: {item1.name} Email: {item1.email}");
        }
    }
}
