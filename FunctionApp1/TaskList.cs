namespace FunctionApp1
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Azure.WebJobs;
    using Microsoft.Azure.WebJobs.Extensions.Http;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Logging;
    using Newtonsoft.Json;
    using Microsoft.Azure.Cosmos;

    public static class TaskList
    {
        [FunctionName("TaskList")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
            ILogger log)
        {
            // 状態は持てないのでSQLやCosmosDBを使用する
            log.LogInformation("C# HTTP trigger function processed a request.");

            var endpointUri = "https://manavucosmos.documents.azure.com:443/";
            var primaryKey = "vpJ2M7ImKuZSnsfdnsIiKpvIHy0b0B3U9xDSyxsik0j3GlE6wnzaXCxHeJRyNHdIkqB3fp8rMKzegWoz1IsKnA==";
            using var cosmosClient = new CosmosClient(endpointUri, primaryKey, new CosmosClientOptions()
            { ApplicationName = "CosmosDBDotnetQuickstart" });

            // データベースはそのまま
            var databaseId = "ToDoList";
            var databaseResponse = await cosmosClient.CreateDatabaseIfNotExistsAsync(databaseId);
            var database = databaseResponse.Database;

            // コンテナはテーブルにあたる（パーティションキー毎に適切にデータが配置されるのである程度分散される値が良いかも）
            var containerId = "NewItems";
            var containerResponse = await database.CreateContainerIfNotExistsAsync(containerId, "/context", 400);
            var container = containerResponse.Container;

            var sqlQueryText = "SELECT * FROM c";
            var queryDefinition = new QueryDefinition(sqlQueryText);
            var queryResultSetIterator = container.GetItemQueryIterator<ToDoItem>(queryDefinition);

            var data = new List<ToDoItem>();
            while (queryResultSetIterator.HasMoreResults)
            {
                var currentResultSet = await queryResultSetIterator.ReadNextAsync();
                foreach (var item in currentResultSet)
                {
                    data.Add(item);
                }
            }

            var responseMessage = JsonConvert.SerializeObject(data);
            return new OkObjectResult(responseMessage);
        }
    }
}
