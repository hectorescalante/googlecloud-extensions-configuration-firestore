using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GoogleCloud.Extensions.Configuration.Sample.Controllers
{
  [ApiController]
  [Route("[controller]")]
  public class WeatherForecastController : ControllerBase
  {
    private static readonly string[] _summaries = new[]
    {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

    private readonly ILogger<WeatherForecastController> _logger;
    private readonly WeatherOptions _weatherOptions;

    public WeatherForecastController(ILogger<WeatherForecastController> logger, IOptionsSnapshot<WeatherOptions> weatherOptions)
    {
      _logger = logger;
      _weatherOptions = weatherOptions.Value;
    }

    [HttpGet]
    public IEnumerable<WeatherForecast> Get()
    {
      _logger.LogInformation($"Getting weather for the next {_weatherOptions.Days} days...");
      var rng = new Random();
      return Enumerable.Range(1, _weatherOptions.Days).Select(index => new WeatherForecast
      {
        Date = DateTime.Now.AddDays(index),
        TemperatureC = rng.Next(-20, 55),
        Summary = _summaries[rng.Next(_summaries.Length)]
      })
      .ToArray();
    }
  }
}
