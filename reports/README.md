# Security Reports

Local security audit and SBOM reports for the .NET solution, generated via either:

```bash
# MSBuild target (no Node knowledge needed):
dotnet msbuild build/SecurityReport.proj -t:SecurityReport

# ...or the underlying script directly:
node scripts/generate-security-report.mjs
```

The MSBuild target is a thin wrapper around the script, so both do exactly the same thing.

These files are **git-tracked** (not ignored) so the history of vulnerabilities, SBOM
changes, and accepted advisories is visible in the repository. Each run **overwrites**
the previous report; git tracks the history via commits.

## Files

- **`audit/audit-vulnerabilities.json`** — output of
  `dotnet list <sln> package --vulnerable --include-transitive --format json`
  (all projects: `src/`, `tests/`, tooling). The CI gate fails on unaccepted
  vulnerabilities in **shipped (`src/`)** packages.
- **`sbom/sbom.json`** — CycloneDX Software Bill of Materials for the solution
  (`dotnet CycloneDX`). Uploaded as an artifact by CI.
- **`vulnerabilities/advisories.md`** — human-readable summary: shipped vs. test/tooling
  vulnerabilities, and accepted vs. unaccepted status.

## Accepted advisories — single source of truth

Accepted advisories live in **`Directory.Build.props`** (`<NuGetAuditSuppress>` entries),
which is also the allow-list the CI security gate reads and the source of the package
`Version`. Rationales for each accepted advisory are documented in **`SECURITY.md`**.
Do **not** maintain a separate allow-list file.

## Requirements

- .NET SDK ≥ 9 (for `dotnet list package --format json`) — the repo targets 10.x.
- CycloneDX .NET tool: `dotnet tool install --global CycloneDX`.

## Generate & review before commit

```bash
node scripts/generate-security-report.mjs
git diff reports/          # review changes
git add reports/
git commit -m "docs: update security reports"
```

The script exits non-zero if it finds a **new unaccepted** vulnerability in a shipped
(`src/`) package, so it can double as a local pre-commit / CI gate.
