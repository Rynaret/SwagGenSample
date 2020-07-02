﻿using System;
using System.Collections.Generic;
using System.Linq;
using App.ApiDtos;
using Microsoft.AspNetCore.Mvc;

namespace App.Controllers
{
    [ApiController]
    [ApiVersion("2.0")]
    [ApiVersion("1.0")]
    [Route("v{version:apiVersion}/weather-forecast")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        [HttpGet]
        [MapToApiVersion("1.0")]
        public IEnumerable<WeatherForecast> Get()
        {
            var rng = new Random();
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            })
            .ToArray();
        }

        [HttpGet("say-hello")]
        public string SayHello()
        {
            return "Hello";
        }
    }
}
