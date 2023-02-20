#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["NuGet.Config", "."]
COPY ["tests/Messaging.Dapr.AspNetCore.IntegrationTests/Publisher/Publisher.csproj", "tests/Messaging.Dapr.AspNetCore.IntegrationTests/Publisher/"]
COPY ["src/Messaging.Dapr.AspNetCore/CodeArchitects.Platform.Messaging.Dapr.AspNetCore.csproj", "src/Messaging.Dapr.AspNetCore/"]
COPY ["src/Dapr.AspNetCore/CodeArchitects.Platform.Dapr.AspNetCore.csproj", "src/Dapr.AspNetCore/"]
COPY ["src/Common/CodeArchitects.Platform.Common.csproj", "src/Common/"]
COPY ["src/Common.Analyzer/CodeArchitects.Platform.Common.Analyzer.csproj", "src/Common.Analyzer/"]
COPY ["src/Messaging.AspNetCore/CodeArchitects.Platform.Messaging.AspNetCore.csproj", "src/Messaging.AspNetCore/"]
COPY ["src/Messaging/CodeArchitects.Platform.Messaging.csproj", "src/Messaging/"]
COPY ["src/Messaging.Dapr/CodeArchitects.Platform.Messaging.Dapr.csproj", "src/Messaging.Dapr/"]
RUN dotnet restore "tests/Messaging.Dapr.AspNetCore.IntegrationTests/Publisher/Publisher.csproj"
COPY . .
WORKDIR "/src/tests/Messaging.Dapr.AspNetCore.IntegrationTests/Publisher"
RUN dotnet build "Publisher.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Publisher.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Publisher.dll"]