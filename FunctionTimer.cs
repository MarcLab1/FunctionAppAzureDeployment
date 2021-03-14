using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace FunctionAppAzureDeployment
{
    public static class FunctionTimer
    {
        [FunctionName("FunctionTimer")]
        public static void Run([TimerTrigger("0 0 0 1 1 *")]TimerInfo myTimer, ILogger log)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");

            //https://www.freeformatter.com/cron-expression-generator-quartz.html

            /*
           {second} {minute} {hour} {day} {month} {day of the week}
            */

            // * == every value
            //     ,  == multiple values
            //  */x == every x intervals
            //  - == range   
            //  L  == "Last" allowed for day of the week and day of the moth fields
            //  W == "weekday" allowed for day of hte week
            //   # == allowed for the day of the month field to specify the #th day of the month

            //every second -->  * * * * * * 
            //every minute -->  0 * * * * * 
            //every hour (at the 0th minute)  -->  0 0 * * * * 
            //every day --> 0 0 0 * * * 
            //Every 30 minutes during business hours -->  0 */30 9-17 ? * 0-4
            //Every 30 minutes during business hours -->  0 */30 9-17 ? * MON-FRI
            //Every minute on 11-11  -->  0 * * 11 11 ?
            //Every 30 seconds on sundays  -->  */30 * * ? * SUN
            //Every month on every Monday, at noon  -->  0 0 12 ? * MON
            //Happy new years! -->  0 0 0 1 1 ?
            //Every month on the third Thursday of the Month, at noon - 12pm  -->  0 0 12 ? * 5#3 	
        }

    }
}
