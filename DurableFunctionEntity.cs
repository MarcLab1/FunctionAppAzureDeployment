using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace FunctionAppAzureDeployment
{
    public static class DurableFunctionEntity
    {

        private static readonly int MAX_VALUE = 5;

        [FunctionName("DurableFunctionEntity_Orchestrator")]
        public static async Task<List<string>> RunOrchestrator(
            [OrchestrationTrigger] IDurableOrchestrationContext context)
        {
            var outputs = new List<string>();
            var entityId = new EntityId(nameof(CounterClass), "HelloCounter");

            for (int i = 1; i <= 3; i++)
            {
                outputs.Add(await context.CallActivityAsync<string>("DurableFunctionEntity_Activity", i));    
            }
            context.SignalEntity(entityId, "Add", 1);
            var finalValue = await context.CallEntityAsync<int>(entityId, "Get");
            outputs.Add($"DurableFunctionEntity was called {finalValue} times");

            if(finalValue >= MAX_VALUE)
            {
                await context.CallEntityAsync<int>(entityId, "Reset");
                outputs.Add($"Counter has been reset to zero");
            }
            return outputs;
        }

        [FunctionName("DurableFunctionEntity_Activity")]
        public static string PrintCounter([ActivityTrigger] int number, ILogger log)
        {
            return $"Counter = {number}";
        }
        
        //Client
        [FunctionName("DurableFunctionEntity_HttpStart")]
        public static async Task<HttpResponseMessage> HttpStart(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequestMessage req,
            [DurableClient] IDurableOrchestrationClient starter,
            ILogger log)
        {   
            string instanceId = await starter.StartNewAsync("DurableFunctionEntity_Orchestrator", null);
            log.LogInformation($"Started orchestration with ID = '{instanceId}'.");
            return starter.CreateCheckStatusResponse(req, instanceId);
        }
    }

    //Class Entity
    public class CounterClass
    {

        [JsonProperty("value")]
        public int CurrentValue { get; set; }
        public void Add(int amount)
        {
            this.CurrentValue += amount;
        }
        public void Reset() => this.CurrentValue = 0;
        public int Get() => this.CurrentValue;
        [FunctionName(nameof(CounterClass))]
        public static Task Run([EntityTrigger] IDurableEntityContext ctx)
          => ctx.DispatchAsync<CounterClass>();
    }
}