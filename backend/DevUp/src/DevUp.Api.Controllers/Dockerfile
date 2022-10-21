FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["src/DevUp.Api.Controllers/DevUp.Api.Controllers.csproj", "src/DevUp.Api.Controllers/"]
RUN dotnet restore "src/DevUp.Api.Controllers/DevUp.Api.Controllers.csproj"
COPY . .
WORKDIR "/src/src/DevUp.Api.Controllers"
RUN dotnet build "DevUp.Api.Controllers.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "DevUp.Api.Controllers.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DevUp.Api.Controllers.dll"]