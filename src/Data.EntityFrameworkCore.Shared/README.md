# CodeArchitects.Platform.Data.EntityFrameworkCore.Shared

[![NuGet](https://img.shields.io/nuget/v/CodeArchitects.Platform.Data.EntityFrameworkCore.Shared.svg)](https://www.nuget.org/packages/CodeArchitects.Platform.Data.EntityFrameworkCore.Shared)
[![License: Apache 2.0](https://img.shields.io/badge/License-Apache_2.0-blue.svg)](../../LICENSE)

Shared Entity Framework Core building blocks used by both the EF Core 7+
([`CodeArchitects.Platform.Data.EntityFrameworkCore`](../Data.EntityFrameworkCore)) and EF Core 5
([`CodeArchitects.Platform.Data.EntityFrameworkCore5`](../Data.EntityFrameworkCore5)) Data Access
Layer implementations. It contains the version-independent repository base classes and plugin
infrastructure.

> This is a shared support package. In most cases you reference one of the concrete EF Core packages,
> which bring this in transitively.

## What's inside

| Type | Purpose |
|------|---------|
| `EFCoreRepository<TEntity, TKey>` / `EFCoreMappedRepository<…>` | Repository base classes. |
| `IDataContext` | EF Core-backed generic data context. |
| `ICaepExtensionPlugin`, `IPluginServiceCollection`, `ICaepOptionsBuilderInfrastructure` | Plugin/extension infrastructure (soft delete, multitenancy, interceptors). |

## Related packages

- [`CodeArchitects.Platform.Data.EntityFrameworkCore`](../Data.EntityFrameworkCore)
- [`CodeArchitects.Platform.Data.EntityFrameworkCore5`](../Data.EntityFrameworkCore5)

## License

Licensed under the [Apache License 2.0](../../LICENSE).
