using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.EventGrid.Models;
using Microsoft.Azure.EventHubs;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.EventGrid;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace BL_Tutorial_Izzat
{
    public static class EventTriggerManual
    {
        [FunctionName("PostDataEventGrid")]
        public static async Task<IActionResult> EventToGridRun(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "trigger/event/hub")] HttpRequest req,
            [EventGrid(TopicEndpointUri = "eventGridEndPoint", TopicKeySetting = "eventGridEndKey")]
        IAsyncCollector<EventGridEvent> outputEvents,
            ILogger log)
        {
            try
            {

                var content = await new StreamReader(req.Body).ReadToEndAsync();
                EventGridEvent myClass = JsonConvert.DeserializeObject<EventGridEvent>(content);

                var myEvent = new EventGridEvent()
                {
                    Id = Guid.NewGuid().ToString(),
                    DataVersion = "1.0",
                    EventTime = DateTime.UtcNow,
                    Subject = myClass.Subject,
                    EventType = "Trigger",
                    Data = JsonConvert.SerializeObject(myClass.Data)
                };
                await outputEvents.AddAsync(myEvent);
                return new OkObjectResult(myEvent);
            }
            catch (Exception e)
            {
                return new BadRequestObjectResult($"Error: {e.Message}");
            }

        }

        [FunctionName("TriggerToEventHubTesting")]
        [return: EventHub("dtoclassrecipient", Connection = "evhBLTutorialConn")]
        public static string Run([EventGridTrigger]EventGridEvent eventGridEvent,
           ILogger log)
        {
            log.LogInformation(eventGridEvent.Data.ToString());
            return eventGridEvent.Data.ToString();
        }
    }
}

