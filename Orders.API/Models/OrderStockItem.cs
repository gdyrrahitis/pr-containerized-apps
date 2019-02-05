using System;

namespace Orders.API.Models
{
    public class OrderStockItem
    {
        public Guid ProductId { get; set; }

        public int Units { get; set; }
    }
}
