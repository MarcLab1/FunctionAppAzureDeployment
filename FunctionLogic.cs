using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using FunctionAppAzureDeployment;

namespace FunctionAppLogic
{
    public static class FunctionLogic
    {
        [FunctionName("FunctionLogic")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            Student student;

            student = JsonConvert.DeserializeObject<Student>(requestBody);

            student.number += 10;
            student.name += " the smelly dude";
            student.group = "frog";

            return (ActionResult)new OkObjectResult(student);
        }
    }
}

    /*
     * {
    {
"name":"Marc",
"number": 10,
"group":"person"
}
}
    */

