using McpServer;
using WeatherService;

var builder = WebApplication.CreateBuilder(args);

var port = Environment.GetEnvironmentVariable("FUNCTIONS_CUSTOMHANDLER_PORT") ?? "8081";
builder.WebHost.UseUrls($"http://0.0.0.0:{port}");

builder.Services.AddMcpServer()
    .WithHttpTransport((options) =>
    {
        options.Stateless = true;
    })
     .WithStdioServerTransport()
    .WithToolsFromAssembly()
    .WithTools<McpServerTools>();

builder.Services.AddHttpClient();
builder.Services.AddScoped<IWeatherForecastService, WeatherForecastService>();

builder.Logging.AddConsole(options =>
{
    options.LogToStandardErrorThreshold = LogLevel.Trace;
});
var app = builder.Build();

// Add health check endpoint
app.MapGet("/api/healthz", () => "Healthy");

// Map MCP endpoints
app.MapMcp(pattern: "/mcp");

// Await the RunAsync method in an async Main
await app.RunAsync();