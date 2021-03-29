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
using System.Collections.Generic;
using System.Linq;
using BL_Tutorial_Izzat.DAL.Repositories;
using BL_Tutorial_Izzat.DAL.Models;

namespace BL_Tutorial_Izzat.API
{
    public static class Function1
    {
        [FunctionName("GetAllClass")]
        public static async Task<IActionResult> GetAllClass(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
            [CosmosDB(ConnectionStringSetting = "cosmos-bl-tutorial-serverless")] DocumentClient documentClient,
            ILogger log)
        {
            using var classRep = new AccessCosmos.ClassRepository(documentClient);
            var data = await classRep.GetAsync();
            return new OkObjectResult(data.Items);
        }

        [FunctionName("GetClassByIdNexus")]
        public static async Task<IActionResult> GetClassByIdNexus(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "CourseNexus/{id}/")] HttpRequest req,
            [CosmosDB(ConnectionStringSetting = "cosmos-bl-tutorial-serverless")] DocumentClient documentClient,
            string id,
            ILogger log)
        {
            using var classRep = new AccessCosmos.ClassRepository(documentClient);
            var data = await classRep.GetAsync(predicate: p => p.Id == id);
            return new OkObjectResult(data.Items.FirstOrDefault());
        }

        [FunctionName("DeleteDataById")]
        public static async Task<IActionResult> DeleteDataById(
            [HttpTrigger(AuthorizationLevel.Function, "delete", Route = "delete/data/{id}")] HttpRequest req,
            [CosmosDB(ConnectionStringSetting = "cosmos-bl-tutorial-serverless")] DocumentClient documentClient,
            string id,
            ILogger log)
        {
            try
            {
                using var classRep = new AccessCosmos.ClassRepository(documentClient);
                classRep.DeleteAsync(id);
                return new OkResult();
            }
            catch (Exception e)
            {
                return new BadRequestObjectResult($"Bad Request Exception : {e.Message}");
            }
        }

        [FunctionName("CreateData")]
        public static async Task<IActionResult> CreateData(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "create/data")] HttpRequest req,
            [CosmosDB(ConnectionStringSetting = "cosmos-bl-tutorial-serverless")] DocumentClient documentClient,
            ILogger log)
        {
            var content = await new StreamReader(req.Body).ReadToEndAsync();

            DTOClass myClass = JsonConvert.DeserializeObject<DTOClass>(content);
            if (myClass.ClassCode == null || myClass.Description == null)
                return new BadRequestResult();

            var class1 = new DTOClass() { ClassCode = myClass.ClassCode, Description = myClass.Description };

            using var classRep = new AccessCosmos.ClassRepository(documentClient);
            var data = await classRep.CreateAsync(class1);

            return new OkObjectResult(data);
        }

        [FunctionName("UpdateDataById")]
        public static async Task<IActionResult> UpdateDataById(
            [HttpTrigger(AuthorizationLevel.Function, "put", Route = "update/data/{id}")] HttpRequest req,
            [CosmosDB(ConnectionStringSetting = "cosmos-bl-tutorial-serverless")] DocumentClient documentClient,
            string id,
            ILogger log)
        {
            var content = await new StreamReader(req.Body).ReadToEndAsync();

            DTOClass myClass = JsonConvert.DeserializeObject<DTOClass>(content);

            if (myClass.ClassCode == null || myClass.ClassCode == null)
                return new BadRequestObjectResult($"Bad Request Exception data is not complete.");

            try
            {
                var class1 = new DTOClass() { ClassCode = myClass.ClassCode, Description = myClass.Description };

                using var classRep = new AccessCosmos.ClassRepository(documentClient);
                var data = await classRep.UpdateAsync(id, class1);

                return new OkObjectResult(data);
            }
            catch (Exception e)
            {
                return new BadRequestObjectResult(e.Message);
            }
        }
    }
}
