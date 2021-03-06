namespace backend.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;
    using Microsoft.Azure.Cosmos;
    using Microsoft.AspNetCore.Authorization;
    using Models;

    [ApiController]
    [Route("api/[controller]")]
    //    [Authorize]
    [Authorize("read:messages")]
    public class ToDoController : ControllerBase
    {
        private readonly ILogger<ToDoController> _logger;

        private readonly string _endpointUri;

        private readonly string _primaryKey;

        public ToDoController(ILogger<ToDoController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _endpointUri = configuration["CosmosDb:EndpointUri"];
            _primaryKey = configuration["CosmosDb:PrimaryKey"];
        }

        [HttpGet]
        public async IAsyncEnumerable<ToDoItem> Get(string email)
        {
            // ユーザー情報が入っていないjwtなので欲しい場合は、auth0に問い合わせる必要があるかも。
            // それかユーザー情報入りのjwtにするか、別個引数でユーザー情報を送ってもらう必要があるかも

            using var cosmosClient = new CosmosClient(this._endpointUri, this._primaryKey, new CosmosClientOptions()
            { ApplicationName = "CosmosDBDotnetQuickstart" });

            // データベースはそのまま
            var databaseId = "ToDoList";
            var databaseResponse = await cosmosClient.CreateDatabaseIfNotExistsAsync(databaseId);
            var database = databaseResponse.Database;

            // コンテナはテーブルにあたる（パーティションキー毎に適切にデータが配置されるのである程度分散される値が良いかも）
            var containerId = "NewItems";
            var containerResponse = await database.CreateContainerIfNotExistsAsync(containerId, "/email", 400);
            var container = containerResponse.Container;

            // インジェクションが可能になる可能性があるためこの方法はよくないかも
            var sqlQueryText = $"SELECT * FROM c WHERE c.email = '{email}'";
            var queryDefinition = new QueryDefinition(sqlQueryText);
            var queryResultSetIterator = container.GetItemQueryIterator<ToDoItem>(queryDefinition);

            var data = new List<ToDoItem>();
            while (queryResultSetIterator.HasMoreResults)
            {
                var currentResultSet = await queryResultSetIterator.ReadNextAsync();
                foreach (var item in currentResultSet)
                {
                    yield return item;
                }
            }
        }

        [HttpPost]
        public async Task<ActionResult<ToDoItem>> Post([FromForm] string context, [FromForm] string email)
        {
            using var cosmosClient = new CosmosClient(this._endpointUri, this._primaryKey, new CosmosClientOptions()
            { ApplicationName = "CosmosDBDotnetQuickstart" });

            // データベースはそのまま
            var databaseId = "ToDoList";
            var databaseResponse = await cosmosClient.CreateDatabaseIfNotExistsAsync(databaseId);
            var database = databaseResponse.Database;

            // コンテナはテーブルにあたる（パーティションキー毎に適切にデータが配置されるのである程度分散される値が良いかも）
            var containerId = "NewItems";
            var containerResponse = await database.CreateContainerIfNotExistsAsync(containerId, "/email", 400);
            var container = containerResponse.Container;

            // Status に enum を使ってうまく表現したほうが良いかも
            var data = new ToDoItem
            {
                Id = Guid.NewGuid(),
                Context = context,
                EMail = email,
                Status = "New",
                CreateAt = DateTime.Now
            };

            var item = await container.CreateItemAsync<ToDoItem>(data, new PartitionKey(data.EMail));

            return this.CreatedAtAction("Get", new { id = data.Id, email = email }, data);
        }

        [HttpPut("{id:Guid}")]
        public async Task<IActionResult> Put(Guid id, [FromForm] string context, [FromForm] string email, [FromForm] string status)
        {
            using var cosmosClient = new CosmosClient(this._endpointUri, this._primaryKey, new CosmosClientOptions()
            { ApplicationName = "CosmosDBDotnetQuickstart" });

            // データベースはそのまま
            var databaseId = "ToDoList";
            var databaseResponse = await cosmosClient.CreateDatabaseIfNotExistsAsync(databaseId);
            var database = databaseResponse.Database;

            // コンテナはテーブルにあたる（パーティションキー毎に適切にデータが配置されるのである程度分散される値が良いかも）
            var containerId = "NewItems";
            var containerResponse = await database.CreateContainerIfNotExistsAsync(containerId, "/email", 400);
            var container = containerResponse.Container;

            // データを取得
            var itemResponse = await container.ReadItemAsync<ToDoItem>(id.ToString(), new PartitionKey(email));
            var data = itemResponse.Resource;

            data.Context = context;
            data.Status = status;
            var item = await container.UpsertItemAsync<ToDoItem>(data, new PartitionKey(data.EMail));

            // REST的には削除したデータを返すのが正しい
            return this.NoContent();
        }

        [HttpDelete("{id:Guid}")]
        public async Task<IActionResult> Delete(Guid id, [FromForm] string email)
        {
            using var cosmosClient = new CosmosClient(this._endpointUri, this._primaryKey, new CosmosClientOptions()
            { ApplicationName = "CosmosDBDotnetQuickstart" });

            // データベースはそのまま
            var databaseId = "ToDoList";
            var databaseResponse = await cosmosClient.CreateDatabaseIfNotExistsAsync(databaseId);
            var database = databaseResponse.Database;

            // コンテナはテーブルにあたる（パーティションキー毎に適切にデータが配置されるのである程度分散される値が良いかも）
            var containerId = "NewItems";
            var containerResponse = await database.CreateContainerIfNotExistsAsync(containerId, "/email", 400);
            var container = containerResponse.Container;

            // 削除するときもパーティションキーが必要っぽい。コレクションはパーティションを跨いで保存されるからかもしれない
            var item = await container.DeleteItemAsync<ToDoItem>(id.ToString(), new PartitionKey(email));

            // REST的には削除したデータを返すのが正しい
            return this.NoContent();
        }
    }
}
