using ModelContextProtocol.Server;
using System.ComponentModel;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.Services.AddMcpServer()
    .WithHttpTransport()
    .WithToolsFromAssembly();

var app = builder.Build();

app.MapDefaultEndpoints();
app.UseHttpsRedirection();
app.MapMcp();

app.Run();

[McpServerToolType]
class WeatherForecastTools
{
    [McpServerTool]
    [Description("指定した日付と場所の天気を取得します。")]
    public string GetWeather(
        [Description("場所")]
        string location,
        [Description("日付")]
        DateTimeOffset date) =>
        $"指定した場所 {location} の {date:yyyy/MM/dd} の天気は晴れです。";
}
