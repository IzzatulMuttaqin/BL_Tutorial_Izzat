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
using AutoMapper;
using BL_Tutorial_Izzat.API.DTO;

namespace BL_Tutorial_Izzat.API.Services
{
    public class FunctionServices
    {
        private readonly IMapper _mapper;
        private readonly FetchService _fetchServices;
        public FunctionServices(CosmosClient client)
        {
            if (_mapper == null)
            {
                var config = new MapperConfiguration(cfg =>
                {
                    cfg.CreateMap<DTOClass, ClassDTO>();
                });
                _mapper = new Mapper(config);
            }

            _fetchServices = new FetchService(new ClassRepository(client));
        }

        // TODO: implement DI utk nexus 3
        // TODO: tidak pakai DocumentClient documentClient => bukan nexus 2
        // TODO: coba pakai automapper utk map dari class database ke DTO
        // TODO: belum ada swagger request dan response
        [FunctionName("GetAllClass")]
        public async Task<IActionResult> GetAllClass(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "all/")] HttpRequest req,
            ILogger log)
        {
            try
            {
                var data = await _fetchServices.GetAllDtoClass();
                if (data == null) return new NotFoundObjectResult("Data doesn't exist");
                var list = new List<ClassDTO>();
                foreach (var item in data.Items)
                {
                    list.Add(_mapper.Map<ClassDTO>(item));
                }
                return new OkObjectResult(list);
            } catch (Exception e)
            {
                return new BadRequestObjectResult(e.Message);
            }
        }

        [FunctionName("GetClassByIdNexus")]
        public async Task<IActionResult> GetClassByIdNexus(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "course/nexus/{id}/")] HttpRequest req,
            string id,
            ILogger log)
        {
            try
            {
                var data = await _fetchServices.GetDtoClassById(id);
                if (data == null) return new NotFoundObjectResult("Data doesn't exist");
                var mappedData = _mapper.Map<ClassDTO>(data);
                return new OkObjectResult(mappedData);
            }
            catch (Exception e)
            {
                return new BadRequestObjectResult(e.Message);
            }
        }

        [FunctionName("DeleteDataById")]
        public async Task<IActionResult> DeleteDataById(
            [HttpTrigger(AuthorizationLevel.Function, "delete", Route = "delete/data/{id}")] HttpRequest req,
            string id,
            ILogger log)
        {
            try
            {
                var data = await _fetchServices.DeleteDtoClass(id);
                if (data.Contains("fail")) return new BadRequestObjectResult("Data doesn't exist");
                return new OkObjectResult("Delete success");
            }
            catch (Exception e)
            {
                return new BadRequestObjectResult(e.Message);
            }
        }

        [FunctionName("CreateData")]
        // TODO: penamaan routing tidak konsisten: ada yang PascalCase, lowercase, dan null
        public async Task<IActionResult> CreateData(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "create/data")] HttpRequest req,
            ILogger log)
        {            
            try
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
                var data = await _fetchServices.CreateNewDtoClass(body);
                var mappedData = _mapper.Map<ClassDTO>(data);
                return new OkObjectResult(mappedData);
            } catch (Exception e)
            {
                return new BadRequestObjectResult(e.Message);
            }
        }

        [FunctionName("UpdateDataById")]
        public async Task<IActionResult> UpdateDataById(
            [HttpTrigger(AuthorizationLevel.Function, "put", Route = "update/data/{id}")] HttpRequest req,
            string id,
            ILogger log)
        {
            var content = await new StreamReader(req.Body).ReadToEndAsync();

            DTOClass myClass = JsonConvert.DeserializeObject<DTOClass>(content);

            if (myClass.ClassCode == null || myClass.ClassCode == null)
                return new BadRequestObjectResult("Input not complete");

            try
            {
                var dataCurrent = await _fetchServices.GetDtoClassById(id);
                if (dataCurrent == null) return new NotFoundObjectResult("Data doesn't exist");
                dataCurrent.Description = myClass.Description;
                dataCurrent.ClassCode = myClass.ClassCode;
                var data = await _fetchServices.UpdateDtoClas(id, dataCurrent);
                var mappedData = _mapper.Map<ClassDTO>(data);
                return new OkObjectResult(mappedData);
            }
            catch (Exception e)
            {
                return new BadRequestObjectResult(e.Message);
            }
        }
    }
}
