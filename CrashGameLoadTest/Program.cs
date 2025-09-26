using CrashGameLoadTest.Engine;
using CrashGameLoadTest.Scenarios;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("logs/loadtest-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

try
{
    var configuration = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
        .AddCommandLine(args)
        .Build();

    var services = new ServiceCollection();
    services.AddSingleton<IConfiguration>(configuration);

    // Get configuration values
    var integratorUrl = configuration["IntegratorUrl"] ?? "https://api.example.com";
    var hubUrl = configuration["HubUrl"] ?? "https://hub.example.com/crash";
    var username = configuration["Username"] ?? "testuser";
    var password = configuration["Password"] ?? "testpass";
    var playerCount = int.Parse(configuration["PlayerCount"] ?? "10");
    var scenarioType = configuration["ScenarioType"] ?? "basic";
    var duration = int.Parse(configuration["DurationMinutes"] ?? "5");

    // Create scenario builder and engine
    var scenarioBuilder = new ScenarioBuilder(integratorUrl, hubUrl, username, password);
    var engine = new LoadTestEngine(scenarioBuilder);

    // Create cancellation token for duration
    using var cts = new CancellationTokenSource(TimeSpan.FromMinutes(duration));

    // Run the load test
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