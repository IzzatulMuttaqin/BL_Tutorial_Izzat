using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace BL_Tutorial_Izzat.API.DTO
{
    public class ClassDTO
    {
        [JsonProperty("classCode")]
        public string ClassCode { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }
    }
}
