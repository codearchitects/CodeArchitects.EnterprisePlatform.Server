# CodeArchitects.Platform.Data.Mapster

[![NuGet](https://img.shields.io/nuget/v/CodeArchitects.Platform.Data.Mapster.svg)](https://www.nuget.org/packages/CodeArchitects.Platform.Data.Mapster)
[![License: Apache 2.0](https://img.shields.io/badge/License-Apache_2.0-blue.svg)](../../LICENSE)

[Mapster](https://github.com/MapsterMapper/Mapster) integration for the Code Architects Platform Data
Access Layer, used to map between database/table models and domain entities (for example in mapped
repositories). This is the **recommended mapping integration** for new projects.

## Installation

```bash
dotnet add package CodeArchitects.Platform.Data.Mapster
```

## Features

- `MapsterTracking` and `TrackingMappingExpressionExtensions` — change-tracking-aware mappings so that
  mapping table records to entities plays well with the ORM's tracking.

## Related packages

- [`CodeArchitects.Platform.Data`](../Data) — DAL abstractions
- [`CodeArchitects.Platform.Data.AutoMapper`](../Data.AutoMapper) — legacy AutoMapper integration (maintenance only)

## License

Licensed under the [Apache License 2.0](../../LICENSE).
