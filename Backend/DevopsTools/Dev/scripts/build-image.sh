#!/usr/bin/env bash
set -euo pipefail

# -------------------------------
# Configuration
# -------------------------------

MODE="dev"
export MODE
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" >/dev/null 2>&1 && pwd)"
ENV_DIR="$(cd "${SCRIPT_DIR}/.." >/dev/null 2>&1 && pwd)"
BACKEND_DIR="$(cd "${SCRIPT_DIR}/../../../API" >/dev/null 2>&1 && pwd)"
HELPER_DIR="${SCRIPT_DIR}/helper"
export ENV_DIR

ENV_FILE="${HELPER_DIR}/env_config.sh"

# -------------------------------
# Load Environment & Helpers
# -------------------------------

# shellcheck source=scripts/helper/env_config.sh
source "${ENV_FILE}"

# -------------------------------
# Docker Image Build
# -------------------------------

IMAGE_PREFIX="${NAMESPACE}/${REPOSITORY_NAME}"

docker rmi "${IMAGE_PREFIX}/backend:${BACKEND_TAG}" || true
docker build \
  --build-arg BACKEND_TAG="${BACKEND_TAG}" \
  --build-arg BACKEND_CONTAINER_PORT="${BACKEND_CONTAINER_PORT}" \
  -f "${BACKEND_DIR}/Docker/Native/Dockerfile" \
  -t "${IMAGE_PREFIX}/backend:${BACKEND_TAG}" \
  "${BACKEND_DIR}" || true
  
echo "Build image completed successfully."
