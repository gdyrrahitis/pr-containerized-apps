using System;

namespace Catalog.API.Models
{
    public class CatalogItem
    {
        public int Id { get; set; }
        public Guid CatalogItemId { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
    }
}
