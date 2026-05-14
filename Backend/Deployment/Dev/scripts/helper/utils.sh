#!/usr/bin/env bash
set -euo pipefail

# Usage: create_dir <use_sudo> <dir>
create_dir() {
  local use_sudo=$1
  local dir=$2

  if [ "$use_sudo" = true ]; then
    sudo mkdir -p "$dir"
    sudo chown -R 1000:1000 "$dir"
    sudo chmod -R 755 "$dir"
  else
    mkdir -p "$dir"
    chown -R 1000:1000 "$dir"
    chmod -R 755 "$dir"
  fi

  echo "Created $dir"
}

# Usage: require_command <command_name>
# Ex: require_command docker
require_command() {
  local command_name=$1

  if ! command -v "$command_name" >/dev/null 2>&1; then
    echo "Error: '$command_name' is required but was not found in PATH." >&2
    return 1
  fi
}

# Usage: ensure_helm_chart <chart_dir>
# Ex: ensure_helm_chart "./k8s-helm/backend"
ensure_helm_chart() {
  local chart_dir=${1:?chart_dir is required}

  if [[ ! -f "${chart_dir}/Chart.yaml" ]]; then
    echo "Error: Helm chart was not found at '${chart_dir}'." >&2
    return 1
  fi
}

ensure_root_ca() {
  if [[ -f "${CERTS_DIR}/ca/ca.crt" && -f "${CERTS_DIR}/ca/ca.key" ]]; then
    echo "Root CA already exists."
    return 0
  fi

  generate_root_ca
}