namespace VolvoHackathon.Api.Models;

/// <summary>
/// Returned by GET /api/factory and accepted by POST /api/factory.
/// Keep in sync with VolvoHackathon.Models.FactoryResponse in the web app.
/// </summary>
public class FactoryResponse
{
    public string Summary { get; set; } = "";

    /// <summary>Whether each agent is running. Index 0 = Agent 1 … index 4 = Agent 5.</summary>
    public bool[] AgentsRunning { get; set; } = new bool[5];
}
