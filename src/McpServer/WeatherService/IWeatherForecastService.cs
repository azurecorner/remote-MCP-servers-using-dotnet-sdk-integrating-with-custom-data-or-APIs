using WeatherService.Models;

namespace WeatherService
{
    public interface IWeatherForecastService
    {
        Task<WeatherForecast> GetWeatherForecast(string city);

        Task<(double Latitude, double Longitude)> GetCoordinatesAsync(string city);
    }
}