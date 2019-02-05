using System;

namespace Orders.API.Infrastructure.IntegrationEvents
{
    public class OrderStockConfirmedIntegrationEvent
    {
        public Guid OrderId { get; set; }
    }
}
