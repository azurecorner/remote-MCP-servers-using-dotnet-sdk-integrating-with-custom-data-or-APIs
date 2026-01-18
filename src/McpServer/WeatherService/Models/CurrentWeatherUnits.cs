using System.Text.Json.Serialization;

namespace WeatherService.Models
{
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
}