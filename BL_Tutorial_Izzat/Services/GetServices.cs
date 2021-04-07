using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using AutoMapper;
using BL_Tutorial_Izzat.API.DTO;
using BL_Tutorial_Izzat.BLL;
using BL_Tutorial_Izzat.DAL.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using static BL_Tutorial_Izzat.DAL.Repositories.AccessCosmos;

namespace BL_Tutorial_Izzat.API.Services
{
    public class GetServices
    {
        private readonly IMapper _mapper;
        private readonly FetchService _fetchServices;
        public GetServices(CosmosClient client)
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
            }
            catch (Exception e)
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
    }
}

