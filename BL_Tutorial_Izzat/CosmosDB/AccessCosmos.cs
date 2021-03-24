
using Microsoft.Azure.Documents.Client;
using Nexus.Base.CosmosDBRepository;
using System;
using System.Collections.Generic;
using System.Text;

namespace BL_Tutorial_Izzat.CosmosDB
{
    public class AccessCosmos
    {
        private static readonly string _eventGridEndPoint = Environment.GetEnvironmentVariable("eventGridEndPoint");
        private static readonly string _eventGridKey = Environment.GetEnvironmentVariable("eventGridEndKey");

        public class ClassRepository : DocumentDBRepository<DTOClass>
        {
            public ClassRepository(DocumentClient client) :
                base("Course", client, createDatabaseIfNotExist: true,
                    eventGridEndPoint: _eventGridEndPoint, eventGridKey: _eventGridKey)
            { }
        }

        public class NotificationRepository : DocumentDBRepository<NotificationRecipient>
        {
            public NotificationRepository(DocumentClient client) :
                base("Course", client, createDatabaseIfNotExist: true)
            { }
        }
    }
}
