using System.Net.Http.Json;
using System.Text.Json;
using WeatherService.Models;

namespace WeatherService
{
    public class WeatherForecastService : IWeatherForecastService
    {
        private readonly HttpClient httpClient;

        public WeatherForecastService(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public async Task<WeatherForecast> GetWeatherForecast(string city)
        {
            var (lat, lon) = await GetCoordinatesAsync(city);

            var response = await httpClient.GetAsync($"https://api.open-meteo.com/v1/forecast?latitude={lat}&longitude={lon}&current_weather=true");
            if (response.IsSuccessStatusCode)
            {
                var forecast = await response.Content.ReadFromJsonAsync<WeatherForecast>();
                return forecast ?? throw new InvalidOperationException("Invalid weather forecast response");
            }
            throw new InvalidOperationException($"Failed to retrieve weather forecast for city {city} with longitude{lon} and latitude {lat}");
        }

        public async Task<(double Latitude, double Longitude)> GetCoordinatesAsync(string city)
        {
            using var http = new HttpClient();

            var url =
                $"https://geocoding-api.open-meteo.com/v1/search" +
                $"?name={Uri.EscapeDataString(city)}&count=1&language=en&format=json";

            var json = await http.GetStringAsync(url);

            var response = JsonSerializer.Deserialize<GeocodingResponse>(json)
                ?? throw new InvalidOperationException("Invalid geocoding response");

            var result = response.Results?.FirstOrDefault()
                ?? throw new InvalidOperationException($"City '{city}' not found");

            return (result.Latitude, result.Longitude);
        }
    }
}