using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Orders.API.Models;

namespace Orders.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IMongoCollection<Order> _ordersCollection;
        private readonly IRabbitMqManager _manager;

        public OrdersController(IOptions<EnvironmentConfiguration> options, IRabbitMqManager manager)
        {
            var client = new MongoClient(options.Value.ConnectionString);
            var database = client.GetDatabase(options.Value.DatabaseName);
            _ordersCollection = database.GetCollection<Order>(options.Value.CollectionName);
            _manager = manager;
        }

        [HttpGet]
        public IActionResult Get()
        {
            return Ok(_ordersCollection.Find(o => true).ToList());
        }

        [HttpGet("{id}")]
        public IActionResult Get(string id)
        {
            var order = _ordersCollection.Find(o => o.Id == id).FirstOrDefault();
            return order != null ? (IActionResult)Ok(order) : NotFound();
        }

        // For demo
        [HttpPost]
        public IActionResult Post([FromBody] OrderStatusRequest request)
        {
            var order = _ordersCollection.Find(o => o.OrderId == request.Id).FirstOrDefault();
            if (order == null)
            {
                return NotFound();
            }
            _manager.SendOrderStatusChangedToAwaitingValidationIntegrationEvent(Guid.Parse(order.OrderId), order.OrderItems);
            return Ok();
        }
    }

    public class OrderStatusRequest
    {
        public string Id { get; set; }
    }
}
