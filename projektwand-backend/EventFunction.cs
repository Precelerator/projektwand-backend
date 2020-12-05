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
    public static class GetEvents
    {
        [FunctionName("GetEvents")]
        public static async Task<IActionResult> RunGetEvents(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            Response response = await FetchAllRowsAsync();

            List<Event> events = ParseToEvents(response);

            return new OkObjectResult(events);
        }

        [FunctionName("GetEvent")]
        public static async Task<IActionResult> RunGetEvent(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");
            var id = Convert.ToInt32(req.Query["id"]);
            Response response = await FetchRowAsync(id);

            List<Event> events = ParseToEvents(response);

            return new OkObjectResult(events.First());
        }

        private static async Task<Response> FetchAllRowsAsync()
        {
            var apiKey = Environment.GetEnvironmentVariable("API_KEY");
            var sheetId = Environment.GetEnvironmentVariable("EVENT_SHEET_ID");
            var client = new RestClient("https://sheets.googleapis.com/v4");

            var request = new RestRequest($"spreadsheets/{sheetId}/values/Events!A2:I50?key={apiKey}", DataFormat.Json);

            var response = await client.GetAsync<Response>(request);

            return response;
        }

        private static async Task<Response> FetchRowAsync(int id)
        {
            var apiKey = Environment.GetEnvironmentVariable("API_KEY");
            var sheetId = Environment.GetEnvironmentVariable("EVENT_SHEET_ID");
            var client = new RestClient("https://sheets.googleapis.com/v4");

            // Google Sheet Daten beginnen ab Zeile 2
            var rowNumber = id + 1;
            var request = new RestRequest($"spreadsheets/{sheetId}/values/Projekte!A{rowNumber}:I{rowNumber}?key={apiKey}", DataFormat.Json);

            var response = await client.GetAsync<Response>(request);

            return response;
        }

        private static List<Event> ParseToEvents(Response response)
        {
            List<Event> events = new List<Event>();
            foreach (var eventValue in response.values)
            {
                if (eventValue.Count != 5) continue;
                Event p = new Event();
                p.datum = eventValue[0];
                p.uhrzeit = eventValue[1];
                p.titel = eventValue[2];
                p.beschreibung = eventValue[3];
                p.zoomLink = eventValue[4];

                events.Add(p);
            }

            return events;
        }
    }
}
