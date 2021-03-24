using Newtonsoft.Json;
using Nexus.Base.CosmosDBRepository;

namespace BL_Tutorial_Izzat
{
    public class DTOClass : ModelBase
    {
        [JsonProperty("classCode")]
        public string ClassCode { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }
    }
}
