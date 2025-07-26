FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Avora.Agent.Api/Avora.Agent.Api.csproj", "Avora.Agent.Api/"]
RUN dotnet restore "Avora.Agent.Api/Avora.Agent.Api.csproj"
COPY . .
WORKDIR "/src/Avora.Agent.Api"
RUN dotnet build "./Avora.Agent.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Avora.Agent.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Avora.Agent.Api.dll"]
