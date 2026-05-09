#!/usr/bin/env bash
set -euo pipefail

# -------------------------------
# Configuration
# -------------------------------

MODE="dev"
export MODE
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" >/dev/null 2>&1 && pwd)"
ENV_DIR="$(cd "${SCRIPT_DIR}/.." >/dev/null 2>&1 && pwd)"
BACKEND_DIR="$(cd "${SCRIPT_DIR}/../../.." >/dev/null 2>&1 && pwd)"
HELPER_DIR="${SCRIPT_DIR}/helper"
export ENV_DIR

ENV_FILE="${HELPER_DIR}/env_config.sh"
FUNCTIONS_FILE="${HELPER_DIR}/functions.sh"
CERT_SCRIPT="${HELPER_DIR}/generate_certs.sh"
KEYPAIR_SCRIPT="${HELPER_DIR}/generate_keypair.sh"

# -------------------------------
# Load Environment & Helpers
# -------------------------------

# shellcheck source=scripts/helper/env_config.sh
source "${ENV_FILE}"
# shellcheck source=scripts/helper/functions.sh
source "${FUNCTIONS_FILE}"
# shellcheck source=scripts/helper/generate_certs.sh
source "${CERT_SCRIPT}"
# shellcheck source=scripts/helper/generate_keypair.sh
source "${KEYPAIR_SCRIPT}"

# -------------------------------
# Generate Environment & Certificates
# -------------------------------

create_env_file
create_files_from_templates
create_data_folders

generate_root_ca
generate_cert_with_keystore_and_truststore "api-gateway" "api-gateway" "${BACKEND_HOSTNAME}"

# -------------------------------
# Docker Image Build
# -------------------------------

IMAGE_PREFIX="${NAMESPACE}/${REPOSITORY_NAME}"

docker build --no-cache \
  --build-arg BACKEND_TAG="${BACKEND_TAG}" \
  --build-arg BACKEND_CONTAINER_PORT="${BACKEND_CONTAINER_PORT}" \
  -f "${BACKEND_DIR}/API/Docker/Native/Dockerfile" \
  -t "${IMAGE_PREFIX}/backend:${BACKEND_TAG}" \
  "${BACKEND_DIR}" || true
  
echo "Initialize completed successfully."
