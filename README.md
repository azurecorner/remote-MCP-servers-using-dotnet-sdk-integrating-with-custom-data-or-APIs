# Remote MCP Servers using .NET SDK - Integrating with custom data and APIs

A comprehensive example demonstrating how to build **Model Context Protocol (MCP)** servers using the .NET SDK, showcasing integration with custom data sources and APIs. This project implements a weather forecast service as an example of how to expose your own APIs through MCP tools.

## Table of Contents

- [Overview](#overview)
- [What is MCP?](#what-is-mcp)
- [Features](#features)
- [Architecture](#architecture)
- [Prerequisites](#prerequisites)
- [Project Structure](#project-structure)
- [Getting Started](#getting-started)
- [Configuration](#configuration)
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
- Visual Studio 2022 / VS Code / any compatible editor supporting the MCP extension
- An MCP-compatible client (e.g., Kiro, Claude Desktop, VS Code with MCP extension)

## Project Structure

```text
remote-MCP-servers-using-dotnet-sdk-integrating-with-our-own-data-or-apis/
├── src/
│   └── McpServer/
│       ├── McpServer/
│       │   ├── Program.cs              # Application entry point
│       │   ├── appsettings.json        # Configuration settings
│       │   ├── Tools/                  # MCP tool implementations
│       │   │   ├── PingTool.cs
│       │   │   └── WeatherTool.cs
│       │   └── Services/               # Business logic services
│       │       └── WeatherService.cs
│       └── McpServer.sln
├── docs/                               # Additional documentation
├── README.md                           # This file
└── LICENSE
```

## Getting Started

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

## Configuration

### Configuring MCP Clients

#### VS Code Copilot

Add to your VS Code settings: .\remote-MCP-servers-using-dotnet-sdk-integrating-with-our-own-data-or-apis\.vscode\mcp.json

```json
{
  "mcp.servers": {
    "local-mcp-server": {
      "url": "http://localhost:8081/mcp",
      "type": "http"
    }
  }
}
```

## Available MCP Tools

bash =>

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

powershell =>

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

## Usage


### Direct API Testing

#### Test Weather

bash =>

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


powershell

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

---

