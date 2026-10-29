namespace VolvoHackathon.Api.Services;

/// <summary>
/// Fake factory activity for testing until real agents are connected.
/// Every few seconds it adds a sample summary and randomly starts or stops one agent.
/// Turn it off with "Simulator:Enabled": false.
/// </summary>
public class FactorySimulator(FactoryMessageQueue queue, AgentStatusStore agents, IConfiguration config) : BackgroundService
{
    private static readonly string[] SampleSummaries =
    [
        "Agent 1: All stations on line A report ready.",
        "Agent 2: Bolt supply at station 4 below 20%.",
        "Agent 3: Restock scheduled for station 4.",
        "Agent 4: Quality scan on batch 118 passed.",
        "Agent 5: Robot arm 2 temperature slightly elevated.",
    ];

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (!config.GetValue("Simulator:Enabled", false)) return;

        var interval = TimeSpan.FromMilliseconds(config.GetValue("Simulator:IntervalMs", 2000));
        var i = 0;

        agents.SetAll([true, true, true, true, true]);

        try
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(interval, stoppingToken);

                queue.Enqueue(SampleSummaries[i++ % SampleSummaries.Length]);

                // Mostly running, sometimes stopped, so both icons show up
                agents.Set(Random.Shared.Next(AgentStatusStore.AgentCount), Random.Shared.NextDouble() < 0.7);
            }
        }
        catch (OperationCanceledException)
        {
            // app is shutting down
        }
    }
}
