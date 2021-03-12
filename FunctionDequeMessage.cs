using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace FunctionAppAzureDeployment
{
    public static class FunctionDequeMessage
    {
        [FunctionName("FunctionDequeMessage")]
        public static void Run([QueueTrigger("queue1", Connection = "AzureWebJobsStorageCloud")]string myQueueItem, 
            ILogger log)
        {

            log.LogInformation($"C# Queue trigger function processed: {myQueueItem}");
        }
    }
}
