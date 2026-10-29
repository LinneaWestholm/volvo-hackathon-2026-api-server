using System.Collections.Concurrent;

namespace VolvoHackathon.Api.Services;

/// <summary>
/// Summaries waiting to be picked up by the web app. Each GET /api/factory takes the oldest one,
/// so every summary is delivered once and the web app's fast poll loop doesn't repeat messages.
/// </summary>
public class FactoryMessageQueue
{
    // Oldest summaries are dropped past this, so the queue can't grow forever while the web app is offline
    private const int MaxQueued = 1000;

    private readonly ConcurrentQueue<string> _summaries = new();

    public void Enqueue(string summary)
    {
        _summaries.Enqueue(summary);
        while (_summaries.Count > MaxQueued && _summaries.TryDequeue(out _)) { }
    }

    public bool TryDequeue(out string summary) => _summaries.TryDequeue(out summary!);
}
