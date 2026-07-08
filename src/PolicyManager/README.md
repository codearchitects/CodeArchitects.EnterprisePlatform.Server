# CodeArchitects.Platform.PolicyManager

[![NuGet](https://img.shields.io/nuget/v/CodeArchitects.Platform.PolicyManager.svg)](https://www.nuget.org/packages/CodeArchitects.Platform.PolicyManager)
[![License: Apache 2.0](https://img.shields.io/badge/License-Apache_2.0-blue.svg)](../../LICENSE)

A **policy engine** for declaring and evaluating authorization policies in Code Architects Platform
applications. Policies are composed from claims and conditions and evaluated at runtime through
`IPolicyManager`, keeping authorization rules centralized and testable.

## Installation

```bash
dotnet add package CodeArchitects.Platform.PolicyManager
```

## Key types

| Type | Purpose |
|------|---------|
| `IPolicyManager` | Entry point to evaluate policies at runtime. |
| `BasePolicy` | Base class for defining a policy. |
| `PolicyCollection` | A set of policies. |
| `PolicyClaim` | A claim requirement used inside a policy. |
| `PolicyCondition` / `PolicyConditionType` | Conditions combined to express a rule. |

## Related packages

- [`CodeArchitects.Platform.PolicyManager.DependencyInjection`](../PolicyManager.DependencyInjection) — DI registration

## License

Licensed under the [Apache License 2.0](../../LICENSE).
