using MongoDB.Bson.Serialization.Attributes;

namespace Orders.API.Models
{
    public class OrderItem
    {
        [BsonElement("item")]
        public string Item { get; set; }

        [BsonElement("price")]
        public decimal Price { get; set; }
    }
}
