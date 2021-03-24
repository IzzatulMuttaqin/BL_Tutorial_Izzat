using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.EventHubs;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace BL_Tutorial_Izzat
{
    public static class TestEvhBL
    {
        [FunctionName("TestEvhBL")]
        public static async Task Run([EventHubTrigger("dtoclassrecipient", Connection = "evhBLTutorialConn")] EventData[] events,
            [CosmosDB(ConnectionStringSetting = "cosmos-bl-tutorial-serverless")] DocumentClient documentClient,
            ILogger log)
        {
            var exceptions = new List<Exception>();
            using var classRep = new CosmosDB.AccessCosmos.NotificationRepository(documentClient);

            foreach (EventData eventData in events)
            {
                try
                {
                    string messageBody = Encoding.UTF8.GetString(eventData.Body.Array, eventData.Body.Offset, eventData.Body.Count);

                    var notif = new NotificationRecipient()
                    {
                        MessageBody = messageBody,
                        DateSaved = DateTime.Now
                    };
                    log.LogInformation($"C# Event Hub trigger function processed a message: {messageBody}");
                    await classRep.CreateAsync(notif);
                }
                catch (Exception e)
                {
                    // We need to keep processing the rest of the batch - capture this exception and continue.
                    // Also, consider capturing details of the message that failed processing so it can be processed again later.
                    exceptions.Add(e);
                }
            }

            // Once processing of the batch is complete, if any messages in the batch failed processing throw an exception so that there is a record of the failure.

            if (exceptions.Count > 1)
                throw new AggregateException(exceptions);

            if (exceptions.Count == 1)
                throw exceptions.Single();
        }
    }
}
