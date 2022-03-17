FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["DevUp.Api/DevUp.Api.csproj", "DevUp.Api/"]
RUN dotnet restore "DevUp.Api/DevUp.Api.csproj"
COPY . .
WORKDIR "/src/DevUp.Api"
RUN dotnet build "DevUp.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "DevUp.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DevUp.Api.dll"]