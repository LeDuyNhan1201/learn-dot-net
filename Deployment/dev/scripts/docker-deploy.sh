#!/usr/bin/env bash
set -euo pipefail

ENV_DIR="$(cd -- "$(dirname -- "${BASH_SOURCE[0]}")/.." && pwd)"

#docker compose -f "${ENV_DIR}/docker-compose.yaml" up -d --force-recreate
docker compose -f "${ENV_DIR}/docker-compose.local.yaml" up gateway keycloak0 postgres -d --force-recreate