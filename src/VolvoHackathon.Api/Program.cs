using Microsoft.AspNetCore.Mvc;
using VolvoHackathon.Api.Models;
using VolvoHackathon.Api.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddSingleton<FactoryMessageQueue>();
builder.Services.AddSingleton<AgentStatusStore>();
builder.Services.AddHostedService<FactorySimulator>(); // fake data, enabled in appsettings.Development.json

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi(); // API description at /openapi/v1.json
}

// No HTTPS redirect: the web app calls this over plain http on localhost.

// The web app polls this. Returns the next summary (empty when nothing is waiting)
// and the current running state of every agent.
app.MapGet("/api/factory", (FactoryMessageQueue queue, AgentStatusStore agents) =>
    new FactoryResponse
    {
        Summary = queue.TryDequeue(out var summary) ? summary : "",
        AgentsRunning = agents.GetAll(),
    })
.WithName("GetFactoryResponse");

// Agents (or anything else) post summaries here to show them in the web app's chat.
app.MapPost("/api/factory", (FactoryResponse response, FactoryMessageQueue queue) =>
{
    if (string.IsNullOrWhiteSpace(response.Summary))
    {
        return Results.BadRequest("Summary is required.");
    }

    queue.Enqueue(response.Summary);
    return Results.Accepted();
})
.WithName("PostFactoryResponse");

// Set whether all agents are running, e.g. [true, true, false, true, true]
app.MapPut("/api/factory/agents", ([FromBody] bool[] running, AgentStatusStore agents) =>
{
    if (running.Length != AgentStatusStore.AgentCount)
    {
        return Results.BadRequest($"Expected {AgentStatusStore.AgentCount} values.");
    }

    agents.SetAll(running);
    return Results.NoContent();
})
.WithName("SetAgentsRunning");

// Set whether one agent is running. Index 0 = Agent 1 … 4 = Agent 5. Body: true or false
app.MapPut("/api/factory/agents/{index:int}", (int index, [FromBody] bool running, AgentStatusStore agents) =>
{
    if (index < 0 || index >= AgentStatusStore.AgentCount)
    {
        return Results.NotFound($"Agent index must be 0-{AgentStatusStore.AgentCount - 1}.");
    }

    agents.Set(index, running);
    return Results.NoContent();
})
.WithName("SetAgentRunning");

app.Run();
