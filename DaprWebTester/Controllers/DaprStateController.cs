using DaprWebTester.Models;
using DaprWebTester.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DaprWebTester.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DaprStateController : ControllerBase
    {
        private readonly IStateBasketService _basketService;
        private readonly ILogger<DaprStateController> _logger;

        public DaprStateController(ILogger<DaprStateController> logger, 
            IStateBasketService stateBasketService)
        {
            _logger = logger;
            _basketService = stateBasketService;
        }

        [HttpGet]
        public async Task<Fruit> Index(int id)
        {
            var fruit = await _basketService.GetFruit(id);
            return fruit;
        }

        [HttpPost]
        public async Task<string> AddFruit(Fruit fruit)
        {
            await _basketService.AddToBasket(fruit);
            return $"{fruit.Name} added to basket";
        }
    }
}
