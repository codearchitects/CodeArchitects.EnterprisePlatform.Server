#!/usr/bin/env node
// Generates local security audit + SBOM reports for the .NET solution into reports/.
// Each run OVERWRITES the previous report; git tracks the history via commits.
//
// This mirrors the the EXACT same commands as the CI `security` job
// (.github/workflows/ci.yml):
//   * dotnet list <sln> package --vulnerable --include-transitive --format json
//   * dotnet CycloneDX <sln> ... (SBOM)
//   * accepted advisories come from Directory.Build.props (single source of truth)
//
// Usage:
//   node scripts/generate-security-report.mjs
//
// Requirements (already used by CI):
//   * .NET SDK >= 9 (for `dotnet list package --format json`) — repo uses 10.x
//   * CycloneDX .NET tool: `dotnet tool install --global CycloneDX`
import { writeFileSync, readFileSync, mkdirSync, existsSync } from "node:fs";
import { join, dirname, relative } from "node:path";
import { fileURLToPath } from "node:url";
import { execSync } from "node:child_process";

const root = join(dirname(fileURLToPath(import.meta.url)), "..");
const reportsDir = join(root, "reports");
const solution = "CodeArchitects.Platform.sln";

// Ensure reports/ subdirectories exist.
["audit", "sbom", "vulnerabilities"].forEach((subdir) => {
  const dir = join(reportsDir, subdir);
  if (!existsSync(dir)) mkdirSync(dir, { recursive: true });
});

// Run a command, returning stdout as string. `dotnet list --vulnerable` exits 0 even
// when vulnerabilities are found, but we still tolerate a non-zero exit and keep stdout.
function run(cmd, opts = {}) {
  try {
    return execSync(cmd, {
      encoding: "utf8",
      cwd: root,
      maxBuffer: 64 * 1024 * 1024,
      stdio: ["ignore", "pipe", "pipe"],
      ...opts,
    });
  } catch (e) {
    if (e.stdout) return e.stdout.toString();
    throw e;
  }
}

console.log("📊 Generating .NET security reports...\n");

// 0. Restore so the vulnerability database resolves against the locked graph
//    (same prerequisite the CI security job satisfies with `dotnet restore`).
console.log(`  • dotnet restore ${solution}`);
try {
  execSync(`dotnet restore ${solution}`, { cwd: root, stdio: "inherit" });
} catch (e) {
  console.error("    Warning: restore failed, audit may be incomplete:", e.message);
}

// 1. Vulnerability audit (full: src + tests + tooling), JSON.
//    Same command family as the CI "Vulnerability scan (full, informational)" step.
console.log("  • dotnet list package --vulnerable --include-transitive --format json");
const auditPath = join(reportsDir, "audit", "audit-vulnerabilities.json");
const auditRaw = run(
  `dotnet list ${solution} package --vulnerable --include-transitive --format json`
);
// Keep only the JSON payload (strip any leading restore/info chatter, just in case).
const jsonStart = auditRaw.indexOf("{");
const auditJsonText = jsonStart >= 0 ? auditRaw.slice(jsonStart) : auditRaw;

let audit = {};
try {
  audit = JSON.parse(auditJsonText);
  // dotnet emits ABSOLUTE machine paths; rewrite to repo-relative, POSIX-style, so the
  // committed report is portable and doesn't leak local filesystem layout.
  for (const project of audit.projects || []) {
    if (project.path) {
      project.path = relative(root, project.path).replace(/\\/g, "/");
    }
  }
  writeFileSync(auditPath, JSON.stringify(audit, null, 2) + "\n");
} catch (e) {
  console.error("    Warning: could not parse audit JSON:", e.message);
  writeFileSync(auditPath, auditJsonText); // fall back to raw output
}

// 2. SBOM (CycloneDX). Same command as the CI "Generate SBOM (CycloneDX)" step.
console.log("  • dotnet CycloneDX (SBOM)");
try {
  execSync(
    `dotnet CycloneDX ${solution} -o "${join(reportsDir, "sbom")}" --json --filename sbom.json`,
    { cwd: root, stdio: "inherit" }
  );
} catch (e) {
  console.error("    Error generating SBOM:", e.message);
}

// 3. Accepted advisories = the allow-list in Directory.Build.props (single source of
//    truth — same list the CI gate reads). We collect every GHSA id mentioned there.
const acceptedSet = new Set();
const propsPath = join(root, "Directory.Build.props");
if (existsSync(propsPath)) {
  const props = readFileSync(propsPath, "utf8");
  for (const m of props.matchAll(/GHSA-[0-9a-z]+-[0-9a-z]+-[0-9a-z]+/gi)) {
    acceptedSet.add(m[0].toUpperCase());
  }
}

// 4. Collect advisories from the audit JSON, classifying by shipped (src/) vs test/other.
//    Shape (dotnet list --format json):
//    { projects: [ { path, frameworks: [ { topLevelPackages, transitivePackages:
//      [ { id, resolvedVersion, vulnerabilities: [ { severity, advisoryurl } ] } ] } ] } ] }
const advisories = new Map(); // GHSA -> { severity, packages:Set, shipped:bool }
const GHSA_RE = /GHSA-[0-9a-z]+-[0-9a-z]+-[0-9a-z]+/i;

for (const project of audit.projects || []) {
  const path = (project.path || "").replace(/\\/g, "/");
  // path is now repo-relative (e.g. "src/Foo/Foo.csproj"); shipped = under src/.
  const shipped = /(^|\/)src\//i.test(path);
  for (const fw of project.frameworks || []) {
    const pkgs = [
      ...(fw.topLevelPackages || []),
      ...(fw.transitivePackages || []),
    ];
    for (const pkg of pkgs) {
      for (const vuln of pkg.vulnerabilities || []) {
        const m = GHSA_RE.exec(vuln.advisoryurl || "");
        if (!m) continue;
        const ghsa = m[0].toUpperCase();
        const entry = advisories.get(ghsa) || {
          severity: vuln.severity || "unknown",
          packages: new Set(),
          shipped: false,
        };
        entry.packages.add(`${pkg.id}@${pkg.resolvedVersion || "?"}`);
        if (shipped) entry.shipped = true;
        advisories.set(ghsa, entry);
      }
    }
  }
}

const shippedAdvisories = [...advisories.entries()].filter(([, v]) => v.shipped);
const otherAdvisories = [...advisories.entries()].filter(([, v]) => !v.shipped);
const unacceptedShipped = shippedAdvisories.filter(([id]) => !acceptedSet.has(id));

const now = new Date().toISOString().split("T")[0];
const md = [
  "# Security Report",
  "",
  `Generated: ${now}`,
  "",
  "Accepted advisories are sourced from `Directory.Build.props` (`NuGetAuditSuppress`),",
  "the single source of truth also used by the CI security gate. Rationales are documented",
  "in `SECURITY.md`.",
  "",
  "## Shipped Dependencies Vulnerabilities (`src/`)",
  "",
  `**Total found:** ${shippedAdvisories.length} | **Accepted:** ${
    shippedAdvisories.filter(([id]) => acceptedSet.has(id)).length
  } | **Unaccepted:** ${unacceptedShipped.length}`,
  "",
];

if (shippedAdvisories.length === 0) {
  md.push("✅ **No vulnerabilities detected in shipped (src/) dependencies.**");
} else {
  md.push("| GHSA | Severity | Package(s) | Status |");
  md.push("|------|----------|------------|--------|");
  for (const [ghsa, info] of shippedAdvisories) {
    const status = acceptedSet.has(ghsa) ? "✓ accepted" : "⚠️ UNACCEPTED";
    const url = `https://github.com/advisories/${ghsa}`;
    md.push(
      `| [${ghsa}](${url}) | ${info.severity} | ${[...info.packages].join(", ")} | ${status} |`
    );
  }
}

md.push("", "## Test / Tooling Vulnerabilities (not shipped)", "");
md.push("_Informational — these packages are not part of published NuGet packages._", "");
if (otherAdvisories.length === 0) {
  md.push("✅ **None detected.**");
} else {
  md.push("| GHSA | Severity | Package(s) | Status |");
  md.push("|------|----------|------------|--------|");
  for (const [ghsa, info] of otherAdvisories) {
    const status = acceptedSet.has(ghsa) ? "✓ accepted" : "⚠️ unaccepted";
    const url = `https://github.com/advisories/${ghsa}`;
    md.push(
      `| [${ghsa}](${url}) | ${info.severity} | ${[...info.packages].join(", ")} | ${status} |`
    );
  }
}

md.push("", "## Accepted Advisories (`Directory.Build.props`)", "");
if (acceptedSet.size === 0) {
  md.push("_(none)_");
} else {
  for (const id of [...acceptedSet].sort()) {
    md.push(`- [${id}](https://github.com/advisories/${id})`);
  }
}
md.push("");

writeFileSync(join(reportsDir, "vulnerabilities", "advisories.md"), md.join("\n"));

console.log("\n✅ Reports generated in reports/\n");
console.log("  audit/audit-vulnerabilities.json  (dotnet list package --vulnerable, full)");
console.log("  sbom/sbom.json                    (CycloneDX SBOM)");
console.log("  vulnerabilities/advisories.md     (summary, shipped vs test, accept status)");
console.log("\nThese files are git-tracked. Review with `git diff reports/` before commit.");

// Exit non-zero if a NEW un-accepted vulnerability is present in shipped packages,
// so the script can double as a local pre-commit / CI gate (matches ci.yml behavior).
if (unacceptedShipped.length > 0) {
  console.error(
    `\n❌ ${unacceptedShipped.length} unaccepted vulnerability(ies) in shipped (src/) packages:`
  );
  for (const [ghsa, info] of unacceptedShipped) {
    console.error(`   ${ghsa} (${info.severity}) — ${[...info.packages].join(", ")}`);
  }
  console.error(
    "\nFix the dependency, or (if no fix exists) add the advisory to the allow-list in\n" +
      "Directory.Build.props with a documented rationale in SECURITY.md."
  );
  process.exit(1);
}
