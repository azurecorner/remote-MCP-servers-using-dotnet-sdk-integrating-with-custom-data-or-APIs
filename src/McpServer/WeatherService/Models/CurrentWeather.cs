using System.Text.Json.Serialization;

namespace WeatherService.Models
{
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
}