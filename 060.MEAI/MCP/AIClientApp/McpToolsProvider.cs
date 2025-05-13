using Microsoft.Extensions.AI;
using ModelContextProtocol.Client;
using ModelContextProtocol.Protocol.Transport;

namespace AIClientApp;

public class McpToolsProvider(HttpClient client) : IAsyncDisposable
{
    private IMcpClient? _mcpClient;

    public async ValueTask DisposeAsync()
    {
        if (_mcpClient != null)
        {
            await _mcpClient.DisposeAsync();
            _mcpClient = null;
        }
    }

    public async Task<IEnumerable<AITool>> GetToolsAsync()
    {
        if (_mcpClient == null)
        {
            _mcpClient = await McpClientFactory.CreateAsync(
                new SseClientTransport(transportOptions: new()
                {
                    Endpoint = new("https://mcpserver/sse"),
                },
                client));
        }

        return await _mcpClient.ListToolsAsync();
    }
}
