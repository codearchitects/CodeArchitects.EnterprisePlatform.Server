#!/usr/bin/env bash
#
# Computes the next package version for the release workflow.
#
# Usage:
#   compute-version.sh bump <type> <preid> <current>
#     type  = patch | minor | major | prepatch | preminor | premajor | prerelease
#     preid = beta | rc | alpha | canary | next   (used only by pre* types)
#     current = current version, e.g. 7.0.0 or 7.1.0-beta.2
#
#   compute-version.sh tag <vX.Y.Z[-preid.N]>
#     Echoes the version encoded in a release tag (strips a leading "v").
#
# Prints the resulting SemVer version to stdout (e.g. 7.1.0 or 7.1.0-beta.0).
set -euo pipefail

mode="${1:?usage: compute-version.sh <bump|tag> ...}"

if [[ "$mode" == "tag" ]]; then
  v="${2:?missing tag}"
  echo "${v#v}"
  exit 0
fi

[[ "$mode" == "bump" ]] || { echo "unknown mode: $mode" >&2; exit 1; }

type="${2:?missing release type}"
preid="${3:-beta}"
current="${4:?missing current version}"

# Parse current into major/minor/patch and optional pre-release (e.g. "beta.2").
current="${current#v}"
core="${current%%-*}"
pre=""
[[ "$current" == *-* ]] && pre="${current#*-}"
IFS='.' read -r M m p <<< "$core"

case "$type" in
  major) echo "$((M + 1)).0.0" ;;
  minor) echo "${M}.$((m + 1)).0" ;;
  patch) echo "${M}.${m}.$((p + 1))" ;;
  premajor) echo "$((M + 1)).0.0-${preid}.0" ;;
  preminor) echo "${M}.$((m + 1)).0-${preid}.0" ;;
  prepatch) echo "${M}.${m}.$((p + 1))-${preid}.0" ;;
  prerelease)
    if [[ -n "$pre" ]]; then
      cur_preid="${pre%%.*}"
      cur_num="${pre##*.}"
      if [[ "$cur_preid" == "$preid" && "$cur_num" =~ ^[0-9]+$ ]]; then
        # Same identifier: increment the pre-release counter.
        echo "${M}.${m}.${p}-${preid}.$((cur_num + 1))"
      else
        # Different identifier: reset the counter.
        echo "${M}.${m}.${p}-${preid}.0"
      fi
    else
      # From a stable version: bump patch and start a pre-release.
      echo "${M}.${m}.$((p + 1))-${preid}.0"
    fi
    ;;
  *) echo "unknown release type: $type" >&2; exit 1 ;;
esac
