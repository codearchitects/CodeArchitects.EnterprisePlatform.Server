# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).
All packages in the repository share a single version.

## [10.0.0]

### Added

- Initial public open-source release of the Code Architects Enterprise Solution
  Platform under the Apache License 2.0.
- `CodeArchitects.Platform.*` libraries — messaging, actors, state management,
  data access (ADO.NET, Entity Framework Core, MongoDB), application services,
  policy management and GraphQL — published to NuGet.org from a single .NET
  solution, with first-class [Dapr](https://dapr.io/) support.
- Project legal and governance documentation: `LICENSE`, `NOTICE`,
  `CONTRIBUTING.md`, `CODE_OF_CONDUCT.md`, `SECURITY.md`, `GOVERNANCE.md`,
  `SUPPORT.md`, issue/PR templates, and `.github/CODEOWNERS`.
- Continuous integration (build + test), a NuGet publish workflow with
  semantic/pre-release versioning, an SBOM step, and Dependabot configuration.

[10.0.0]: https://github.com/codearchitects/CodeArchitects.EnterprisePlatform.Server/releases/tag/v10.0.0
