#!/usr/bin/env bash
set -euo pipefail

# -------------------------------
# Configurations
# -------------------------------

MODE="dev"
export MODE
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" >/dev/null 2>&1 && pwd)"
ENV_DIR="$(cd "${SCRIPT_DIR}/.." >/dev/null 2>&1 && pwd)"
HELPER_DIR="${SCRIPT_DIR}/helper"
export ENV_DIR

ENV_FILE="${HELPER_DIR}/env_config.sh"
UTILS_FILE="${HELPER_DIR}/utils.sh"
K8S_FILE="${HELPER_DIR}/k8s_functions.sh"

# -------------------------------
# Load Environments & Helpers
# -------------------------------

# shellcheck source=scripts/helper/env_config.sh
source "${ENV_FILE}"
# shellcheck source=scripts/helper/utils.sh
source "${UTILS_FILE}"
# shellcheck source=scripts/helper/k8s_functions.sh
source "${K8S_FILE}"

# -------------------------------
# Uninstall Helm Releases & Delete Kubernetes Resources
# -------------------------------

delete_k8s_resources

echo "Helm releases and Kubernetes resources deleted successfully."
