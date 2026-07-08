# CodeArchitects.Platform.Data.EntityFrameworkCore5

[![NuGet](https://img.shields.io/nuget/v/CodeArchitects.Platform.Data.EntityFrameworkCore5.svg)](https://www.nuget.org/packages/CodeArchitects.Platform.Data.EntityFrameworkCore5)
[![License: Apache 2.0](https://img.shields.io/badge/License-Apache_2.0-blue.svg)](../../LICENSE)

An **Entity Framework Core 5** implementation of the [`CodeArchitects.Platform.Data`](../Data) Data
Access Layer, for applications that must target **.NET Core 3.1 or .NET 5**. It mirrors the API of
[`CodeArchitects.Platform.Data.EntityFrameworkCore`](../Data.EntityFrameworkCore) (EF Core 7+).

> New applications on .NET 6+ should prefer
> [`CodeArchitects.Platform.Data.EntityFrameworkCore`](../Data.EntityFrameworkCore).

## Installation

```bash
dotnet add package CodeArchitects.Platform.Data.EntityFrameworkCore5
```

## Usage

Same repository model as the EF Core 7+ package — derive from `EFCoreRepository<TEntity, TKey>` and
use the protected `Entities` property. See the
[EF Core package README](../Data.EntityFrameworkCore) for details.

## Related packages

- [`CodeArchitects.Platform.Data.EntityFrameworkCore5.DependencyInjection`](../Data.EntityFrameworkCore5.DependencyInjection) — DI registration
- [`CodeArchitects.Platform.Data`](../Data) — DAL abstractions

## License

Licensed under the [Apache License 2.0](../../LICENSE).
