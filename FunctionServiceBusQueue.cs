using System.Text.Json;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace FunctionAppAzureDeployment
{
    public static class FunctionServiceBusQueue
    {
        [FunctionName("FunctionServiceBusQueue")]
        public static void Run([ServiceBusTrigger("inqueue", Connection = "ServiceBusConnection")]string inqueue, 
            [ServiceBus("outqueue", Connection ="ServiceBusConnection")] ICollector<Item> outqueue,
            ILogger log)
        {
            Item item1 = JsonSerializer.Deserialize<Item>(inqueue);
            Item item2 = new Item("1212", "Marc", "marc@smellycat.com");
            
            outqueue.Add(item1);
            outqueue.Add(item2);

            //{
            //    "id": "1212",
	           // "name": "joe",
	           // "email": "joe@email.com"
            //}
        }
    }
}
