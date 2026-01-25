﻿using ModelContextProtocol.Server;
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
        _weatherForecastService = weatherForecastService;
        _logger = logger;
    }

    [McpServerTool]
    [Description("Health check tool that verifies the MCP server is running. If a message is provided, it echoes it back; otherwise, returns server health status.")]
    public Task<string> Ping([Description("Optional message to echo back. If empty, returns health status.")] string message)
    {
        _logger.LogInformation($"MCP server is alive => cheching input => {message}");
        return Task.FromResult(
            string.IsNullOrWhiteSpace(message)
                ? "✅ MCP server is alive."
                : $"✅ MCP server received: {message}"
        );
    }

    [McpServerTool]
    [Description("Retrieves the current weather forecast for a specified city.")]
    public async Task<WeatherForecast> GetWeather([Description("The name of the city to get weather forecast for.")] string city)
    {
        _logger.LogInformation("GetWeather called with city: {City}", city);
        var forecasts = await _weatherForecastService.GetWeatherForecast(city);

        _logger.LogInformation("GetWeather returning forecast for city: {City}", city);
        return forecasts;
    }
}