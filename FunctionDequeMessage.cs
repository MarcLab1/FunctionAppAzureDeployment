using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace FunctionAppAzureDeployment
{
    public static class FunctionDequeMessage
    {
        [FunctionName("FunctionDequeMessage")]
        public static void Run([QueueTrigger("queue1", Connection = "AzureWebJobsStorageCloud")]string myQueueItem,
            [Queue("queueCopy", Connection = "AzureWebJobsStorageCloud")] ICollector<string> outQueue,
            ILogger log)
        {
            //this will fire for each myQueueItem in the queue
            log.LogInformation($"C# Queue trigger function processed: {myQueueItem}");

            //pass the message to a new Queue because I feel like it
            outQueue.Add(myQueueItem);
        }
    }
}
