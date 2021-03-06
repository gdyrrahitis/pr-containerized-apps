﻿using Catalog.API.Models;
using System;
using System.Collections.Generic;

namespace Catalog.API.Infrastructure.IntegrationEvents
{
    public class OrderStockRejectedIntegrationEvent
    {
        public Guid OrderId { get; set; }
        public IEnumerable<ConfirmedOrderStockItem> ConfirmedOrderStockItems { get; set; }
    }
}
