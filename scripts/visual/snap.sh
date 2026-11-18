#!/usr/bin/env bash
# Screenshots of the preview via Playwright in Docker (the host lacks browser libs).
#   scripts/visual/snap.sh <outdir> "/work,/projects" dark,light d,m full
# <outdir> may be absolute; it is mounted into the container at the same path.
set -euo pipefail
here="$(cd "$(dirname "$0")" && pwd)"
out="$(mkdir -p "$1" && cd "$1" && pwd)"; shift
IMG=mcr.microsoft.com/playwright:v1.65.1-noble
if [[ ! -d "$here/node_modules/playwright-core" ]]; then
    docker run --rm --user "$(id -u):$(id -g)" -e HOME=/tmp -v "$here:/work" -w /work "$IMG" \
        npm install --no-save --silent playwright-core@1.65.1 >/dev/null
fi
docker run --rm --network host --user "$(id -u):$(id -g)" -e HOME=/tmp \
    -v "$here:/work" -v "$out:$out" -w /work "$IMG" node snap.mjs "$out" "$@"
