# CodeArchitects.Platform.Messaging.AspNetCore

[![NuGet](https://img.shields.io/nuget/v/CodeArchitects.Platform.Messaging.AspNetCore.svg)](https://www.nuget.org/packages/CodeArchitects.Platform.Messaging.AspNetCore)
[![License: Apache 2.0](https://img.shields.io/badge/License-Apache_2.0-blue.svg)](../../LICENSE)

ASP.NET Core hosting support for [`CodeArchitects.Platform.Messaging`](../Messaging): configuration
binding and the plumbing needed to expose message handlers as HTTP endpoints. It is used together
with a concrete transport such as [`CodeArchitects.Platform.Messaging.Dapr.AspNetCore`](../Messaging.Dapr.AspNetCore).

## Installation

```bash
dotnet add package CodeArchitects.Platform.Messaging.AspNetCore
```

## Features

- Binds the `Messaging` configuration section (`MessagingConfig`, handler/output-binding configs).
- Provides the shared infrastructure for mapping handlers to endpoints in an ASP.NET Core app.

## Configuration

```json
{
  "Messaging": {
    "DefaultBus": "pubsub"
  }
}
```

## Related packages

- [`CodeArchitects.Platform.Messaging`](../Messaging) — core messaging abstractions
- [`CodeArchitects.Platform.Messaging.Dapr.AspNetCore`](../Messaging.Dapr.AspNetCore) — Dapr transport + endpoint mapping

## License

Licensed under the [Apache License 2.0](../../LICENSE).
