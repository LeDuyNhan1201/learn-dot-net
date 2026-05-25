#!/usr/bin/env bash
set -euo pipefail

# -------------------------------
# Configuration
# -------------------------------

MODE="dev"
export MODE
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" >/dev/null 2>&1 && pwd)"
ENV_DIR="$(cd "${SCRIPT_DIR}/.." >/dev/null 2>&1 && pwd)"
HELPER_DIR="${SCRIPT_DIR}/helper"
export ENV_DIR

ENV_FILE="${HELPER_DIR}/env_config.sh"

# shellcheck source=scripts/helper/env_config.sh
source "${ENV_FILE}"

# Down Docker Compose services and remove volumes
docker compose -f "${ENV_DIR}/docker-compose.yaml" down -v

IMAGE_PREFIX="${NAMESPACE}/${REPOSITORY_NAME}"

# -------------------------------
# Cleanup Files
# -------------------------------

echo "Removing certs, keypairs, data and environment files..."

sudo rm -rf "${ENV_DIR}/certs/"*
sudo rm -rf "${ENV_DIR}/keypair/"*
sudo rm -rf "${ENV_DIR}/data/"*
sudo rm -f "${ENV_DIR}/.env"

# -------------------------------
# Remove Docker Images
# -------------------------------

docker rmi "${IMAGE_PREFIX}/backend:${BACKEND_TAG}" || true
docker rmi "${IMAGE_PREFIX}/postgres:${POSTGRES_TAG}" || true
docker rmi "${IMAGE_PREFIX}/keycloak:${KEYCLOAK_TAG}" || true

echo "Cleanup all completed successfully."
