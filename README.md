# Code Architects Enterprise Solution Platform 7.0

[![License: Apache 2.0](https://img.shields.io/badge/License-Apache_2.0-blue.svg)](LICENSE)
[![CI](https://github.com/code-architects/CodeArchitects.Platform/actions/workflows/ci.yml/badge.svg)](https://github.com/code-architects/CodeArchitects.Platform/actions/workflows/ci.yml)

The **Code Architects Enterprise Solution Platform (ESP)** is a set of .NET libraries
for building distributed, cloud-native applications. It provides consistent building
blocks for messaging, state management, data access, actors and cross-cutting policies,
with first-class support for [Dapr](https://dapr.io/).

## Features

- **Messaging** — publish/subscribe and command abstractions, with a Dapr-backed implementation.
- **State management & Actors** — actor model and state abstractions, with Dapr integration.
- **Data access** — ADO.NET (SQL Server, PostgreSQL, MySQL, Oracle), Entity Framework Core and MongoDB providers.
- **Application layer** — application services, policies and cross-cutting infrastructure.
- **Mapping** — AutoMapper and Mapster integrations.
- **Analyzers** — Roslyn analyzers to enforce platform conventions.

## Requirements

- [.NET SDK 10.0](https://dotnet.microsoft.com/download) (targets `net7.0`, `net8.0`, `net9.0`, `net10.0` and `netstandard2.0/2.1`)

## Getting started

Packages are published to NuGet under the `CodeArchitects.Platform.*` namespace, for example:

```bash
dotnet add package CodeArchitects.Platform.Messaging
dotnet add package CodeArchitects.Platform.Messaging.Dapr.AspNetCore
```

See the [`samples/`](samples/) folder for runnable examples (`EFCoreSample`, `DaprSample`).

## Building from source

```bash
git clone https://github.com/code-architects/CodeArchitects.Platform.git
cd CodeArchitects.Platform
dotnet restore CodeArchitects.Platform.sln
dotnet build CodeArchitects.Platform.sln --configuration Release
```

Run the unit tests:

```bash
dotnet test CodeArchitects.Platform.sln --configuration Release
```

> Integration tests under `tests/*.IntegrationTests` require external services
> (databases, Dapr) and are excluded from the default CI run.

## Documentation

Documentation is built with [MkDocs](https://www.mkdocs.org/) from the [`docs/`](docs/) folder:

```bash
pip install mkdocs
mkdocs serve
```

## Contributing

Contributions are welcome! Please read [CONTRIBUTING.md](CONTRIBUTING.md) and our
[Code of Conduct](CODE_OF_CONDUCT.md) before opening an issue or pull request.

## Security

To report a security vulnerability, please follow the process in [SECURITY.md](SECURITY.md).

## License

Copyright © 2026 Code Architects S.r.l.

Licensed under the [Apache License, Version 2.0](LICENSE). See the [NOTICE](NOTICE) file
for attribution requirements.
