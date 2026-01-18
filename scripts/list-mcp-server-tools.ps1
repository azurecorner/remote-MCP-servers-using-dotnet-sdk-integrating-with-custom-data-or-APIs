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

# Parse Server-Sent Events (SSE) JSON payload
if ($content -match 'data:\s*(.+)') {
    $jsonData = $matches[1] | ConvertFrom-Json

    $jsonData.result.tools | ForEach-Object {
        Write-Host "`nTool: $($_.name)" -ForegroundColor Cyan
        Write-Host "Description: $($_.description)" -ForegroundColor Gray
        Write-Host "Input Schema:" -ForegroundColor Yellow
        $_.inputSchema | ConvertTo-Json -Depth 10 | Write-Host -ForegroundColor White
    }
}

Write-Host "Success!" -ForegroundColor Green