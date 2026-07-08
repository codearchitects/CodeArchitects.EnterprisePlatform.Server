# Contributing to Code Architects Enterprise Solution Platform

Thank you for your interest in contributing! This document describes how to
propose changes to the project.

## Code of Conduct

This project and everyone participating in it is governed by our
[Code of Conduct](CODE_OF_CONDUCT.md). By participating, you are expected to
uphold it.

## How to contribute

### Reporting bugs

Open an issue using the **Bug report** template. Include the platform version,
target framework, a minimal reproduction and the expected vs. actual behavior.

### Suggesting enhancements

Open an issue using the **Feature request** template and describe the use case
and the problem you are trying to solve.

### Pull requests

1. Fork the repository and create a branch from `develop`.
2. Make your change, keeping the existing code style and conventions.
3. Add or update tests. New behavior should be covered by unit tests under `tests/`.
4. Ensure the build and tests pass locally:

   ```bash
   dotnet build CodeArchitects.Platform.sln --configuration Release
   dotnet test CodeArchitects.Platform.sln --configuration Release
   ```

5. Open a pull request against `develop`, filling in the PR template.

## Developer environment

- [.NET SDK 10.0](https://dotnet.microsoft.com/download)
- An IDE with C# support (Visual Studio, Rider or VS Code)

## Licensing of contributions

By submitting a contribution, you agree that your contribution will be licensed
under the [Apache License, Version 2.0](LICENSE), consistent with the rest of the
project.
