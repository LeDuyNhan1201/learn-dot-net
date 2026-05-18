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
#docker pull dhi.io/envoy:${ENVOY_TAG}
#docker pull dhi.io/grafana:${GRAFANA_TAG}
#docker pull dhi.io/loki:${LOKI_TAG}
#docker pull dhi.io/tempo:${TEMPO_TAG}
#docker pull dhi.io/prometheus:${PROMETHEUS_TAG}
#docker pull dhi.io/opentelemetry-collector:${OTEL_COLLECTOR_TAG}

docker pull envoyproxy/envoy:${ENVOY_TAG} 
docker pull grafana/grafana:${GRAFANA_TAG} 
docker pull grafana/loki:${LOKI_TAG} 
docker pull grafana/tempo:${TEMPO_TAG} 
docker pull prom/prometheus:${PROMETHEUS_TAG} 
docker pull otel/opentelemetry-collector-contrib:${OTEL_COLLECTOR_TAG}