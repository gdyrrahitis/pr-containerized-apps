using System;

namespace Orders.API.Models
{
    public class ConfirmedOrderStockItem
    {
        public Guid CatalogItemId { get; set; }

        public bool HasStock { get; set; }
    }
}
