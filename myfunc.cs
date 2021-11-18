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


            var str = Environment.GetEnvironmentVariable("sqldb_connection");
            
            string rst = "";
            
            using (SqlConnection conn = new SqlConnection(str))
            {
                conn.Open();
                var query = @"select name from product where sku = 1";
            
                using (SqlCommand cmd = new SqlCommand(query,conn))
                {
                    var reader = cmd.ExecuteReader();
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            var row = reader.GetString(0);
                            log.LogInformation($"row {row}");
                            log.LogInformation($"result {rst}");
                            return new OkObjectResult(row);
                        }
                        string responseMessage = "finished";
                        return new OkObjectResult(responseMessage);
                    }
                    else
                    {
                        string responseMessage = "no results";
                        return new OkObjectResult(responseMessage);
                    }
                    //return new OkObjectResult(rst);
                }
            }
        }

    }
}
