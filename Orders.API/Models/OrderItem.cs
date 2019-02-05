using MongoDB.Bson.Serialization.Attributes;
using System;

namespace Orders.API.Models
{
    public class OrderItem
    {
        [BsonElement("productId")]
        public Guid ProductId { get; set; }

        [BsonElement("item")]
        public string Item { get; set; }

        [BsonElement("price")]
        public decimal Price { get; set; }

        [BsonElement("units")]
        public int Units { get; set; }
    }
}
