using BL_Tutorial_Izzat.DAL.Models;
using Microsoft.Azure.Documents.Client;
using System;
using Microsoft.Azure.Cosmos;
using Nexus.Base.CosmosDBRepository;

namespace BL_Tutorial_Izzat.DAL.Repositories
{
    public class AccessCosmos
    {
        private static readonly string _eventGridEndPoint = Environment.GetEnvironmentVariable("eventGridEndPoint");
        private static readonly string _eventGridKey = Environment.GetEnvironmentVariable("eventGridEndKey");
        private static readonly string _cosmosDBEndpoint = Environment.GetEnvironmentVariable("CosmosDBEndPoint");
        private static readonly string _cosmosDBKey = Environment.GetEnvironmentVariable("CosmosDBKey");
        private static readonly string _DB = "Course";

        public class ClassRepository : DocumentDBRepository<DTOClass>
        {
            public ClassRepository(CosmosClient client) :
                base(databaseId: _DB, client, createDatabaseIfNotExist: true,
                    eventGridEndPoint: _eventGridEndPoint, eventGridKey: _eventGridKey)
            { }
        }

        public class NotificationRepository : DocumentDBRepository<NotificationRecipient>
        {
            public NotificationRepository() :
                base(databaseId: _DB, endPoint: _cosmosDBEndpoint, key: _cosmosDBKey, createDatabaseIfNotExist: true)
            { }
        }
    }
}
