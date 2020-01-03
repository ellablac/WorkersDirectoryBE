using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace WorkersDirectoryBE.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForeController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForeController> _logger;

        public WeatherForeController(ILogger<WeatherForeController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IEnumerable<WeatherFore> Get()
        {
            var rng = new Random();
            return Enumerable.Range(1, 5).Select(index => new WeatherFore
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            })
            .ToArray();
        }

        // GET api/weatherfore/5
        [HttpGet("{id}")]
        public WeatherFore Get(int id)
        {
            var rng = new Random();
            return Enumerable.Range(1, 5).Select(index => new WeatherFore
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            })
            .ToArray().ElementAt(id);
        }
    }
}
