var builder = DistributedApplication.CreateBuilder(args);

var ollama = builder.AddOllama("ollama")
    .WithDataVolume();
var chat = ollama.AddModel("chat", "llama3.2");
var embeddings = ollama.AddModel("embeddings", "all-minilm");

var ingestionCache = builder.AddSqlite("ingestionCache");

var webApp = builder.AddProject<Projects.ChatApp1_Web>("aichatweb-app");
webApp
    .WithReference(chat)
    .WithReference(embeddings)
    .WaitFor(chat)
    .WaitFor(embeddings);
webApp
    .WithReference(ingestionCache)
    .WaitFor(ingestionCache);

builder.Build().Run();
