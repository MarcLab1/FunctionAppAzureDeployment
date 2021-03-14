using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

namespace FunctionAppAzureDeployment
{
    public static class FunctionHttpCosmosLog
    {
        [FunctionName("FunctionHttpCosmosLog")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log,

             [CosmosDB(
                databaseName: "database437",
                collectionName: "container437",
                ConnectionStringSetting = "DBConnection2",
                CreateIfNotExists = true
            )] IAsyncCollector<InternetItem> items
            )
        {
            string name = req.Query["name"];
            string ipaddress = GetIpFromRequestHeaders(req);
            string date = DateTime.Now.ToString();
            string group = "person"; //logical partition

            InternetItem item = new InternetItem(name, ipaddress,date, group);

            await items.AddAsync(item);

            string responseMessage = $"Hello, {name}. This HTTP triggered function executed successfully.";
            return new OkObjectResult(responseMessage);
        }

        private static string GetIpFromRequestHeaders(HttpRequest request)
        {
            return (request.Headers["X-Forwarded-For"].FirstOrDefault() ?? "").Split(new char[] { ':' }).FirstOrDefault();
        }
    }
}

