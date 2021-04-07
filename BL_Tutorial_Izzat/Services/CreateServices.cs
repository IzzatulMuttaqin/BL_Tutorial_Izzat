using System;
using System.IO;
using System.Threading.Tasks;
using AutoMapper;
using AzureFunctions.Extensions.Swashbuckle.Attribute;
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
    public class CreateServices
    {
        private readonly IMapper _mapper;
        private readonly FetchService _fetchServices;
        public CreateServices(CosmosClient client)
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

        [FunctionName("CreateData")]
        // TODO: penamaan routing tidak konsisten: ada yang PascalCase, lowercase, dan null
        public async Task<IActionResult> CreateData(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "create/data")] HttpRequest req,
            [RequestBodyType(typeof(DTOClass), "Create request")]
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
            }
            catch (Exception e)
            {
                return new BadRequestObjectResult(e.Message);
            }
        }
    }
}

