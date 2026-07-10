# Project Governance

This document describes how the Code Architects Enterprise Solution Platform
project is governed and how decisions are made.

## Overview

The project is maintained by **Code Architects S.r.l.** and is open to
contributions from the community under the [Apache License 2.0](LICENSE).

## Roles

### Users

Anyone who uses the libraries. Users are encouraged to participate by reporting
issues, requesting features, and joining discussions.

### Contributors

Anyone who submits a contribution (code, documentation, tests, reviews, triage)
to the project. Contributions are accepted via pull request and must follow
[CONTRIBUTING.md](CONTRIBUTING.md).

### Maintainers

Maintainers are responsible for the overall health and direction of the project.
They review and merge pull requests, triage issues, manage releases, and uphold
the [Code of Conduct](CODE_OF_CONDUCT.md). Maintainers are appointed by Code
Architects S.r.l. and are listed in [.github/CODEOWNERS](.github/CODEOWNERS).

## Decision making

Day-to-day decisions are made by maintainers through the normal review process:

- **Pull requests** require approval from at least one maintainer who is not the
  author before merging.
- **Non-trivial or breaking changes** should first be discussed in an issue to
  reach rough consensus.
- Where consensus cannot be reached, Code Architects S.r.l., as the project
  steward, makes the final decision.

## Releases

All packages share a single version and are released together. Releases are cut
by maintainers via the automated publish workflow — either by running it manually
from the Actions tab or by publishing a GitHub Release. See
[README.md](README.md#versioning--releases) for details.

## Changes to governance

This document may be amended by the maintainers. Material changes will be made
via pull request so the community can review them.
