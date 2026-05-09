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

# -------------------------------
# Cleanup Files
# -------------------------------

echo "Removing environment file..."
sudo rm -f "${ENV_DIR}/.env"

echo "Cleanup completed successfully."
