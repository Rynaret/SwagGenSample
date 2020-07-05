using System;
using System.Collections.Generic;
using System.Linq;
using App.ApiDtos;
using Microsoft.AspNetCore.Mvc;

namespace App.Controllers.V2
{
    [ApiController]
    [ApiVersion("2.0")]
    [Route("v{version:apiVersion}/weather-forecast")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        [HttpGet]
        public IEnumerable<WeatherForecastV2> Get()
        {
            var rng = new Random();
            return Enumerable.Range(1, 5).Select(index => new WeatherForecastV2
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            })
            .ToArray();
        }

        [HttpGet("generic-response")]
        public ApiResponse<IEnumerable<WeatherForecastV2>> GenericResponse()
        {
            var rng = new Random();
            return Enumerable.Range(1, 5).Select(index => new WeatherForecastV2
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            })
            .ToArray();
        }
    }
}
