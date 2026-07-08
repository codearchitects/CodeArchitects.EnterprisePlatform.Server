# CodeArchitects.Platform.GraphQL

[![NuGet](https://img.shields.io/nuget/v/CodeArchitects.Platform.GraphQL.svg)](https://www.nuget.org/packages/CodeArchitects.Platform.GraphQL)
[![License: Apache 2.0](https://img.shields.io/badge/License-Apache_2.0-blue.svg)](../../LICENSE)

A **GraphQL document model** for the Code Architects Platform: a strongly-typed object model (AST) for
GraphQL documents, together with parsing/serialization helpers. It is transport- and server-agnostic
and underpins the platform's GraphQL client tooling.

## Installation

```bash
dotnet add package CodeArchitects.Platform.GraphQL
```

## What's inside

- A typed node model for GraphQL documents (`GraphDocument`, operations, selections, arguments,
  fragments, value nodes, type nodes).
- `DocumentSerializationOptions` and (de)serialization utilities.
- `InvalidGraphQLDocumentException` for malformed documents.

## Related packages

- [`CodeArchitects.Platform.GraphQL.ChilliCream`](../GraphQL.ChilliCream) — client execution over ChilliCream HotChocolate / StrawberryShake

## License

Licensed under the [Apache License 2.0](../../LICENSE).
