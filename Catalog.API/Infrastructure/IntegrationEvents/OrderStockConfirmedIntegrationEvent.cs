﻿using System;

namespace Catalog.API.Infrastructure.IntegrationEvents
{
    public class OrderStockConfirmedIntegrationEvent
    {
        public Guid OrderId { get; set; }
    }
}
