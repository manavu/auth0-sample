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

    public static class Users
    {
        [FunctionName("Users")]
        public static IActionResult Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            // 状態は持てないのでSQLやCosmosDBを使用する
            log.LogInformation("C# HTTP trigger function processed a request.");

            var data = new[]
            {
                new { Id = 1, Name = "Test1" },
                new { Id = 2, Name = "Test2" },
            };

            var responseMessage = JsonConvert.SerializeObject(data);

            return new OkObjectResult(responseMessage);
        }
    }
}
