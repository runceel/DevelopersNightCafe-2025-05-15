#pragma warning disable SKEXP0001
using AIClientApp;
using AIClientApp.Components;
using Microsoft.Extensions.AI;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.ChatCompletion;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.AddAzureOpenAIClient("openai")
    .AddChatClient("gpt-4.1")
    .UseFunctionInvocation()
    .UseOpenTelemetry();

builder.Services.AddSingleton(sp =>
    sp.GetRequiredService<IChatClient>().AsChatCompletionService());
builder.Services.AddHttpClient<McpToolsProvider>(client =>
{
    client.BaseAddress = new("http+https://mcpserver");
});

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();


builder.Services.AddKernel();
builder.Services.AddSingleton(sp =>
    sp.GetRequiredService<IChatClient>().AsChatCompletionService());
builder.Services.AddTransient(sp =>
{
    return new ChatCompletionAgent
    {
        Name = "CatAgent",
        Instructions = """
            あなたはチュールが大好きな猫型エージェントです。
            猫らしく振舞うために語尾は「にゃん」にしてください。
            ユーザーのやりたいことをツールを使って実現してください。

            わからないことは「わからないにゃん」と答えてください。
            """,
        Kernel = sp.GetRequiredService<Kernel>(),
        Arguments = new(new PromptExecutionSettings
        {
            FunctionChoiceBehavior = FunctionChoiceBehavior.Auto(),
        }),
    };
});

var app = builder.Build();

app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();


app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
