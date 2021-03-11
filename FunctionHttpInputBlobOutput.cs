using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace FunctionAppAzureDeployment
{
    public static class FunctionHttpInputBlobOutput
    {
        [FunctionName("FunctionHttpInputBlobOutput")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            [Blob("container001/log.TXT", FileAccess.Write, Connection = "AzureWebJobsStorageCloud")] Stream outBlob,
            [Blob("container001/log.TXT", FileAccess.Read, Connection = "AzureWebJobsStorageCloud")] Stream inBlob,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string name = req.Query["name"];

            StreamReader reader = new StreamReader(inBlob);
            string oldContent = reader.ReadToEnd();

            byte[] newBuffer = Encoding.UTF8.GetBytes(oldContent + name + "\n");
            outBlob.Write(newBuffer, 0, newBuffer.Length);

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            name = name ?? data?.name;

            string responseMessage = string.IsNullOrEmpty(name)
                ? "This HTTP triggered function executed successfully. Pass a name in the query string or in the request body for a personalized response."
                : $"Hello, {name}. This HTTP triggered function executed successfully.";

            return new OkObjectResult(responseMessage);
        }
    }
}

