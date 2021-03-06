﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Newtonsoft.Json;
using Orders.API.Infrastructure.IntegrationEvents;
using Orders.API.Models;
using RabbitMQ.Client;

namespace Orders.API.Infrastructure
{
    public class OrderStockRejectedIntegrationEventHandler: DefaultBasicConsumer
    {
        private IRabbitMqManager _manager;
        private IOptions<EnvironmentConfiguration> _options;

        public OrderStockRejectedIntegrationEventHandler(IRabbitMqManager rabbitMqManager, IOptions<EnvironmentConfiguration> options)
        {
            _manager = rabbitMqManager;
            _options = options;
        }

        public override void HandleBasicDeliver(string consumerTag, ulong deliveryTag, bool redelivered, string exchange, string routingKey,
            IBasicProperties properties, byte[] body)
        {
            if (properties.ContentType != "application/json")
            {
                throw new ArgumentException($"Can't handle content type of {properties.ContentType}");
            }

            var message = Encoding.UTF8.GetString(body);
            var command = JsonConvert.DeserializeObject<OrderStockRejectedIntegrationEvent>(message);
            Consume(command);
            _manager.SendAck(deliveryTag);
        }

        public void Consume(OrderStockRejectedIntegrationEvent command)
        {
            var client = new MongoClient(_options.Value.ConnectionString);
            var database = client.GetDatabase(_options.Value.DatabaseName);
            var ordersCollection = database.GetCollection<Order>(_options.Value.CollectionName);

            var order = ordersCollection.Find(o => o.OrderId == command.OrderId.ToString("N")).FirstOrDefault();
            if (order == null)
            {
                throw new Exception($"Cannot find order with id {command.OrderId}");
            }

            Console.WriteLine("Stock is rejected");
        }
    }
}
