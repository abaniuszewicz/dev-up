cmd /c dotnet build ..\DevUp\DevUp.sln
forfiles /p ..\DevUp /M *.nupkg /S /C "cmd /c nuget add @PATH -Source %~dp0"