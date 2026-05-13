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
# Generate Environment, Envoy Configs & Certificates
# -------------------------------

create_env_file
create_files_from_templates
ensure_root_ca
generate_tls_certs

# -------------------------------
# Build, Load & Deploy Helm Releases
# -------------------------------

build_backend_image "${BACKEND_DIR}"
ensure_kind_cluster
load_backend_image_to_kind
deploy_k8s_resources

echo "Kubernetes deploy completed successfully."
