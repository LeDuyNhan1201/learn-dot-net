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
UTILS_FILE="${HELPER_DIR}/utils.sh"
GENERATOR_FILE="${HELPER_DIR}/generator.sh"

# -------------------------------
# Load Environment & Helpers
# -------------------------------

# shellcheck source=scripts/helper/env_config.sh
source "${ENV_FILE}"
# shellcheck source=scripts/helper/utils.sh
source "${UTILS_FILE}"
# shellcheck source=scripts/helper/functions.sh
source "${GENERATOR_FILE}"

# -------------------------------
# Pull Docker Images
# -------------------------------
docker pull "${ENVOY_REPO_NAME}/envoy:${ENVOY_TAG}" 
docker pull "${GRAFANA_REPO_NAME}/grafana:${GRAFANA_TAG}" 
docker pull "${LOKI_REPO_NAME}/loki:${LOKI_TAG}" 
docker pull "${TEMPO_REPO_NAME}/tempo:${TEMPO_TAG}" 
docker pull "${PROMETHEUS_REPO_NAME}/prometheus:${PROMETHEUS_TAG}" 
docker pull "${OTEL_REPO_NAME}/opentelemetry-collector-contrib:${OTEL_TAG}"
docker pull "postgres:${POSTGRES_TAG}"
docker pull "${KEYCLOAK_REPO_NAME}/keycloak:${KEYCLOAK_TAG}"