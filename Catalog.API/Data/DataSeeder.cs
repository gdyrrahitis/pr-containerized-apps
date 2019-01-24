using Catalog.API.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Options;
using System;
using System.Data.SqlClient;

namespace Catalog.API.Data
{
    public static class DataSeeder
    {
        public static IApplicationBuilder SeedData(this IApplicationBuilder app)
        {
            var options = app.ApplicationServices.GetService(typeof(IOptions<EnvironmentConfiguration>)) as IOptions<EnvironmentConfiguration>;
            using (var connection = new SqlConnection(options.Value.ConnectionString))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "DROP TABLE [dbo.Catalog]; GO; CREATE TABLE [dbo].[Catalog] " +
                        "(" +
                        "Id int identity(1,1) unique clustered not null, " +
                        "CatalogItemId uniqueidentifier primary key nonclustered not null," +
                        "Name varchar(100) not null," +
                        "Price money not null" +
                        ")";
                    command.ExecuteNonQuery();
                }

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "INSERT INTO [dbo].[Catalog] (CatalogItemId, Name, Price) VALUES (@CatalogITemId, @Name, @Price)";
                    command.Parameters.Add("@CatalogItemId", System.Data.SqlDbType.UniqueIdentifier).Value = Guid.NewGuid();
                    command.Parameters.Add("@Name", System.Data.SqlDbType.VarChar).Value = "Laptop";
                    command.Parameters.Add("@Price", System.Data.SqlDbType.Decimal).Value = 2000;

                    command.ExecuteNonQuery();
                    connection.Close();
                }
            }
            return app;
        }
    }
}
