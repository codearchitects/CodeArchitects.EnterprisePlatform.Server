# Security Report

Generated: 2026-07-10

Accepted advisories are sourced from `Directory.Build.props` (`NuGetAuditSuppress`),
the single source of truth also used by the CI security gate. Rationales are documented
in `SECURITY.md`.

## Shipped Dependencies Vulnerabilities (`src/`)

**Total found:** 2 | **Accepted:** 2 | **Unaccepted:** 0

| GHSA | Severity | Package(s) | Status |
|------|----------|------------|--------|
| [GHSA-RVV3-G6HJ-G44X](https://github.com/advisories/GHSA-RVV3-G6HJ-G44X) | High | AutoMapper@10.0.0, AutoMapper@14.0.0 | ✓ accepted |
| [GHSA-6C8G-7P36-R338](https://github.com/advisories/GHSA-6C8G-7P36-R338) | Moderate | SharpCompress@0.30.1 | ✓ accepted |

## Test / Tooling Vulnerabilities (not shipped)

_Informational — these packages are not part of published NuGet packages._

| GHSA | Severity | Package(s) | Status |
|------|----------|------------|--------|
| [GHSA-2M69-GCR7-JV3Q](https://github.com/advisories/GHSA-2M69-GCR7-JV3Q) | High | SQLitePCLRaw.lib.e_sqlite3@2.1.11, SQLitePCLRaw.lib.e_sqlite3@2.1.4, SQLitePCLRaw.lib.e_sqlite3@2.1.6, SQLitePCLRaw.lib.e_sqlite3@2.1.10 | ✓ accepted |
| [GHSA-6QMF-MMC7-6C2P](https://github.com/advisories/GHSA-6QMF-MMC7-6C2P) | High | NuGet.Common@5.6.0, NuGet.Protocol@5.6.0 | ✓ accepted |
| [GHSA-68W7-72JG-6QPP](https://github.com/advisories/GHSA-68W7-72JG-6QPP) | Critical | NuGet.Packaging@5.6.0 | ✓ accepted |
| [GHSA-G3Q9-XF95-8HP5](https://github.com/advisories/GHSA-G3Q9-XF95-8HP5) | High | NuGet.Protocol@5.6.0 | ✓ accepted |

## Accepted Advisories (`Directory.Build.props`)

- [GHSA-2M69-GCR7-JV3Q](https://github.com/advisories/GHSA-2M69-GCR7-JV3Q)
- [GHSA-68W7-72JG-6QPP](https://github.com/advisories/GHSA-68W7-72JG-6QPP)
- [GHSA-6C8G-7P36-R338](https://github.com/advisories/GHSA-6C8G-7P36-R338)
- [GHSA-6QMF-MMC7-6C2P](https://github.com/advisories/GHSA-6QMF-MMC7-6C2P)
- [GHSA-G3Q9-XF95-8HP5](https://github.com/advisories/GHSA-G3Q9-XF95-8HP5)
- [GHSA-RVV3-G6HJ-G44X](https://github.com/advisories/GHSA-RVV3-G6HJ-G44X)
