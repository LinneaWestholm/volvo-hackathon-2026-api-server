@echo off
rem Starts the API server on http://localhost:5137. Works from any folder.
rem Press Ctrl+C in this window to stop it.
dotnet watch --project "%~dp0src\VolvoHackathon.Api"
pause
