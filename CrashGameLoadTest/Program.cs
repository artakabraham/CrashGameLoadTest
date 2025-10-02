using CrashGameLoadTest.Engine;
using CrashGameLoadTest.Extensions;
using CrashGameLoadTest.Interfaces;
using CrashGameLoadTest.Scenarios;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("logs/loadtest-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

var builder = Host.CreateApplicationBuilder(args);

builder.Services.RegisterServices(builder.Configuration);

var app = builder.Build();

try
{
    var playerCount = 5;
    var scenarioType = "basic";
    var duration = 5;

    Console.WriteLine($"Starting load test with {playerCount} players for {duration} minutes");
    Console.WriteLine($"Scenario: {scenarioType}");

    // Create scenario builder
    var scenarioBuilder = new ScenarioBuilder();
    var playerFactory = app.Services.GetRequiredService<IPlayerFactory>();
    var engine = new LoadTestEngine(scenarioBuilder, playerFactory);

    using var cts = new CancellationTokenSource(TimeSpan.FromMinutes(duration));

    await engine.RunAsync(playerCount, scenarioType, cts.Token);

    Log.Information("Load test completed successfully");
}
catch (Exception ex)
{
    Log.Fatal(ex, "Load test failed");
    Environment.Exit(1);
}
finally
{
    Log.CloseAndFlush();
}