using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Orders.API.Models
{
    public class Order
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("total")]
        public decimal Total { get; set; }

        [BsonElement("items")]
        public int Items { get; set; }

        [BsonElement("orderItems")]
        public IEnumerable<OrderItem> OrderItems { get; set; }
    }
}
