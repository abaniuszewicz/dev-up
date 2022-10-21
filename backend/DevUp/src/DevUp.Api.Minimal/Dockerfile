FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["src/DevUp.Api.Minimal/DevUp.Api.Minimal.csproj", "src/DevUp.Api.Minimal/"]
RUN dotnet restore "src/DevUp.Api.Minimal/DevUp.Api.Minimal.csproj"
COPY . .
WORKDIR "/src/src/DevUp.Api.Minimal"
RUN dotnet build "DevUp.Api.Minimal.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "DevUp.Api.Minimal.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DevUp.Api.Minimal.dll"]