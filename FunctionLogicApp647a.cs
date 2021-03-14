using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace FunctionAppAzureDeployment
{
    public static class FunctionLogicApp647a
    {
        [FunctionName("FunctionLogicApp647a")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            [CosmosDB(databaseName: "database647",
                collectionName: "container647a",
                ConnectionStringSetting = "DBConnection2",
                CreateIfNotExists = true
            )] IAsyncCollector<Item> items,
            ILogger log)
        {
            var content = await new StreamReader(req.Body).ReadToEndAsync();

            Item item = JsonConvert.DeserializeObject<Item>(content);
            await items.AddAsync(item);

            return new OkObjectResult(content);
        }
    }
}

