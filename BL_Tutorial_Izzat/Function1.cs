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
using BL_Tutorial_Izzat.BLL;
using Microsoft.Azure.Cosmos;
using AzureFunctions.Extensions.Swashbuckle.Attribute;
using static BL_Tutorial_Izzat.DAL.Repositories.AccessCosmos;

namespace BL_Tutorial_Izzat.API
{
    public class Function1
    {
        // TODO: implement DI utk nexus 3
        // TODO: tidak pakai DocumentClient documentClient => bukan nexus 2
        // TODO: coba pakai automapper utk map dari class database ke DTO
        // TODO: belum ada swagger request dan response
        [FunctionName("GetAllClass")]
        public static async Task<IActionResult> GetAllClass(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "all/")] HttpRequest req,
            ILogger log)
        {
            FetchService fetchService = new FetchService(new ClassRepository());
            var data = await fetchService.GetAllDtoClass();
            return new OkObjectResult(data);
        }

        [FunctionName("GetClassByIdNexus")]
        public static async Task<IActionResult> GetClassByIdNexus(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "course/nexus/{id}/")] HttpRequest req,
            string id,
            ILogger log)
        {
            FetchService fetchService = new FetchService(new ClassRepository());
            var data = await fetchService.GetDtoClassById(id);
            return new OkObjectResult(data);
        }

        [FunctionName("DeleteDataById")]
        public static async Task<IActionResult> DeleteDataById(
            [HttpTrigger(AuthorizationLevel.Function, "delete", Route = "delete/data/{id}")] HttpRequest req,
            string id,
            ILogger log)
        {
            FetchService fetchService = new FetchService(new ClassRepository());
            try { fetchService.DeleteDtoClass(id); return new OkResult(); }
            catch { return new BadRequestResult(); }
        }

        [FunctionName("CreateData")]
        // TODO: penamaan routing tidak konsisten: ada yang PascalCase, lowercase, dan null
        public static async Task<IActionResult> CreateData(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "create/data")] HttpRequest req,
            ILogger log)
        {
            var content = await new StreamReader(req.Body).ReadToEndAsync();

            DTOClass myClass = JsonConvert.DeserializeObject<DTOClass>(content);

            if (myClass.ClassCode == null || myClass.ClassCode == null)
                return new BadRequestResult();

            var body = new DTOClass
            {
                ClassCode = myClass.ClassCode,
                Description = myClass.Description,
            };


            FetchService fetchService = new FetchService(new ClassRepository());

            var result = await fetchService.CreateNewDtoClass(body);
            log.LogInformation(result.DocumentNamespace);
            return new OkObjectResult(result);
        }

        [FunctionName("UpdateDataById")]
        public static async Task<IActionResult> UpdateDataById(
            [HttpTrigger(AuthorizationLevel.Function, "put", Route = "update/data/{id}")] HttpRequest req,
            string id,
            ILogger log)
        {
            var content = await new StreamReader(req.Body).ReadToEndAsync();

            DTOClass myClass = JsonConvert.DeserializeObject<DTOClass>(content);

            if (myClass.ClassCode == null || myClass.ClassCode == null)
                return new BadRequestResult();

            var body = new DTOClass
            {
                Id = id,
                ClassCode = myClass.ClassCode,
                Description = myClass.Description,
            };

            FetchService fetchService = new FetchService(new ClassRepository());

            log.LogInformation(body.ClassCode);
            log.LogInformation(id);

            var result = await fetchService.UpdateDtoClas(id, body);
            return new OkObjectResult(result);
        }
    }
}
