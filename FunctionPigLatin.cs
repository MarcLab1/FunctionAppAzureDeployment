using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Text;

namespace FunctionAppAzureDeployment
{
    public static class FunctionPigLatin
    {
        [FunctionName("FunctionPigLatinConverter")]
        public static IActionResult Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            String word = req.Query["word"].ToString().ToLower();
            if (word == null || word.Length == 0)
                return new BadRequestObjectResult("Pass a word in the query string or in the request body.\n" + "Example: &word=dogs");

            return new OkObjectResult($"Word : {word}\n" + $"Word converted to Pig Latin : {convertToPigLatin(word)}");
        }

        private static String convertToPigLatin(String word)
        {
            if (isCharVowel(word[0]))
                return word + "way";

            String cluster = getConstCluster(word);
            return word.Substring(word.IndexOf(cluster) + cluster.Length) + cluster + "ay";
        }
        private static Boolean isCharVowelOrY(char c)
        {
            String vowelsAndY = "aeiouy";
            return vowelsAndY.Contains(c);
        }
        private static Boolean isCharVowel(char c)
        {
            String vowels = "aeiou";
            return vowels.Contains(c);
        }

        private static String getConstCluster(String word)
        {
            StringBuilder cluster = new StringBuilder();
            cluster.Append(word[0]);
            for (int i = 1; i < word.Length; i++)
            {
                if (isCharVowelOrY(word[i]))
                    break;
                else
                    cluster.Append(word[i]);
            }
            return cluster.ToString();
        }
    }
}
