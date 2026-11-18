#!/usr/bin/env bash
# Run admin-smoke.mjs against the preview. Credentials come from .env and reach the
# container as inherited variables (-e NAME), never on a command line.
set -euo pipefail
here="$(cd "$(dirname "$0")" && pwd)"
set -a; . "$here/../../.env"; set +a
export ADMIN_EMAIL ADMIN_PASSWORD
docker run --rm --network host --user "$(id -u):$(id -g)" -e HOME=/tmp -e ADMIN_EMAIL -e ADMIN_PASSWORD \
    -v "$here:/work" -w /work mcr.microsoft.com/playwright:v1.65.1-noble node admin-smoke.mjs
