using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

namespace FunctionAppAzureDeployment
{
    public static class DurableFuncitonChaining
    {
        [FunctionName("DurableFuncitonChaining_Orchestration")]
        public static async Task<List<string>> RunOrchestrator(
            [OrchestrationTrigger] IDurableOrchestrationContext context,
             ILogger log
            )
        {
            var outputs = new List<string>();
            ItemList data = context.GetInput<ItemList>();

            //Fan out
            foreach (var item in data.Items)
            {
                outputs.Add(await context.CallActivityAsync<string>("DurableFuncitonChaining_Activity", item.name));
                outputs.Add(await context.CallActivityAsync<string>("DurableFuncitonChaining_Activity_DirectInput", item.name));
                outputs.Add(await context.CallActivityAsync<string>("DurableFuncitonChaining_Activity_Object_Pass", item));
            }

            log.LogInformation("Context name = " + context.ToString());

            return outputs;
        }

        [FunctionName("DurableFuncitonChaining_Activity")]
        public static string SayHello([ActivityTrigger] string name, ILogger log)
        {
            log.LogInformation($">> ACTIVITY: Saying hello to {name}.");
            return $"Hello {name} from Function DurableFuncitonChaining_Activity";
        }

        [FunctionName("DurableFuncitonChaining_Activity_DirectInput")]
        public static string SayHelloDirectInput([ActivityTrigger] string name, ILogger log)
        {
            log.LogInformation($">> DIRECT_INPUT: Saying hello to {name}.");
            return $"Hello {name} from Function DurableFuncitonChaining_Activity_DirectInput";
        }

        [FunctionName("DurableFuncitonChaining_Activity_Object_Pass")]
        public static string SayHelloObjectPass([ActivityTrigger] Item item, ILogger log)
        {
            log.LogInformation($">> OBJECT_PASS: Saying hello to {item.name}.");
            return $"Hello {item.name} from Function DurableFuncitonChaining_Activity_Object_Pass";
        }

        //This is the entry point
        //Client/External Function
        [FunctionName("DurableFuncitonChaining_HttpStart")]
        public static async Task<HttpResponseMessage> HttpStart(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequestMessage req,
            [DurableClient] IDurableOrchestrationClient starter,
            ILogger log)
        {

            var data = await req.Content.ReadAsAsync<ItemList>();

            // Function input comes from the request content.
            string instanceId = await starter.StartNewAsync("DurableFuncitonChaining_Orchestration", data);

            log.LogInformation($"Started orchestration with ID = '{instanceId}'.");

            return starter.CreateCheckStatusResponse(req, instanceId);
        }
    }
}
/*
{
"Items": [{
    "id":"011",
    "name":"Marc",
    "email":"Marcisdumb@gmail.com"
},

{
    "id":"021",
    "name":"Jin",
    "email":"jinsmells@gmail.com"
}
]
}
*/

