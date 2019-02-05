using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Catalog.API.Infrastructure.IntegrationEvents
{
    public class OrderStockConfirmedIntegrationEvent
    {
        public Guid OrderId { get; set; }
    }
}
