using System;
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
    public class DeleteServices
    {
        private readonly IMapper _mapper;
        private readonly FetchService _fetchServices;
        public DeleteServices(CosmosClient client)
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
    }
}

