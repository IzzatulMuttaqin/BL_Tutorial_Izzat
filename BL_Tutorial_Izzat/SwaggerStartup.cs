using Microsoft.Azure.WebJobs.Hosting;
using Microsoft.Azure.WebJobs;
using AzureFunctions.Extensions.Swashbuckle;
using System.Reflection;

[assembly: WebJobsStartup(typeof(BL_Tutorial_Izzat.API.SwaggerStartup))]
namespace BL_Tutorial_Izzat.API
{
    public class SwaggerStartup : IWebJobsStartup
    {
        public void Configure(IWebJobsBuilder builder)
        {
            builder.AddSwashBuckle(Assembly.GetExecutingAssembly());
        }
    }
}
