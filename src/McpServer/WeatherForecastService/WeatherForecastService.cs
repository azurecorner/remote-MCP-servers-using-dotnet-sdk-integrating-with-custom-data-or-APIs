using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace McpServer
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

    public interface IWeatherForecastService
    {
        Task<WeatherForecast> GetWeatherForecast(string city);

        Task<(double Latitude, double Longitude)> GetCoordinatesAsync(string city);
    }

    public class WeatherForecast
    {
        [JsonPropertyName("latitude")]
        public double Latitude { get; init; }

        [JsonPropertyName("longitude")]
        public double Longitude { get; init; }

        [JsonPropertyName("generationtime_ms")]
        public double GenerationTimeMs { get; init; }

        [JsonPropertyName("utc_offset_seconds")]
        public int UtcOffsetSeconds { get; init; }

        [JsonPropertyName("timezone")]
        public string Timezone { get; init; } = string.Empty;

        [JsonPropertyName("timezone_abbreviation")]
        public string TimezoneAbbreviation { get; init; } = string.Empty;

        [JsonPropertyName("elevation")]
        public double Elevation { get; init; }

        [JsonPropertyName("current_weather_units")]
        public CurrentWeatherUnits CurrentWeatherUnits { get; init; } = new();

        [JsonPropertyName("current_weather")]
        public CurrentWeather CurrentWeather { get; init; } = new();
    }

    public sealed class CurrentWeather
    {
        [JsonPropertyName("time")]
        public DateTime Time { get; init; }

        [JsonPropertyName("interval")]
        public int Interval { get; init; }

        [JsonPropertyName("temperature")]
        public double Temperature { get; init; }

        [JsonPropertyName("windspeed")]
        public double WindSpeed { get; init; }

        [JsonPropertyName("winddirection")]
        public int WindDirection { get; init; }

        [JsonPropertyName("is_day")]
        public int IsDay { get; init; }

        [JsonPropertyName("weathercode")]
        public int WeatherCode { get; init; }
    }

    public sealed class CurrentWeatherUnits
    {
        [JsonPropertyName("time")]
        public string Time { get; init; } = string.Empty;

        [JsonPropertyName("interval")]
        public string Interval { get; init; } = string.Empty;

        [JsonPropertyName("temperature")]
        public string Temperature { get; init; } = string.Empty;

        [JsonPropertyName("windspeed")]
        public string WindSpeed { get; init; } = string.Empty;

        [JsonPropertyName("winddirection")]
        public string WindDirection { get; init; } = string.Empty;

        [JsonPropertyName("is_day")]
        public string IsDay { get; init; } = string.Empty;

        [JsonPropertyName("weathercode")]
        public string WeatherCode { get; init; } = string.Empty;
    }

    public sealed class GeocodingResponse
    {
        [JsonPropertyName("results")]
        public List<GeocodingResult>? Results { get; init; }
    }

    public sealed class GeocodingResult
    {
        [JsonPropertyName("name")]
        public string Name { get; init; } = string.Empty;

        [JsonPropertyName("country")]
        public string Country { get; init; } = string.Empty;

        [JsonPropertyName("latitude")]
        public double Latitude { get; init; }

        [JsonPropertyName("longitude")]
        public double Longitude { get; init; }
    }
}