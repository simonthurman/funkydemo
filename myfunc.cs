using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Data.SqlClient;

namespace myfunction
{
    public static class myfunc
    {
        [FunctionName("myfunc")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)]HttpRequest req, ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string skuid = req.Query["skuid"];
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            skuid = skuid ?? data?.skuid;


            //var str = Environment.GetEnvironmentVariable("sqldb_connection");
            string rst = "";
            var str ="Server=tcp:funkydemoserver.database.windows.net,1433;Initial Catalog=funkdemodb;Persist Security Info=False;User ID=simont;Password=Bmw325Ci;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
            using (SqlConnection conn = new SqlConnection(str))
            {
                conn.Open();
                var query = @"select name from product where sku = 1";
            
                using (SqlCommand cmd = new SqlCommand(query,conn))
                {
                    var row = cmd.ExecuteReader();
                    //if (row.HasRows)
                    //{
                        rst = row.GetString(0);
                        log.LogInformation($"row {row}");
                        log.LogInformation($"result {rst}");
                    //}
                    //return new OkObjectResult(rst);
                }
            }
            return new OkObjectResult(rst);
        }

    }
}
