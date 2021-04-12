using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using System.Text;

namespace FunctionAppAzureDeployment
{
    public class FunctionStateIsFun
    {
        static int counter = 0;

        [FunctionName("FunctionStateIsFun_orch")]
        public static async Task<List<string>> RunOrchestrator(
            [OrchestrationTrigger] IDurableOrchestrationContext context)
        {
            var outputs = new List<string>();

            outputs.Add(await context.CallActivityAsync<string>("FunctionStateIsFun_activitytrigger", 5));
            outputs.Add(await context.CallActivityAsync<string>("FunctionStateIsFun_activitytrigger", 13));
            
            return outputs;
        }

        [FunctionName("FunctionStateIsFun_activitytrigger")]
        public static string printTenTimes([ActivityTrigger] int number, ILogger log)
        {
            int i = 0;
            while (i < number)
            {
                log.LogInformation("count = " + i++ + " : " + "counter = " + counter++);
            }
            string str = "hello world";
            log.LogInformation(doSomethingFun(str.Length, str));
            return counter + "";
        }

        [FunctionName("FunctionStateIsFun_HttpStart")]
        public static async Task<HttpResponseMessage> HttpStart(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequestMessage req,
            [DurableClient] IDurableOrchestrationClient starter,
            ILogger log)
        {
            // Function input comes from the request content.
            string instanceId = await starter.StartNewAsync("FunctionStateIsFun_orch", null);

            return starter.CreateCheckStatusResponse(req, instanceId);
        }

        private static string doSomethingFun(int number, string str)
        {
            StringBuilder sb = new StringBuilder();

            for(int i=0; i<number; i+=1)
            {
                sb.Append(str[i]);
            }

            return sb.ToString();
        }
    }
}