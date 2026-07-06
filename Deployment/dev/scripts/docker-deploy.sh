#!/usr/bin/env bash
set -euo pipefail

MODE="${1:-dev}"
ENV_DIR="$(cd -- "$(dirname -- "${BASH_SOURCE[0]}")/.." && pwd)"

if [[ "${MODE}" == "dev" ]]; then
    echo "Starting Docker Compose services for development mode..."
    docker compose -f "${ENV_DIR}/docker-compose.yaml" up -d --force-recreate
elif [[ "${MODE}" == "local" ]]; then
    echo "Starting Docker Compose services for local mode..."
    docker compose -f "${ENV_DIR}/docker-compose.local.yaml" up gateway keycloak0 postgres -d --force-recreate
else
    echo "Invalid mode specified. Use 'dev' or 'local'."
    exit 1
fi
