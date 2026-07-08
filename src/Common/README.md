# CodeArchitects.Platform.Common

[![NuGet](https://img.shields.io/nuget/v/CodeArchitects.Platform.Common.svg)](https://www.nuget.org/packages/CodeArchitects.Platform.Common)
[![License: Apache 2.0](https://img.shields.io/badge/License-Apache_2.0-blue.svg)](../../LICENSE)

Core abstractions shared across the Code Architects Enterprise Solution Platform. This is the
foundational package most other platform libraries depend on. It contains no infrastructure or
provider-specific code — only technology-agnostic contracts and helpers.

## Installation

```bash
dotnet add package CodeArchitects.Platform.Common
```

## Key abstractions

| Type | Purpose |
|------|---------|
| `IServiceResolver<T>` | Resolves a named instance of a service `T` at runtime (e.g. one of several message buses or state stores). |
| `IIdentityProfile` | Represents the identity of the current caller (user id, claims). |
| `ITenantProfile` | Represents the current tenant in multi-tenant scenarios. |
| `Page<T>` | A page of results with paging metadata, used by paged queries. |

## Usage

Resolve a named service instance:

```csharp
public class Publisher
{
    private readonly IMessageBus _bus;

    public Publisher(IServiceResolver<IMessageBus> busResolver)
    {
        _bus = busResolver.Resolve("busName");
    }
}
```

Access the current identity/tenant:

```csharp
public class MyService
{
    public MyService(IIdentityProfile identity, ITenantProfile tenant)
    {
        var userId = identity.Id;
        var tenantId = tenant.Id;
    }
}
```

## Related packages

- [`CodeArchitects.Platform.Application`](../Application) — application-layer services built on these abstractions
- [`CodeArchitects.Platform.Messaging`](../Messaging) — messaging abstractions
- [`CodeArchitects.Platform.Data`](../Data) — data-access abstractions

## License

Licensed under the [Apache License 2.0](../../LICENSE).
