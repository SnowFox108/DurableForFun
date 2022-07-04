using Microsoft.AspNetCore.Mvc;
using Wkhtmltopdf.NetCore;

namespace PdfDockerTest.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class HomeController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

        private readonly ILogger<HomeController> _logger;

        private readonly IGeneratePdf _generatePdf;

        public HomeController(ILogger<HomeController> logger, IGeneratePdf generatePdf)
        {
            _logger = logger;
            _generatePdf = generatePdf;
        }

        //[HttpGet]
        //public IEnumerable<WeatherForecast> Get()
        //{
        //    return Enumerable.Range(1, 5).Select(index => new WeatherForecast
        //    {
        //        Date = DateTime.Now.AddDays(index),
        //        TemperatureC = Random.Shared.Next(-20, 55),
        //        Summary = Summaries[Random.Shared.Next(Summaries.Length)]
        //    })
        //    .ToArray();
        //}

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var result = await _generatePdf.GetPdf("Index");
            return result;
        }
    }
}