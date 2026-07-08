# Security Policy

## Reporting a vulnerability

We take the security of the Code Architects Enterprise Solution Platform seriously.

**Please do not report security vulnerabilities through public GitHub issues.**

Instead, report them privately opening a [private security advisory](https://github.com/codearchitects/CodeArchitects.EnterprisePlatform.Server/security/advisories/new)

Please include:

- A description of the vulnerability and its impact.
- Steps to reproduce, or a proof of concept.
- The affected package(s) and version(s).

We will acknowledge your report as soon as possible and keep you informed of the
progress toward a fix. We ask that you give us a reasonable amount of time to
address the issue before any public disclosure.

## Supported versions

Security fixes are provided for the latest released major version.

## Known / accepted advisories

The following dependency advisories are known and **accepted** because no viable fix is
currently available. They are suppressed centrally in `Directory.Build.props` (which the CI
security gate reads as its allow-list) and reviewed on every dependency update. Any *new*,
un-accepted vulnerability in a shipped (`src/`) package fails the build.

| Advisory | Package | Severity | Impact | Rationale |
|----------|---------|----------|--------|-----------|
| [GHSA-rvv3-g6hj-g44x](https://github.com/advisories/GHSA-rvv3-g6hj-g44x) | AutoMapper 10/14 | High | Shipped (`CodeArchitects.Platform.Data.AutoMapper`) | The fix exists only in the commercial AutoMapper 15/16 majors. This package is kept in **maintenance mode** for applications not yet migrated off AutoMapper — new projects should use `CodeArchitects.Platform.Data.Mapster`. |
| [GHSA-6c8g-7p36-r338](https://github.com/advisories/GHSA-6c8g-7p36-r338) | SharpCompress 0.30.1 | Moderate | Shipped (transitive via `MongoDB.Driver` 2.19.0) | No patched release is compatible with the `netstandard2.1` target. The proper remediation is upgrading `MongoDB.Driver`, which is a breaking change tracked as a separate work item. |
| [GHSA-2m69-gcr7-jv3q](https://github.com/advisories/GHSA-2m69-gcr7-jv3q) | SQLitePCLRaw.lib.e_sqlite3 2.1.x | High | **Test-only** (EF Core functional tests) | Not shipped in any package. The fix requires the SQLitePCLRaw 3.x major, which EF Core 7/8 do not support. |
| [GHSA-68w7-72jg-6qpp](https://github.com/advisories/GHSA-68w7-72jg-6qpp) | NuGet.Packaging 5.6.0 | Critical | **Test-only** (Roslyn analyzer-testing harness) | Transitive via `Microsoft.CodeAnalysis.CSharp.Analyzer.Testing.XUnit` 1.1.1. Patched NuGet.* only ships in harness 1.1.2, whose behavioral changes break the analyzer code-fix tests. Not shipped; no untrusted input. Follow-up: upgrade harness to 1.1.2 + fix the affected tests. |
| [GHSA-6qmf-mmc7-6c2p](https://github.com/advisories/GHSA-6qmf-mmc7-6c2p), [GHSA-g3q9-xf95-8hp5](https://github.com/advisories/GHSA-g3q9-xf95-8hp5) | NuGet.Common / NuGet.Protocol 5.6.0 | High | **Test-only** (Roslyn analyzer-testing harness) | Same root cause and follow-up as GHSA-68w7-72jg-6qpp. |

Advisories that **were** remediated (for reference): the vulnerable `Snappier` (shipped, via `MongoDB.Driver`) and `System.Security.Cryptography.Xml` (tests) were upgraded to patched versions.
