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
    public class UpdateServices
    {
        private readonly IMapper _mapper;
        private readonly FetchService _fetchServices;
        public UpdateServices(CosmosClient client)
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

