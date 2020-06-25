namespace FunctionApp1
{
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Azure.WebJobs;
    using Microsoft.Azure.WebJobs.Extensions.Http;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Logging;
    using Newtonsoft.Json;
    using Microsoft.Azure.Cosmos;

    public static class TaskDel
    {
        [FunctionName("TaskDel")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "delete", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            // delete で送られたデータを受け取る
            string id = req.Form["id"];
            string context = req.Form["context"];

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

            // 削除するときもパーティションキーが必要っぽい。コレクションはパーティションを跨いで保存されるからかもしれない
            var item = await container.DeleteItemAsync<ToDoItem>(id, new PartitionKey(context));

            return new OkObjectResult("OK");
        }
    }
}
