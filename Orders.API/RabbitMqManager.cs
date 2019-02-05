using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Orders.API.Infrastructure;
using Orders.API.Infrastructure.IntegrationEvents;
using Orders.API.Models;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Orders.API
{
    public interface IRabbitMqManager
    {
        void ListenForOrderStockRejectedEvent();
        void ListenForOrderStockConfirmedEvent();
        void SendOrderStatusChangedToAwaitingValidationIntegrationEvent(Guid orderId, IEnumerable<OrderItem> orderItems);
        void SendAck(ulong deliveryTag);
        void CreateConsumerChannel();
        void Disconnect();
    }

    public class RabbitMqManager : IRabbitMqManager, IDisposable
    {
        const string ExchangeStatusAwaiting = "eshop.exchange";
        const string ExchangeStockRejected = "eshop.exchange.stock.rejected";
        const string ExchangeStockConfirmed = "eshop.exchange.stock.confirmed";
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

        public void CreateConsumerChannel()
        {
            _channel.QueueDeclare(OrderStockRejectedQueue, durable: false, exclusive: false,
                autoDelete: false, arguments: null);
            _channel.QueueDeclare(OrderStockConfirmedQueue, durable: false, exclusive: false,
                autoDelete: false, arguments: null);

            _channel.BasicQos(prefetchCount: 1, prefetchSize: 0, global: false);

            _channel.ExchangeDeclare(ExchangeStatusAwaiting, ExchangeType.Direct);
            _channel.ExchangeDeclare(ExchangeStockRejected, ExchangeType.Direct);
            _channel.ExchangeDeclare(ExchangeStockConfirmed, ExchangeType.Direct);

            _channel.QueueDeclare(queue: OrderStatusChangedToAwaitingQueue, durable: false, exclusive: false, autoDelete: false, arguments: null);
            _channel.QueueBind(OrderStatusChangedToAwaitingQueue, exchange: ExchangeStatusAwaiting, routingKey: "");
        }

        public void Disconnect() => Dispose();

        public void Dispose()
        {
            _channel.Close();
            _connection.Close();
        }

        public void ListenForOrderStockConfirmedEvent()
        {
            var consumer = new OrderStockConfirmedIntegrationEventHandler(this, _options);
            _channel.BasicConsume(queue: OrderStockConfirmedQueue,
                autoAck: false,
                consumer: consumer);
        }

        public void ListenForOrderStockRejectedEvent()
        {
            var consumer = new OrderStockRejectedIntegrationEventHandler(this, _options);
            _channel.BasicConsume(queue: OrderStockRejectedQueue,
                autoAck: false,
                consumer: consumer);
        }

        public void SendAck(ulong deliveryTag) => _channel.BasicAck(deliveryTag: deliveryTag, multiple: false);

        public void SendOrderStatusChangedToAwaitingValidationIntegrationEvent(Guid orderId, IEnumerable<OrderItem> orderItems)
        {
            var integrationEvent = new OrderStatusChangedToAwaitingValidationIntegrationEvent
            {
                OrderId = orderId,
                CatalogItems = orderItems.Select(o =>
                {
                    return new OrderStockItem
                    {
                        ProductId = o.ProductId,
                        Units = o.Units
                    };
                })
            };

            var serializedCommand = JsonConvert.SerializeObject(integrationEvent);
            var properties = _channel.CreateBasicProperties();
            properties.ContentType = "application/json";

            _channel.BasicPublish(ExchangeStatusAwaiting, routingKey: "",
                basicProperties: properties, body: Encoding.UTF8.GetBytes(serializedCommand));
        }
    }
}
