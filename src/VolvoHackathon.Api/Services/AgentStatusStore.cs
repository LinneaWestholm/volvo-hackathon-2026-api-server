namespace VolvoHackathon.Api.Services;

/// <summary>
/// Current running state of the agents, returned with every GET /api/factory.
/// Index 0 = Agent 1 … index 4 = Agent 5. All agents start as not running.
/// </summary>
public class AgentStatusStore
{
    public const int AgentCount = 5;

    private readonly Lock _lock = new();
    private readonly bool[] _running = new bool[AgentCount];

    public bool[] GetAll()
    {
        lock (_lock) return (bool[])_running.Clone();
    }

    public void SetAll(bool[] running)
    {
        if (running.Length != AgentCount)
            throw new ArgumentException($"Expected {AgentCount} values, got {running.Length}.", nameof(running));

        lock (_lock) running.CopyTo(_running, 0);
    }

    public void Set(int index, bool running)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(index);
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(index, AgentCount);

        lock (_lock) _running[index] = running;
    }
}
