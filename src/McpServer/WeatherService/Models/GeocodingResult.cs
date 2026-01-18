using System.Text.Json.Serialization;

namespace WeatherService.Models
{
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