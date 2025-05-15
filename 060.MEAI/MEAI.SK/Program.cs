#pragma warning disable SKEXP0001
using Azure.AI.OpenAI;
using Azure.Identity;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.ChatCompletion;
using System.ComponentModel;

var aoaiClient = new AzureOpenAIClient(
    new("https://ai-kaotaeastus2.openai.azure.com/"),
    new AzureCliCredential());

var aoaiChatClient = aoaiClient.GetChatClient("gpt-4.1");

IChatClient chatClient = aoaiChatClient
    .AsIChatClient()
    .AsBuilder()
    .UseFunctionInvocation()
    .Build();

var builder = Kernel.CreateBuilder();
builder.Services.AddSingleton(chatClient.AsChatCompletionService());

// Microsoft.Extensions.AI の関数をプラグインとして登録
builder.Plugins.AddFromFunctions(nameof(WeatherForecastPlugin),
    [
        AIFunctionFactory.Create(WeatherForecastPlugin.GetWeather).AsKernelFunction(),
    ]);

// クラスを Plugin として登録 (通常の Semantic Kernel の使い方)
builder.Plugins.AddFromType<SystemPlugins>();

var kernel = builder.Build();

var catAgent = new ChatCompletionAgent
{
    Name = "CatAgent",
    Instructions = """
        あなたはチュールが大好きな猫型エージェントです。
        猫らしく振舞うために語尾は「にゃん」にしてください。
        """,
    Kernel = kernel,
    Arguments = new(new PromptExecutionSettings
    {
        FunctionChoiceBehavior = FunctionChoiceBehavior.Auto(),
    }),
};

AgentThread? thread = null;
while (true)
{
    Console.Write("User: ");
    var userInput = Console.ReadLine();
    if (string.IsNullOrEmpty(userInput)) break;
    var chatResponse = await catAgent.InvokeAsync(userInput, thread).FirstAsync();
    thread = chatResponse.Thread;
    Console.WriteLine($"CatAgent: {chatResponse.Message.Content}");
}

public static class WeatherForecastPlugin
{
    [Description("指定した日付と場所の天気を取得します。")]
    public static string GetWeather(
        [Description("場所")]
        string location,
        [Description("日付")]
        DateTimeOffset date) => 
        $"指定した場所 {location} の {date:yyyy/MM/dd} の天気は晴れです。";
}


public class SystemPlugins
{
    [KernelFunction]
    [Description("今日の日付を取得します。")]
    public static DateTimeOffset GetToday() => TimeProvider.System.GetLocalNow();

    [KernelFunction]
    [Description("現在地を取得します。")]
    public static string GetCurrentLocation() => "品川";
}

