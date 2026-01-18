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

