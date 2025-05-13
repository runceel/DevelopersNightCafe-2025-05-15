var builder = DistributedApplication.CreateBuilder(args);

var openai = builder.AddConnectionString("openai");

var mcpServer = builder.AddProject<Projects.WeatherForecastMCPServer>("mcpserver");

builder.AddProject<Projects.AIClientApp>("aiclientapp")
    .WithReference(openai)
    .WithReference(mcpServer)
    .WaitFor(mcpServer);

builder.Build().Run();
