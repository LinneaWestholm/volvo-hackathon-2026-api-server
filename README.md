# volvo-hackathon-2026-api-server
API server for the Volvo hackathon web app (`volvo-hackathon-2026`), built with ASP.NET Core minimal APIs (.NET 10).

The web app polls `GET /api/factory` and shows each summary in the Factory Monitoring chat.
Agents (or anything else) send summaries with `POST /api/factory`.

## Run

Requires the [.NET 10 SDK](https://dotnet.microsoft.com/download).

Double-click `Start.bat`, or from a terminal:

```powershell
cd volvo-hackathon-2026-api-server
dotnet watch --project src/VolvoHackathon.Api
```

The server listens on http://localhost:5137. Start it alongside the web app (in a second terminal); the web app is already configured to call it in `appsettings.Development.json`.

## Endpoints

| Method | Path           | Body                     | Result |
|--------|----------------|--------------------------|--------|
| GET    | `/api/factory` |                          | `{ "summary": "..." }`: the next waiting summary, or `""` when there is none |
| POST   | `/api/factory` | `{ "summary": "..." }`   | `202 Accepted`, or `400` if the summary is empty |

Each summary is returned by GET once, so the web app's fast poll loop doesn't repeat messages.
Up to 1000 summaries are kept while waiting; older ones are dropped.

Try them from VS Code with `src/VolvoHackathon.Api/VolvoHackathon.Api.http` (REST Client extension), or:

```powershell
Invoke-RestMethod http://localhost:5137/api/factory -Method Post -ContentType 'application/json' -Body '{"summary":"Line B stopped"}'
```

The API description (OpenAPI) is at http://localhost:5137/openapi/v1.json in development.

## Simulator

In development, `FactorySimulator` adds a sample summary every 2 seconds so the chat has something to show.
Turn it off or change the speed in `appsettings.Development.json`:

```json
"Simulator": { "Enabled": false, "IntervalMs": 2000 }
```

## Project layout

```
VolvoHackathon.Api.slnx
src/VolvoHackathon.Api/
  Program.cs                   Endpoints and service registration
  Models/FactoryResponse.cs    Shared shape with the web app; keep both copies in sync
  Services/
    FactoryMessageQueue.cs     Summaries waiting to be picked up
    FactorySimulator.cs        Fake data for development
```
