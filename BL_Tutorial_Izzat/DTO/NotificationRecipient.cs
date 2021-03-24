using Newtonsoft.Json;
using Nexus.Base.CosmosDBRepository;
using System;

namespace BL_Tutorial_Izzat
{
    public class NotificationRecipient : ModelBase
    {
        [JsonProperty("messageBody")]
        public string MessageBody { get; set; }

        [JsonProperty("dateSaved")]
        public DateTime DateSaved { get; set; }
    }
}
