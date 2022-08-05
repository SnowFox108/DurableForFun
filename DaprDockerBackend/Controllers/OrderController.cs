using Dapr;
using DaprDockerBackend.Models;
using Microsoft.AspNetCore.Mvc;

namespace DaprDockerBackend.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly ILogger<OrderController> _logger;

        public OrderController(ILogger<OrderController> logger)
        {
            _logger = logger;
        }

        [HttpPost("", Name = "Printer")]
        [Topic("servicebus-pubsub", "printer")]
        public async Task<IActionResult> Submit(Basket basket)
        {
            _logger.LogInformation($"Received a new order from {basket.Id}");
            foreach (var fruit in basket.Fruits)
            {
                _logger.LogInformation($"Fruit in basket is {fruit.Name}");
            }
            return Ok();
        }
    }
}
