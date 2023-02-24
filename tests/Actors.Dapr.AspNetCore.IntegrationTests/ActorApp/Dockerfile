#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["NuGet.Config", "."]
COPY ["tests/Actors.Dapr.AspNetCore.IntegrationTests/ActorApp/ActorApp.csproj", "tests/Actors.Dapr.AspNetCore.IntegrationTests/ActorApp/"]
COPY ["src/Actors.Dapr.AspNetCore/CodeArchitects.Platform.Actors.Dapr.AspNetCore.csproj", "src/Actors.Dapr.AspNetCore/"]
COPY ["src/Actors.Dapr/CodeArchitects.Platform.Actors.Dapr.csproj", "src/Actors.Dapr/"]
COPY ["src/Actors/CodeArchitects.Platform.Actors.csproj", "src/Actors/"]
COPY ["src/Actors.Analyzer/CodeArchitects.Platform.Actors.Analyzer.csproj", "src/Actors.Analyzer/"]
COPY ["src/Common/CodeArchitects.Platform.Common.csproj", "src/Common/"]
COPY ["src/Common.Analyzer/CodeArchitects.Platform.Common.Analyzer.csproj", "src/Common.Analyzer/"]
COPY ["src/Emit/CodeArchitects.Platform.Emit.csproj", "src/Emit/"]
COPY ["src/Dapr.AspNetCore/CodeArchitects.Platform.Dapr.AspNetCore.csproj", "src/Dapr.AspNetCore/"]
COPY ["src/Messaging.Dapr.AspNetCore/CodeArchitects.Platform.Messaging.Dapr.AspNetCore.csproj", "src/Messaging.Dapr.AspNetCore/"]
COPY ["src/Messaging.AspNetCore/CodeArchitects.Platform.Messaging.AspNetCore.csproj", "src/Messaging.AspNetCore/"]
COPY ["src/Messaging/CodeArchitects.Platform.Messaging.csproj", "src/Messaging/"]
COPY ["src/Messaging.Dapr/CodeArchitects.Platform.Messaging.Dapr.csproj", "src/Messaging.Dapr/"]
RUN dotnet restore "tests/Actors.Dapr.AspNetCore.IntegrationTests/ActorApp/ActorApp.csproj"
COPY . .
WORKDIR "/src/tests/Actors.Dapr.AspNetCore.IntegrationTests/ActorApp"
RUN dotnet build "ActorApp.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ActorApp.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ActorApp.dll"]