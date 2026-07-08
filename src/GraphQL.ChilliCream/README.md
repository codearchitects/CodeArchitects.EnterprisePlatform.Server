# CodeArchitects.Platform.GraphQL.ChilliCream

[![NuGet](https://img.shields.io/nuget/v/CodeArchitects.Platform.GraphQL.ChilliCream.svg)](https://www.nuget.org/packages/CodeArchitects.Platform.GraphQL.ChilliCream)
[![License: Apache 2.0](https://img.shields.io/badge/License-Apache_2.0-blue.svg)](../../LICENSE)

GraphQL **client execution** built on the [ChilliCream](https://chillicream.com/) stack (HotChocolate
/ StrawberryShake). It turns the platform's [GraphQL document model](../GraphQL) into executable,
strongly-typed requests and extracts variables from typed inputs.

## Installation

```bash
dotnet add package CodeArchitects.Platform.GraphQL.ChilliCream
```

## What's inside

| Type | Purpose |
|------|---------|
| `IOperationExecutorProvider` / `OperationExecutorProvider` | Resolves an executor for a result type. |
| `IGraphRequest<TResult>` / `IGraphRequest<TResult, TVariables>` | A ready-to-run GraphQL request. |
| `RequestFactory` | Builds requests from documents. |
| `VariableExtractor<TVariables>` | Extracts GraphQL variables from a typed input object. |

## Usage

```csharp
IGraphRequest<TResult> request = requestFactory.CreateRequest<TResult>(document);
TResult result = await executor.ExecuteAsync(request, cancellationToken);
```

## Related packages

- [`CodeArchitects.Platform.GraphQL`](../GraphQL) — GraphQL document model

## License

Licensed under the [Apache License 2.0](../../LICENSE).
