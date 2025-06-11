#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

ARG TARGET_FRAMEWORK=${TARGET_FRAMEWORK}
ARG LATEST_FRAMEWORK=${LATEST_FRAMEWORK}

FROM mcr.microsoft.com/dotnet/aspnet:${TARGET_FRAMEWORK} AS base
ARG HTTP_PORT=${HTTP_PORT}
WORKDIR /app
EXPOSE ${HTTP_PORT}

FROM mcr.microsoft.com/dotnet/sdk:${LATEST_FRAMEWORK} AS build
ARG TARGET_FRAMEWORK=${TARGET_FRAMEWORK}
WORKDIR /src
COPY ["NuGet.Config", "."]
COPY ["tests/Actors.Dapr.AspNetCore.IntegrationTests/ActorApp/ActorApp.csproj", "tests/Actors.Dapr.AspNetCore.IntegrationTests/ActorApp/"]
RUN dotnet restore "tests/Actors.Dapr.AspNetCore.IntegrationTests/ActorApp/ActorApp.csproj"
COPY . .
WORKDIR "/src/tests/Actors.Dapr.AspNetCore.IntegrationTests/ActorApp"
RUN dotnet build --framework net${TARGET_FRAMEWORK} "ActorApp.csproj" -c Release -o /app/build

FROM build AS publish
ARG TARGET_FRAMEWORK=${TARGET_FRAMEWORK}
RUN dotnet publish --framework net${TARGET_FRAMEWORK} "ActorApp.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ActorApp.dll"]