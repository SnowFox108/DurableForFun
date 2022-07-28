using DaprDockerTester.Models;
using DaprDockerTester.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DaprDockerTester.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DaprSubmitController : ControllerBase
    {
        private readonly IOrderSubmitService _orderSubmitService;
        private readonly ILogger<DaprSubmitController> _logger;

        public DaprSubmitController(
            ILogger<DaprSubmitController> logger, 
            IOrderSubmitService orderSubmitService)
        {
            _logger = logger;
            _orderSubmitService = orderSubmitService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(int id)
        {
            var basket = new Basket(id);

            await _orderSubmitService.SubmitOrder(basket);
            return Ok();
        }

    }
}
