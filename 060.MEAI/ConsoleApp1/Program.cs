
using Azure.AI.OpenAI;
using Azure.Identity;
using Microsoft.Extensions.AI;

var aoaiClient = new AzureOpenAIClient(
    new("https://ai-kaotaeastus2.openai.azure.com/"),
    new AzureCliCredential());

var aoaiChatClient = aoaiClient.GetChatClient("gpt-4.1");

IChatClient chatClient = aoaiChatClient
    .AsIChatClient()
    .AsBuilder()
    .UseFunctionInvocation()
    .Build();

var data = await File.ReadAllBytesAsync("test.png");

var chatResponse = await chatClient.GetResponseAsync(new ChatMessage(ChatRole.User,
    [
        new TextContent("この画像ってなに？"),
        new DataContent(data, "image/png"),
    ]));
Console.WriteLine(chatResponse.Text);
