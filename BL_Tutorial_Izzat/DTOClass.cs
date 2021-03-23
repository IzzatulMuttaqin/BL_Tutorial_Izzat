using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace BL_Tutorial_Izzat
{
    public class DTOClass
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("classCode")]
        public string ClassCode { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("partitionKey")]
        public string PartitionKey { get; set; }
    }
}
