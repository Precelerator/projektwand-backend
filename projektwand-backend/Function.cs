using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using RestSharp;
using System.Collections.Generic;
using projektwand_backend.Models;

namespace projektwand_backend
{
    public static class GetProjects
    {
        [FunctionName("GetProjects")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            Response response = await FetchGoogleSheetAsync();

            List<Project> projects = ParseToProjects(response);

            return new OkObjectResult(projects);
        }

        private static async Task<Response> FetchGoogleSheetAsync()
        {
            var apiKey = Environment.GetEnvironmentVariable("API_KEY");
            var sheetId = Environment.GetEnvironmentVariable("SHEET_ID");
            var client = new RestClient("https://sheets.googleapis.com/v4");

            var request = new RestRequest($"spreadsheets/{sheetId}/values/Projekte!A2:H10?key={apiKey}", DataFormat.Json);

            var response = await client.GetAsync<Response>(request);

            return response;
        }

        private static List<Project> ParseToProjects(Response response)
        {
            List<Project> projects = new List<Project>();
            foreach (var projectValue in response.values)
            {
                Project p = new Project();
                p.projektname = projectValue[0];
                p.kurzbeschreibung = projectValue[1];
                p.ausfuehrlicheBeschreibung = projectValue[2];
                p.kurzbeschreibungErsteller = projectValue[4];
                p.kategorie = projectValue[5];
                p.suchtNach = projectValue[6];
                p.onlineSeit = projectValue[7];

                projects.Add(p);
            }

            return projects;
        }
    }
}
