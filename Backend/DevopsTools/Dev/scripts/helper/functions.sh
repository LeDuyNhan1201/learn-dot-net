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

create_data_folders() {
  echo "Creating data folders"

  create_dir true "$DATA_DIR"

  # TODO: Add more data folders as needed"
}

create_env_file() {
  echo "Creating env file"

  : > "${ENV_DIR}/.env"

  local vars=(
    LOCAL_IP
    GATEWAY_PORT
    BACKEND_HOSTNAME

    NAMESPACE
    REPOSITORY_NAME

    ENVOY_TAG

    CERT_SECRET

    # TODO: Add more variables as needed
  )

  for var in "${vars[@]}"; do
    echo "$var=\"${!var}\"" >> "${ENV_DIR}/.env"
  done

  echo "Env file created successfully."
}
