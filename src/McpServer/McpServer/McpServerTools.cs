using ModelContextProtocol.Server;
using System.ComponentModel;
using WeatherService;
using WeatherService.Models;

namespace McpServer;

[McpServerToolType]
public sealed class McpServerTools
{
    private readonly IWeatherForecastService _weatherForecastService;

    private readonly ILogger<McpServerTools> _logger;

    public McpServerTools(IWeatherForecastService weatherForecastService, ILogger<McpServerTools> logger)
    {
        this._weatherForecastService = weatherForecastService;
        _logger = logger;
    }

    [McpServerTool]
    public async Task<string> Ping([Description("Health check tool. Behaves as follows:\n" +
     "- If message is empty, respond with server health\n" +
     "- Otherwise echo the message")] string message)
    {
        await Task.CompletedTask;

        return string.IsNullOrWhiteSpace(message)
            ? "✅ MCP server is alive."
            : $"✅ MCP server received: {message}";
    }

    [McpServerTool]
    public async Task<WeatherForecast> GetWeather([Description("The name of city")] string city)
    {
        _logger.LogInformation("GetWeather called with city: {City}", city);
        var forecasts = await _weatherForecastService.GetWeatherForecast(city);

        _logger.LogInformation("GetWeather returning forecast for city: {City}", city);
        return forecasts;
    }
}