using System.Text.Json.Serialization;

namespace WeatherService.Models
{
    public sealed class GeocodingResponse
    {
        [JsonPropertyName("results")]
        public List<GeocodingResult>? Results { get; init; }
    }
}