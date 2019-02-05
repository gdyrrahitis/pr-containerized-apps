﻿using Orders.API.Models;
using System;
using System.Collections.Generic;

namespace Orders.API.Infrastructure.IntegrationEvents
{
    public class OrderStatusChangedToAwaitingValidationIntegrationEvent
    {
        public Guid OrderId { get; set; }
        public IEnumerable<OrderStockItem> CatalogItems { get; set; }
    }
}
