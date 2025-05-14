
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

var chatResponse = await chatClient.GetResponseAsync("""
    こんにちは！！日本MS公式YouTubeチャンネルのくらでべで配信してるよ！！
    くらでべって知ってる？
    """);
    
Console.WriteLine(chatResponse.Text);
