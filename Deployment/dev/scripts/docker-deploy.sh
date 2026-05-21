#!/usr/bin/env bash
set -euo pipefail

ENV_DIR="$(cd -- "$(dirname -- "${BASH_SOURCE[0]}")/.." && pwd)"
SERVICES_DIR="$(cd -- "${ENV_DIR}/../services" && pwd)"
export SERVICES_DIR

docker compose -f "${ENV_DIR}/docker-compose.yml" up -d --force-recreate
