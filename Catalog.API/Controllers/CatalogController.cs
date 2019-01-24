using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Catalog.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Catalog.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CatalogController : ControllerBase
    {
        private readonly EnvironmentConfiguration _configuration;

        public CatalogController(IOptions<EnvironmentConfiguration> options) => _configuration = options.Value;

        [HttpGet]
        public IActionResult Get()
        {
            using (var connection = new SqlConnection(_configuration.ConnectionString))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "SELECT [c].[Id], [c].[CatalogItemId], [c].[Name], [c].[Price] FROM [dbo].[Catalog] AS [c]";
                    var reader = command.ExecuteReader();
                    var list = new List<CatalogItem>();
                    while (reader.Read())
                    {
                        var item = new CatalogItem
                        {
                            Id = (int)reader["Id"],
                            CatalogItemId = Guid.Parse(reader["CatalogItemId"].ToString()),
                            Name = reader["Name"].ToString(),
                            Price = (decimal)reader["Price"]
                        };
                        list.Add(item);
                    }

                    connection.Close();
                    return Ok(list);
                }
            }
        }

        [HttpPost]
        public IActionResult Post([FromBody] CatalogItem item)
        {
            using (var connection = new SqlConnection(_configuration.ConnectionString))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "INSERT INTO [dbo].[Catalog] (CatalogItemId, Name, Price) VALUES (@CatalogITemId, @Name, @Price)";
                    command.Parameters.Add("@CatalogItemId", System.Data.SqlDbType.UniqueIdentifier).Value = Guid.NewGuid();
                    command.Parameters.Add("@Name", System.Data.SqlDbType.VarChar).Value = item.Name;
                    command.Parameters.Add("@Price", System.Data.SqlDbType.Decimal).Value = item.Price;

                    var result = command.ExecuteNonQuery();
                    connection.Close();
                    return result > -1 ? (IActionResult)Ok() : BadRequest();
                }
            }
        }
    }
}
