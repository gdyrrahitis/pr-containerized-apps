using System;

namespace Catalog.API.Models
{
    public class CatalogItem
    {
        public int Id { get; set; }
        public Guid CatalogItemId { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int AvailableStock { get; set; }
    }

    public class ConfirmedOrderStockItem {
        public Guid CatalogItemId { get; set; }

        public bool HasStock { get; set; }
    }

    public class OrderStockItem
    {
        public Guid ProductId { get; set; }

        public int Units { get; set; }
    }
}
