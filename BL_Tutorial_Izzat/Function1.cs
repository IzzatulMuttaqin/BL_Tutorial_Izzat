using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents;

namespace BL_Tutorial_Izzat
{
    public static class Function1
    {
        [FunctionName("GetAllClass")]
        public static async Task<IActionResult> GetAllClass(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
            [CosmosDB(ConnectionStringSetting = "cosmos-bl-tutorial-serverless")] DocumentClient documentClient,
            ILogger log)
        {
            var query = new SqlQuerySpec("SELECT * FROM c");
            var pk = new PartitionKey("Data2");
            var options = new FeedOptions() { PartitionKey = pk };
            var data = documentClient.CreateDocumentQuery(UriFactory.CreateDocumentCollectionUri("Data2", "Course2"), query, options);
            return new OkObjectResult(data);
        }

        [FunctionName("DeleteDataById")]
        public static async Task<IActionResult> DeleteDataById(
            [HttpTrigger(AuthorizationLevel.Function, "delete", Route = "delete/data/{id}")] HttpRequest req,
            [CosmosDB(
                databaseName: "Data2",
                collectionName: "Course2",
                ConnectionStringSetting = "cosmos-bl-tutorial-serverless",
                Id = "{id}",
                PartitionKey = "Data2")] Document document,
            [CosmosDB(ConnectionStringSetting = "cosmos-bl-tutorial-serverless")] DocumentClient documentClient,
            ILogger log)
        {
            if (document == null)
                return new BadRequestResult();

            log.LogInformation(document.ToString());
            await documentClient.DeleteDocumentAsync(document.SelfLink, new RequestOptions() { PartitionKey = new PartitionKey("Data2") });

            return new OkResult();
        }

        [FunctionName("CreateData")]
        public static async Task<IActionResult> CreateData(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "create/data")] HttpRequest req,
            [CosmosDB(ConnectionStringSetting = "cosmos-bl-tutorial-serverless")] DocumentClient documentClient,
            ILogger log)
        {
            var content = await new StreamReader(req.Body).ReadToEndAsync();

            DTOClass myClass = JsonConvert.DeserializeObject<DTOClass>(content);
            log.LogInformation("Object");
            if (myClass.ClassCode == null || myClass.ClassCode == null)
                return new BadRequestResult();

            var book = new DTOClass
            {
                ClassCode = myClass.ClassCode,
                Description = myClass.Description,
                PartitionKey = "Data2"
            };
            await documentClient.CreateDocumentAsync(UriFactory.CreateDocumentCollectionUri("Data2", "Course2"), book);

            return new OkResult();
        }

        [FunctionName("UpdateDataById")]
        public static async Task<IActionResult> UpdateDataById(
            [HttpTrigger(AuthorizationLevel.Function, "put", Route = "update/data/{id}")] HttpRequest req,
            [CosmosDB(
                databaseName: "Data2",
                collectionName: "Course2",
                ConnectionStringSetting = "cosmos-bl-tutorial-serverless",
                Id = "{id}",
                PartitionKey = "Data2")] Document document,
            [CosmosDB(ConnectionStringSetting = "cosmos-bl-tutorial-serverless")] DocumentClient documentClient,
            string id,
            ILogger log)
        {
            var content = await new StreamReader(req.Body).ReadToEndAsync();

            DTOClass myClass = JsonConvert.DeserializeObject<DTOClass>(content);

            if (document == null || myClass.ClassCode == null || myClass.ClassCode == null)
                return new BadRequestResult();

            var book = new DTOClass
            {
                Id = id,
                ClassCode = myClass.ClassCode,
                Description = myClass.Description,
                PartitionKey = "Data2"
            };

            await documentClient.DeleteDocumentAsync(document.SelfLink, new RequestOptions() { PartitionKey = new PartitionKey("Data2") });
            await documentClient.CreateDocumentAsync(UriFactory.CreateDocumentCollectionUri("Data2", "Course2"), book);

            return new OkResult();
        }
    }
}
