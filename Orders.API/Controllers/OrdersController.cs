using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

        public OrdersController(IOptions<EnvironmentConfiguration> options)
        {
            var client = new MongoClient(options.Value.ConnectionString);
            var database = client.GetDatabase(options.Value.DatabaseName);
            _ordersCollection = database.GetCollection<Order>(options.Value.CollectionName);
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
            return order != null ? (IActionResult) Ok(order): NotFound();
        }
    }
}
