using Catalog.API.Infrastructure.IntegrationEvents;
using Catalog.API.Models;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace Catalog.API.Infrastructure
{
    public class OrderStatusChangedToAwaitingValidationIntegrationEventHandler : DefaultBasicConsumer
    {
        private readonly IRabbitMqManager _manager;
        private readonly EnvironmentConfiguration _configuration;

        public OrderStatusChangedToAwaitingValidationIntegrationEventHandler(IRabbitMqManager manager,
            IOptions<EnvironmentConfiguration> options)
        {
            _manager = manager;
            _configuration = options.Value;
        }

        public override void HandleBasicDeliver(string consumerTag, ulong deliveryTag, bool redelivered, string exchange, string routingKey,
            IBasicProperties properties, byte[] body)
        {
            if (properties.ContentType != "application/json")
            {
                throw new ArgumentException($"Can't handle content type of {properties.ContentType}");
            }

            var message = Encoding.UTF8.GetString(body);
            var command = JsonConvert.DeserializeObject<OrderStatusChangedToAwaitingValidationIntegrationEvent>(message);
            Consume(command);
            _manager.SendAck(deliveryTag);
        }

        public void Consume(OrderStatusChangedToAwaitingValidationIntegrationEvent command)
        {
            var confirmedOrderStockItems = command.CatalogItems.Select(orderStockItem =>
            {
                var catalogItem = GetCatalogItemById(orderStockItem.ProductId);
                var hasStock = catalogItem.AvailableStock >= orderStockItem.Units;
                var confirmedOrderStockItem = new ConfirmedOrderStockItem
                {
                    HasStock = hasStock,
                    CatalogItemId = catalogItem.CatalogItemId
                };

                return confirmedOrderStockItem;
            });

            if (confirmedOrderStockItems.Any(c => !c.HasStock))
            {
                _manager.SendOrderStockRejectedIntegrationEvent(command.OrderId, confirmedOrderStockItems);
            }
            else
            {
                _manager.SendOrderStockConfirmedIntegrationEvent(command.OrderId);
            }
        }

        private CatalogItem GetCatalogItemById(Guid id)
        {
            using (var connection = new SqlConnection(_configuration.ConnectionString))
            {
                connection.Open();
                using (var cmd = connection.CreateCommand())
                {
                    cmd.CommandText = "SELECT [c].[CatalogItemId], [c].[AvailableStock] FROM [dbo].[Catalog] AS [c] WHERE [c].[CatalogItemId]=@id";
                    cmd.Parameters.Add("@id", System.Data.SqlDbType.UniqueIdentifier).Value = id;
                    var reader = cmd.ExecuteReader();
                    reader.Read();
                    var item = new CatalogItem
                    {
                        CatalogItemId = Guid.Parse(reader["CatalogItemId"].ToString()),
                        AvailableStock = (int)reader["AvailableStock"]
                    };
                    return item;
                }
            }
        }
    }
}
