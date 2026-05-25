#!/usr/bin/env bash
set -euo pipefail

# -------------------------------
# Configurations
# -------------------------------

MODE="dev"
export MODE
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" >/dev/null 2>&1 && pwd)"
ENV_DIR="$(cd "${SCRIPT_DIR}/.." >/dev/null 2>&1 && pwd)"
BACKEND_DIR="$(cd "${SCRIPT_DIR}/../../../Backend" >/dev/null 2>&1 && pwd)"
HELPER_DIR="${SCRIPT_DIR}/helper"
export ENV_DIR

ENV_FILE="${HELPER_DIR}/env_config.sh"
UTILS_FILE="${HELPER_DIR}/utils.sh"
GENERATOR_FILE="${HELPER_DIR}/generator.sh"

# -------------------------------
# Load Environments & Helpers
# -------------------------------

# shellcheck source=scripts/helper/env_config.sh
source "${ENV_FILE}"
# shellcheck source=scripts/helper/utils.sh
source "${UTILS_FILE}"
# shellcheck source=scripts/helper/generator.sh
source "${GENERATOR_FILE}"

# -------------------------------
# Docker Images Build
# -------------------------------

#build_backend_image "${BACKEND_DIR}"

build_postgres_image "${SERVICES_DIR}/postgres"

build_keycloak_image "${SERVICES_DIR}/keycloak"

echo "Build images completed successfully."
