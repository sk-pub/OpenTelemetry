using Microsoft.AspNetCore.Mvc;

namespace Api1.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private const string Api2Endpoint = "http://localhost:5100/WeatherForecast";

        private readonly ILogger<WeatherForecastController> _logger;
        private readonly HttpClient _httpClient;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, HttpClient httpClient)
        {
            _logger = logger;
            _httpClient = httpClient;
        }

        [HttpGet(Name = "GetWeatherForecast")]
        public async Task<IEnumerable<WeatherForecast>> Get()
        {
            var httpResponse = await _httpClient.GetAsync(Api2Endpoint);

            var result = await httpResponse.Content.ReadFromJsonAsync<WeatherForecast[]>();

            return result;
        }
    }
}