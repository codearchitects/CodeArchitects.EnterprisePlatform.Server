#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["NuGet.Config", "."]
COPY ["tests/Actors.Dapr.AspNetCore.IntegrationTests/ActorApp/ActorApp.csproj", "tests/Actors.Dapr.AspNetCore.IntegrationTests/ActorApp/"]
COPY ["src/Actors.Dapr/CodeArchitects.Platform.Actors.Dapr.csproj", "src/Actors.Dapr/"]
COPY ["src/Actors/CodeArchitects.Platform.Actors.csproj", "src/Actors/"]
COPY ["src/Actors.Analyzer/CodeArchitects.Platform.Actors.Analyzer.csproj", "src/Actors.Analyzer/"]
COPY ["src/Common/CodeArchitects.Platform.Common.csproj", "src/Common/"]
COPY ["src/Common.Analyzer/CodeArchitects.Platform.Common.Analyzer.csproj", "src/Common.Analyzer/"]
COPY ["src/Emit/CodeArchitects.Platform.Emit.csproj", "src/Emit/"]
RUN dotnet restore "tests/Actors.Dapr.AspNetCore.IntegrationTests/ActorApp/ActorApp.csproj"
COPY . .
WORKDIR "/src/tests/Actors.Dapr.AspNetCore.IntegrationTests/ActorApp"
RUN dotnet build "ActorApp.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ActorApp.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ActorApp.dll"]