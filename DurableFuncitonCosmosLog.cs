using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

namespace FunctionAppAzureDeployment
{
    public static class DurableFuncitonCosmosLog
    {
        [FunctionName("DurableFuncitonCosmosLog_Orchestrator")]
        public static async Task<List<string>> RunOrchestrator(
            [OrchestrationTrigger] IDurableOrchestrationContext context)      
        {
            var outputs = new List<string>();
            StudentList list = context.GetInput<StudentList>();
            
            for (int i = 0; i < list.students.Count; i++)
            {
                outputs.Add(await context.CallActivityAsync<string>("DurableFuncitonCosmosLog_Activity", list.students[i]));
            }
            return outputs;
        }

        [FunctionName("DurableFuncitonCosmosLog_Activity")]
        public static string printSomeStuff([ActivityTrigger] Student sp, ILogger log) 
        {
            //log.LogInformation($">>>>>>Student Name = {sp.name}");
            return $"Student Name = {sp.name}";
        }

        //this is the entry client
        [FunctionName("DurableFuncitonCosmosLog_HttpStart")]
        public static async Task<HttpResponseMessage> HttpStart(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = "DurableFuncitonCosmosLog_HttpStart/{number:int?}")] HttpRequestMessage req,
            [DurableClient] IDurableOrchestrationClient starter,
            [CosmosDB(
                databaseName: "database905",
                collectionName: "container905",
                ConnectionStringSetting = "DBConnection2",
                CreateIfNotExists = true
            )] IAsyncCollector<Student> students,
            int? number
            )
        {
            StudentList list = await req.Content.ReadAsAsync<StudentList>();

            if (number == null) 
                number = 5;

            foreach (Student student in list.students)
            {
                student.number = (int)number;
                await students.AddAsync(student); //add to the cosmos db
            }
           
            string instanceId = await starter.StartNewAsync("DurableFuncitonCosmosLog_Orchestrator", list);
            return starter.CreateCheckStatusResponse(req, instanceId);
        }

        /*
        {
"Students": [{
    "number":"1",
    "name":"Teddy"
},

{
    "number":"3",
    "name":"Israel"
}
]
}
        */
    }
}