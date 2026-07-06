#!/usr/bin/env bash
set -euo pipefail

# -------------------------------
# Configuration
# -------------------------------

MODE="${1:-dev}"
export MODE
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" >/dev/null 2>&1 && pwd)"
ENV_DIR="$(cd "${SCRIPT_DIR}/.." >/dev/null 2>&1 && pwd)"
HELPER_DIR="${SCRIPT_DIR}/helper"
export ENV_DIR

ENV_FILE="${HELPER_DIR}/env_config.sh"

# shellcheck source=scripts/helper/env_config.sh
source "${ENV_FILE}"

# Down Docker Compose services and remove volumes
if [[ "${MODE}" == "dev" ]]; then
    echo "Stopping Docker Compose services for development mode..."
    docker compose -f "${ENV_DIR}/docker-compose.yaml" down -v
elif [[ "${MODE}" == "local" ]]; then
    echo "Stopping Docker Compose services for local mode..."
    docker compose -f "${ENV_DIR}/docker-compose.local.yaml" down gateway keycloak0 postgres -v
else
    echo "Invalid mode specified. Use 'dev' or 'local'."
    exit 1
fi

# -------------------------------
# Cleanup Data
# -------------------------------

echo "Removing all data"

sudo rm -rf "${ENV_DIR}/data/"*
sudo rm -f "${ENV_DIR}/.env"

echo "Cleanup data completed successfully."
