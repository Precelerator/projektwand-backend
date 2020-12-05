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
using System.Linq;

namespace projektwand_backend
{
    public static class GetProjects
    {
        [FunctionName("GetProjects")]
        public static async Task<IActionResult> RunGetProjects(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            Response response = await FetchAllRowsAsync();

            List<Project> projects = ParseToProjects(response);

            return new OkObjectResult(projects);
        }

        [FunctionName("GetProject")]
        public static async Task<IActionResult> RunGetProject(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");
            var id = Convert.ToInt32(req.Query["id"]);
            Response response = await FetchRowAsync(id);

            List<Project> projects = ParseToProjects(response);

            return new OkObjectResult(projects.First());
        }

        private static async Task<Response> FetchAllRowsAsync()
        {
            var apiKey = Environment.GetEnvironmentVariable("API_KEY");
            var sheetId = Environment.GetEnvironmentVariable("PROJECT_SHEET_ID");
            var client = new RestClient("https://sheets.googleapis.com/v4");

            var request = new RestRequest($"spreadsheets/{sheetId}/values/Projekte!A2:I50?key={apiKey}", DataFormat.Json);

            var response = await client.GetAsync<Response>(request);

            return response;
        }

        private static async Task<Response> FetchRowAsync(int id)
        {
            var apiKey = Environment.GetEnvironmentVariable("API_KEY");
            var sheetId = Environment.GetEnvironmentVariable("PROJECT_SHEET_ID");
            var client = new RestClient("https://sheets.googleapis.com/v4");

            // Google Sheet Daten beginnen ab Zeile 2
            var rowNumber = id + 1;
            var request = new RestRequest($"spreadsheets/{sheetId}/values/Projekte!A{rowNumber}:I{rowNumber}?key={apiKey}", DataFormat.Json);

            var response = await client.GetAsync<Response>(request);

            return response;
        }

        private static List<Project> ParseToProjects(Response response)
        {
            List<Project> projects = new List<Project>();
            foreach (var projectValue in response.values)
            {
                if (projectValue.Count != 9) continue;
                Project p = new Project();
                p.projektname = projectValue[0];
                p.kurzbeschreibung = projectValue[1];
                p.suchtNach = projectValue[3];
                p.ausfuehrlicheBeschreibung = projectValue[4];
                p.kategorie = projectValue[6];
                p.kurzbeschreibungErsteller = projectValue[7];
                p.onlineSeit = projectValue[8];

                projects.Add(p);
            }

            return projects;
        }
    }
}
