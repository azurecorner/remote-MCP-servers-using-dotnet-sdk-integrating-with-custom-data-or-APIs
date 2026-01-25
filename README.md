# Remote MCP Servers using .NET SDK - Integrating with custom data and APIs

A comprehensive example demonstrating how to build **Model Context Protocol (MCP)** servers using the .NET SDK, showcasing integration with custom data sources and APIs. This project implements a weather forecast service as an example of how to expose your own APIs through MCP tools.

## Table of Contents

- [Overview](#overview)
- [What is MCP?](#what-is-mcp)
- [Features](#features)
- [Architecture](#architecture)
- [Prerequisites](#prerequisites)
- [Project Structure](#project-structure)
- [Development](#development)
- [Quick Start](#quick-start)
- [Available MCP Tools](#available-mcp-tools)
- [Usage](#usage)
- [Contributing](#contributing)
- [License](#license)

## Overview

This project demonstrates how to create **remote MCP servers** that can be consumed by AI applications (like Claude Desktop, VS Code Copilot, etc.) to extend their capabilities with custom tools and data sources. The example implementation includes a weather forecast service that showcases the integration pattern.

MCP servers really shine when they’re connected to existing APIs or services, allowing clients to query real, live data. There’s an expanding ecosystem of MCP servers that can already be used by clients, including tools we rely on daily like Git, GitHub, local filesystem, etc.

With that in mind, let’s enhance our MCP server by wiring it up to an API, accepting query parameters, and returning data-driven responses.

## What is MCP?

**Model Context Protocol (MCP)** is an open protocol that standardizes how applications provide context to Large Language Models (LLMs). It enables AI assistants to:

- Access external data sources
- Execute custom tools and functions
- Integrate with your own APIs and services
- Maintain context across multiple interactions

## Features

- **HTTP Transport Support** - Expose MCP servers via HTTP endpoints for remote access
- **Stdio Transport Support** - Support for standard input/output communication
- **Custom Tool Integration** - Easy integration of your own APIs and services
- **Health Monitoring** - Built-in health check endpoints
- **Stateless Operation** - Designed for scalable, stateless deployments
- **.NET 10.0** - Built on the latest .NET framework
- **Structured Logging** - Comprehensive logging for debugging and monitoring

## Architecture

The project follows a clean architecture pattern:

```text
┌─────────────────┐
│   MCP Client    │ (Claude Desktop, VS Code, etc.)
└────────┬────────┘
         │ HTTP/SSE
         ▼
┌─────────────────┐
│  ASP.NET Core   │
│   Web Host      │
└────────┬────────┘
         │
         ▼
┌─────────────────┐
│  MCP Protocol   │
│     Layer       │
└────────┬────────┘
         │
         ▼
┌─────────────────┐
│   MCP Tools     │ (Weather, Ping, etc.)
└────────┬────────┘
         │
         ▼
┌─────────────────┐
│  Your APIs &    │
│  Data Sources   │
└─────────────────┘
```

## Prerequisites

- [.NET 10.0 SDK](https://dotnet.microsoft.com/download/dotnet/10.0) or later
- Visual Studio 2022 / VS Code / any compatible editor
- An MCP-compatible client:
  - [Claude Desktop](https://claude.ai/download) (recommended)
  - [VS Code with MCP extension](https://marketplace.visualstudio.com/items?itemName=modelcontextprotocol.mcp-vscode)
  - [Kiro](https://github.com/modelcontextprotocol/kiro) or other MCP clients

## Project Structure

```text
remote-MCP-servers-using-dotnet-sdk-integrating-with-our-own-data-or-apis/
├── src/
│   └── McpServer/
│       ├── McpServer/
│       │   ├── Program.cs              # Application entry point
│       │   ├── appsettings.json        # Configuration settings
│       │   ├── McpServerTools.cs       # MCP tool implementations
│       │   └── Services/               # Business logic services
│       │       └── WeatherService.cs   # Weather API integration
│       └── McpServer.sln
├── docs/                               # Additional documentation
├── README.md                           # This file
└── LICENSE
```

## Development

This section explains how to create custom MCP tools and integrate them with your own APIs and data sources.

### Creating Custom MCP Tools

MCP tools are the bridge between AI clients and your backend services. Each tool represents a capability that AI assistants can invoke.

#### Tool Architecture

```text
┌──────────────────┐
│   MCP Client     │ (Claude, VS Code, etc.)
└────────┬─────────┘
         │ "Get weather for Paris"
         ▼
┌──────────────────┐
│ McpServerTools   │ [McpServerTool] decorated methods
└────────┬─────────┘
         │
         ▼
┌──────────────────┐
│ Business Service │ IWeatherForecastService
└────────┬─────────┘
         │
         ▼
┌──────────────────┐
│  External API    │ Weather API, Database, etc.
└──────────────────┘
```

#### Step 1: Define Your Tool Class

Create a class decorated with `[McpServerToolType]`:

```csharp
[McpServerToolType]
public sealed class McpServerTools
{
    private readonly IWeatherForecastService _weatherForecastService;
    private readonly ILogger<McpServerTools> _logger;

    public McpServerTools(
        IWeatherForecastService weatherForecastService, 
        ILogger<McpServerTools> logger)
    {
        _weatherForecastService = weatherForecastService;
        _logger = logger;
    }
}
```

**Key Points:**

- Use `[McpServerToolType]` to mark the class as containing MCP tools
- Inject services via constructor (supports standard .NET DI)
- Follow .NET dependency injection patterns

#### Step 2: Create Tool Methods

Each method decorated with `[McpServerTool]` becomes an invokable tool:

```csharp
[McpServerTool]
[Description("Retrieves the current weather forecast for a specified city.")]
public async Task<WeatherForecast> GetWeather(
    [Description("The name of the city to get weather forecast for.")] 
    string city)
{
    _logger.LogInformation("GetWeather called with city: {City}", city);
    
    // Call your business service/API
    var forecasts = await _weatherForecastService.GetWeatherForecast(city);

    _logger.LogInformation("GetWeather returning forecast for city: {City}", city);
    return forecasts;
}
```

**Key Attributes:**

- `[McpServerTool]` - Marks method as an MCP tool
- `[Description("...")]` - Provides description for AI to understand tool purpose
- Parameter descriptions help AI choose correct arguments

**Best Practices:**

- Use clear, descriptive names (e.g., `GetWeather`, not `GW`)
- Add detailed descriptions for both tool and parameters
- Use strongly-typed return values
- Include logging for diagnostics
- Handle exceptions gracefully

#### Step 3: Implement Your Business Service

Create a service that encapsulates your API/data logic:

```csharp
public interface IWeatherForecastService
{
    Task<WeatherForecast> GetWeatherForecast(string city);
}

public class WeatherForecastService : IWeatherForecastService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<WeatherForecastService> _logger;

    public WeatherForecastService(
        HttpClient httpClient, 
        ILogger<WeatherForecastService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<WeatherForecast> GetWeatherForecast(string city)
    {
        try
        {
            // Call external weather API
            var response = await _httpClient.GetAsync(
                $"https://api.weather.com/v1/forecast?city={city}");
            
            response.EnsureSuccessStatusCode();
            
            var forecast = await response.Content
                .ReadFromJsonAsync<WeatherForecast>();
            
            return forecast ?? throw new InvalidOperationException(
                "Failed to deserialize weather data");
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Failed to fetch weather for {City}", city);
            throw;
        }
    }
}
```

#### Step 4: Register Services

In `Program.cs`, register your services:

```csharp
var builder = WebApplication.CreateBuilder(args);

// Register MCP server
builder.Services.AddMcpServer();

// Register your business services
builder.Services.AddHttpClient<IWeatherForecastService, WeatherForecastService>(
    client =>
    {
        client.BaseAddress = new Uri("https://api.weather.com");
        client.DefaultRequestHeaders.Add("User-Agent", "MCP-Weather-Server/1.0");
    });

// Add logging
builder.Services.AddLogging(logging =>
{
    logging.AddConsole();
    logging.AddDebug();
});

var app = builder.Build();

// Map MCP endpoint
app.MapMcp("/mcp");

app.Run();
```

## Quick Start

### 1. Clone the Repository

```bash
git clone https://github.com/azurecorner/remote-MCP-servers-using-dotnet-sdk-integrating-with-our-own-data-or-apis.git
cd remote-MCP-servers-using-dotnet-sdk-integrating-with-our-own-data-or-apis
```

### 2. Restore Dependencies

```bash
cd src/McpServer/McpServer
dotnet restore
```

### 3. Run the Server

```bash
dotnet run
```

The server will start on `http://localhost:8081` by default.

### 4. Verify Health

```bash
curl http://localhost:8081/api/healthz
```

Expected response:

```powershell
StatusCode        : 200
StatusDescription : OK
Content           : Healthy
RawContent        : HTTP/1.1 200 OK
                    Transfer-Encoding: chunked
                    Content-Type: text/plain; charset=utf-8
                    Date: Sun, 25 Jan 2026 10:20:38 GMT
                    Server: Kestrel
```

## Available MCP Tools

This MCP server exposes tools that can be discovered and invoked by MCP clients. To see all available tools, you can query the server using the `tools/list` JSON-RPC method.

### Current Tools

- **`get_weather`** - Retrieves weather information for a specified city
  - **Parameters**: `city` (string) - The name of the city
  - **Returns**: Weather forecast data including temperature, conditions, and humidity

- **`ping`** - Simple echo tool for testing connectivity
  - **Parameters**: `message` (string) - Message to echo back
  - **Returns**: The same message with a timestamp

### Discovering Tools

You can discover all available tools by sending a `tools/list` request to the MCP server:

#### bash

```bash
curl -X POST http://localhost:8081/mcp \
     -H "Content-Type: application/json" \
     -H "Accept: application/json, text/event-stream" \
     -d '{
           "jsonrpc": "2.0",
           "id": 1,
           "method": "tools/list",
           "params": {}
         }'
```

#### powershell

```powershell
# MCP endpoint
$mcpEndpoint = "http://localhost:8081/mcp"

# JSON-RPC request body
$body = @{
    jsonrpc = "2.0"
    id      = 1
    method  = "tools/list"
    params  = @{}
} | ConvertTo-Json -Depth 5

# HTTP headers
$headers = @{
    "Content-Type" = "application/json"
    "Accept"       = "application/json, text/event-stream"
}

# Send request
$response = Invoke-WebRequest `
    -Uri $mcpEndpoint `
    -Method Post `
    -Headers $headers `
    -Body $body `
    -UseBasicParsing

# Read response content
$content = $response.Content

write-Host "Received Response:" -ForegroundColor Green
Write-Host $content -ForegroundColor White
```

**Expected Response:**

```json
{
  "jsonrpc": "2.0",
  "id": 1,
  "result": {
    "tools": [
      {
        "name": "get_weather",
        "description": "Get weather information for a city",
        "inputSchema": {
          "type": "object",
          "properties": {
            "city": {
              "type": "string",
              "description": "The city name"
            }
          },
          "required": ["city"]
        }
      },
      {
        "name": "ping",
        "description": "Echo a message back",
        "inputSchema": {
          "type": "object",
          "properties": {
            "message": {
              "type": "string",
              "description": "Message to echo"
            }
          },
          "required": ["message"]
        }
      }
    ]
  }
}
```

## Usage

### Direct API Testing

You can test MCP tools directly by sending JSON-RPC requests to the server endpoint. This is useful for debugging, testing, and understanding how MCP clients interact with your server.

#### Test Weather Tool

The `get_weather` tool retrieves weather information for a specified city. Use the following scripts to invoke the tool:

##### Bash

```bash
# Test Weather

# Default parameters
MCP_ENDPOINT="${1:-http://localhost:8081/mcp}"
TOOL_NAME="${2:-get_weather}"
CITY="${3:-Paris}"

# JSON-RPC request body
read -r -d '' BODY <<EOF
{
  "jsonrpc": "2.0",
  "id": 2,
  "method": "tools/call",
  "params": {
    "name": "$TOOL_NAME",
    "arguments": {
      "city": "$CITY"
    }
  }
}
EOF

# Call MCP server and output raw JSON
curl -s -X POST "$MCP_ENDPOINT" \
     -H "Content-Type: application/json" \
     -H "Accept: application/json, text/event-stream" \
     -d "$BODY"
```

##### PowerShell

```powershell
Param(
    [string]$mcpEndpoint = "http://localhost:8081/mcp",
    [string]$toolName = "get_weather",
    [hashtable]$toolParams = @{ city = "Paris" }
)
# Example usage:
#  dotnet run --project .\src\McpServer\McpServer\McpServer.csproj
# .\call-mcp-tool.ps1 -toolName "get_weather" -toolParams @{ city = "Paris" }
# .\call-mcp-tool.ps1 -toolName "ping" -mcpEndpoint http://localhost:8081/mcp  -toolParams @{ message = "hello" }


$mcpEndpoint = "http://localhost:8081/mcp"

$body = @{
    jsonrpc = "2.0"
    id      = 2
    method  = "tools/call"
    params  = @{
        name = $toolName
        arguments = $toolParams
    }
} | ConvertTo-Json -Depth 5

$headers = @{
    "Content-Type" = "application/json"
    "Accept"       = "application/json, text/event-stream"
}

$response = Invoke-WebRequest `
    -Uri $mcpEndpoint `
    -Method Post `
    -Headers $headers `
    -Body $body `
    -UseBasicParsing

Write-Host "Success!" -ForegroundColor Green
write-host $response.Content | ConvertTo-Json
```

## Contributing

Contributions are welcome! Please follow these steps:

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

---

## Acknowledgments

- [Model Context Protocol](https://modelcontextprotocol.io) - For the MCP specification
- [Anthropic](https://www.anthropic.com) - For Claude and MCP support
- [Microsoft](https://microsoft.com) - For .NET and Azure
- The MCP community for examples and support

---

## Author

Gora LEYE

[https://logcorner.com/](https://logcorner.com/)

For questions or support, please open an issue on GitHub.
