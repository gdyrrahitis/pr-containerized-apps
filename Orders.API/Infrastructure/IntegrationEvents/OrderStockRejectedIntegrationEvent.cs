using Orders.API.Models;
using System;
using System.Collections.Generic;

namespace Orders.API.Infrastructure.IntegrationEvents
{
    public class OrderStockRejectedIntegrationEvent
    {
        public Guid OrderId { get; set; }
        public IEnumerable<ConfirmedOrderStockItem> ConfirmedOrderStockItems { get; set; }
    }
}
