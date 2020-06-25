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

    public static class TaskAdd
    {
        [FunctionName("TaskAdd")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            // SPA などのクライアントにソースがばれるものだと動的な認証が必要になるはず。
            // AuthorizationLevel は API を呼び出すときにAPIキーが必要かどうかを指定する
            // Anonymous APIキーは必要ない
            // Function 関数ごとの APIキーが必要
            // Admin マスターAPIキーが必要
            // System 未調査
            // User 未調査

            // Functions の出力の設定でCosmosDB を出力対象としてオブジェクトを返すことで
            // ドキュメントを追加することができる。
            // 普通につなげる方法もできるか調べる必要がある。その場合は接続文字列を隠す必要があるが

            // 状態は持てないのでSQLやCosmosDBを使用する
            log.LogInformation("C# HTTP trigger function processed a request.");

            // post で送られたデータを受け取る
            string context = req.Form["Context"];

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

            var data = new ToDoItem
            {
                Id = Guid.NewGuid(),
                Context = context,
                CreateAt = DateTime.Now
            };

            var item = await container.CreateItemAsync<ToDoItem>(data, new PartitionKey(data.Context));

            return new OkObjectResult("OK");
        }
    }
}
