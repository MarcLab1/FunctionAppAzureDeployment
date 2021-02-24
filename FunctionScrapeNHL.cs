using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using NHLStats;
using System.Collections.Generic;
using System.Text;

namespace FunctionAppAzureDeployment
{
    public static class FunctionScrapeNHL
    {
        private static String heroTeamName;
        private static String heroTeamAbbr;
        private static String villainTeamName;

        private static readonly String[] NHLteamAbbrs = { "ANA","ARI", "BOS", "BUF","CAR","CGY", "CHI", "COL", "CBJ", "DAL","DET", "EDM", "FLA",
            "LAK","MIN", "MTL", "NSH", "NJD", "NYI","NYR", "OTT","PHI", "PIT","SJS", "STL", "TBL","TOR", "VAN", "VGK","WSH", "WPG"}; //TOR is 26

        private static readonly String[] NHLteamNames = {
            "Anaheim Ducks",
"Arizona Coyotes",
"Boston Bruins",
"Buffalo Sabres",
"Carolina Hurricanes",
"Calgary Flames",
"Chicago Blackhawks",
"Colorado Avalanche",
"Columbus Blue Jackets",
"Dallas Stars",
"Detroit Red Wings",
"Edmonton Oilers",
"Florida Panthers",
"Los Angeles Kings",
"Minnesota Wild",
"Montreal Canadiens",
"Nashville Predators",
"New Jersey Devils",
"New York Islanders",
"New York Rangers",
"Ottawa Senators",
"Philadelphia Flyers",
"Pittsburgh Penguins",
"San Jose Sharks",
"St. Louis Blues",
"Tampa Bay Lightning",
"Toronto Maple Leafs",
"Vancouver Canucks",
"Vegas Golden Knights",
"Washington Capitals",
"Winnipeg Jets" }; //31 total

        [FunctionName("FunctionScrapeNHLDetails")]
        public static IActionResult Run([HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req, ILogger log)
        {
            log.LogInformation("HttpGetDetails function processed a request.");

            StringBuilder result = new StringBuilder();
            int heroTeamIndex = getTeamIndex(req.Query["team"].ToString().ToUpper());
            if (heroTeamIndex == -1)
            {
                result.Append("Pass a 3 character NHL team abbreviaion in the query string or in the request body.\n" +
                    "Example: &team=TOR");
                return new OkObjectResult(result.ToString());
            }
            populateHeroTeam(heroTeamIndex);
            DateTime dateToday = System.DateTime.Today;
            Schedule sched = new Schedule(dateToday.ToString("yyyy-MM-dd"));
            List<Game> games = sched.games;

            result.Append("On " + dateToday.ToString("ddd MMM dd yyyy") + ", " + "the ");

            foreach (Game game in games)
            {
                if (game.homeTeam.TeamAbbreviation.ToString().Equals(heroTeamAbbr))
                {
                    populateVillainTeam(game.awayTeam.TeamAbbreviation);
                    result.Append(heroTeamName +
                        " host the " + villainTeamName);
                    return new OkObjectResult(result.ToString());
                }
                else if (game.awayTeam.TeamAbbreviation.ToString().Equals(heroTeamAbbr))
                {
                    populateVillainTeam(game.homeTeam.TeamAbbreviation);
                    result.Append(heroTeamName +
                         " visit the " + villainTeamName);
                    return new OkObjectResult(result.ToString());
                }
            }
            result.Append(heroTeamName + " have the night off");
            return new OkObjectResult(result.ToString());
        }

        private static void populateHeroTeam(int index)
        {
            heroTeamName = NHLteamNames[index];
            heroTeamAbbr = NHLteamAbbrs[index];
        }

        private static void populateVillainTeam(String teamOpponent)
        {
            villainTeamName = NHLteamNames[getTeamIndex(teamOpponent)];
        }

        private static int getTeamIndex(String teamAbbr)
        {
            return Array.IndexOf(NHLteamAbbrs, teamAbbr);
        }
    }
}
