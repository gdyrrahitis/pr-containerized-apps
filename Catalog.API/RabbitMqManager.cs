using Catalog.API.Infrastructure;
using Catalog.API.Infrastructure.IntegrationEvents;
using Catalog.API.Models;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace Catalog.API
{
    public interface IRabbitMqManager
    {
        void ListenForOrderStatusChangedToAwaitingValidationEvent();
        void SendOrderStockRejectedIntegrationEvent(Guid orderId, IEnumerable<ConfirmedOrderStockItem> confirmedOrderStockItems);
        void SendOrderStockConfirmedIntegrationEvent(Guid orderId);
        void SendAck(ulong deliveryTag);
        void CreateConsumerChannel();
        void Disconnect();
    }

    public class RabbitMqManager : IRabbitMqManager, IDisposable
    {
        const string Exchange = "eshop.exchange";
        const string OrderStatusChangedToAwaitingQueue = "eshop.queue.status.awaiting";
        const string OrderStockRejectedQueue = "eshop.queue.order.stock.rejected";
        const string OrderStockConfirmedQueue = "eshop.queue.order.stock.confirmed";
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private readonly IOptions<EnvironmentConfiguration> _options;

        public RabbitMqManager(IConnectionFactory connectionFactory, IOptions<EnvironmentConfiguration> options)
        {
            _connection = connectionFactory.CreateConnection();
            _channel = _connection.CreateModel();
            _options = options;
        }

        public void ListenForOrderStatusChangedToAwaitingValidationEvent()
        {
            var consumer = new OrderStatusChangedToAwaitingValidationIntegrationEventHandler(this, _options);
            _channel.BasicConsume(queue: OrderStatusChangedToAwaitingQueue,
                autoAck: false,
                consumer: consumer);
        }

        public void SendAck(ulong deliveryTag) => _channel.BasicAck(deliveryTag: deliveryTag, multiple: false);

        public void Dispose()
        {
            _channel.Close();
            _connection.Close();
        }

        public void SendOrderStockRejectedIntegrationEvent(Guid orderId, IEnumerable<ConfirmedOrderStockItem> confirmedOrderStockItems)
        {
            var integrationEvent = new OrderStockRejectedIntegrationEvent
            {
                ConfirmedOrderStockItems = confirmedOrderStockItems,
                OrderId = orderId
            };
            var serializedCommand = JsonConvert.SerializeObject(integrationEvent);
            var properties = _channel.CreateBasicProperties();
            properties.ContentType = "application/json";

            _channel.BasicPublish(Exchange, routingKey: "",
                basicProperties: properties, body: Encoding.UTF8.GetBytes(serializedCommand));
        }

        public void SendOrderStockConfirmedIntegrationEvent(Guid orderId)
        {
            var integrationEvent = new OrderStockConfirmedIntegrationEvent
            {
                OrderId = orderId
            };
            var serializedCommand = JsonConvert.SerializeObject(integrationEvent);
            var properties = _channel.CreateBasicProperties();
            properties.ContentType = "application/json";

            _channel.BasicPublish(Exchange, routingKey: "",
                basicProperties: properties, body: Encoding.UTF8.GetBytes(serializedCommand));
        }

        public void CreateConsumerChannel()
        {
            _channel.QueueDeclare(queue: OrderStatusChangedToAwaitingQueue, durable: false, exclusive: false, autoDelete: false, arguments: null);
            _channel.BasicQos(prefetchCount: 1, prefetchSize: 0, global: false);

            _channel.ExchangeDeclare(Exchange, ExchangeType.Direct);
            _channel.QueueDeclare(OrderStockRejectedQueue, durable: false, exclusive: false,
                autoDelete: false, arguments: null);
            _channel.QueueBind(OrderStockRejectedQueue, exchange: Exchange, routingKey: "");

            _channel.QueueDeclare(OrderStockConfirmedQueue, durable: false, exclusive: false,
                autoDelete: false, arguments: null);
            _channel.QueueBind(OrderStockConfirmedQueue, exchange: Exchange, routingKey: "");

            ListenForOrderStatusChangedToAwaitingValidationEvent();
        }

        public void Disconnect() => Dispose();
    }
}
