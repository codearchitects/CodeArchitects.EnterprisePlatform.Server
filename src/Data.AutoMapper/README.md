# CodeArchitects.Platform.Data.AutoMapper

[![NuGet](https://img.shields.io/nuget/v/CodeArchitects.Platform.Data.AutoMapper.svg)](https://www.nuget.org/packages/CodeArchitects.Platform.Data.AutoMapper)
[![License: Apache 2.0](https://img.shields.io/badge/License-Apache_2.0-blue.svg)](../../LICENSE)

[AutoMapper](https://automapper.org/) integration for the Code Architects Platform Data Access Layer,
used to map between table models and domain entities (e.g. in mapped repositories).

> ⚠️ **Maintenance mode.** This package is kept for applications that have not yet migrated off
> AutoMapper. New projects should use [`CodeArchitects.Platform.Data.Mapster`](../Data.Mapster).
> The package pins AutoMapper 14.x, which is affected by advisory
> [GHSA-rvv3-g6hj-g44x](https://github.com/advisories/GHSA-rvv3-g6hj-g44x); the fix is only available
> in the commercial AutoMapper 15/16 majors. See [SECURITY.md](../../SECURITY.md) for the
> accepted-risk rationale.

## Installation

```bash
dotnet add package CodeArchitects.Platform.Data.AutoMapper
```

## Related packages

- [`CodeArchitects.Platform.Data.Mapster`](../Data.Mapster) — recommended mapping integration
- [`CodeArchitects.Platform.Data`](../Data) — DAL abstractions

## License

Licensed under the [Apache License 2.0](../../LICENSE).
