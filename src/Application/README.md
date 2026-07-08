# CodeArchitects.Platform.Application

[![NuGet](https://img.shields.io/nuget/v/CodeArchitects.Platform.Application.svg)](https://www.nuget.org/packages/CodeArchitects.Platform.Application)
[![License: Apache 2.0](https://img.shields.io/badge/License-Apache_2.0-blue.svg)](../../LICENSE)

Application-layer building blocks for Code Architects Platform services: identity profile wiring,
request/remoting helpers and SignalR hub registration. It builds on the abstractions defined in
[`CodeArchitects.Platform.Common`](../Common).

## Installation

```bash
dotnet add package CodeArchitects.Platform.Application
```

## Features

- **Identity profile** — `ClaimsIdentityProfile` implements `IIdentityProfile` on top of the
  ASP.NET Core `ClaimsPrincipal`, exposing the authenticated caller to your services.
- **SignalR hubs** — `AddHubs` registers strongly-typed SignalR hubs.
- **Remoting helpers** — utilities for building and consuming HTTP query strings and requests.

## Usage

Register the claims-based identity profile in `Program.cs`:

```csharp
builder.Services.AddIdentityProfile();
```

Then inject `IIdentityProfile` anywhere:

```csharp
public class OrderService
{
    private readonly IIdentityProfile _identity;

    public OrderService(IIdentityProfile identity) => _identity = identity;
}
```

Register SignalR hubs:

```csharp
builder.Services
    .AddSignalR()
    .AddHubs();
```

## Related packages

- [`CodeArchitects.Platform.Common`](../Common) — core abstractions (`IIdentityProfile`, `ITenantProfile`)

## License

Licensed under the [Apache License 2.0](../../LICENSE).
