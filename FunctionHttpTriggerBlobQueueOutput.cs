using System;
using System.IO;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Linq;
using Microsoft.Azure.Storage.Blob;

namespace FunctionAppAzureDeployment
{
    public static class FunctionHttpTriggerBlobQueueOutput
    {
        [FunctionName("FunctionHttpTriggerBlobQueueOutput")]
        [return: Queue("queue2", Connection = "AzureWebJobsStorageCloud")] //this also creates the queue container if it does not exist

        public static string QueueOutput(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log,
             [Blob("container001", FileAccess.Write, Connection = "AzureWebJobsStorageCloud")] CloudBlobContainer blobcontainer,
              [Queue("queue1", Connection = "AzureWebJobsStorageCloud")] ICollector<string> outQueue
            )
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string name = req.Query["name"];
            string ipaddress = GetIpFromRequestHeaders(req);
            string date = DateTime.Now.ToString();
            string content = name + "\n" + ipaddress + "\n" + date;

            string blobName = $"{name}.txt";
            CloudBlockBlob blob = blobcontainer.GetBlockBlobReference($"{blobName}");
            blob.UploadText(content);

            string content2 = name + " was here with ip address " + ipaddress + " on " + date;
            outQueue.Add(content2);

            return content2;
        }

        private static string GetIpFromRequestHeaders(HttpRequest request)
        {
            return (request.Headers["X-Forwarded-For"].FirstOrDefault() ?? "").Split(new char[] { ':' }).FirstOrDefault();
        }
    }
}
