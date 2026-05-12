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

require_command() {
  local command_name=$1

  if ! command -v "$command_name" >/dev/null 2>&1; then
    echo "Error: '$command_name' is required but was not found in PATH." >&2
    return 1
  fi
}

backend_image_name() {
  echo "${NAMESPACE}/${REPOSITORY_NAME}/backend:${BACKEND_TAG}"
}

build_backend_image() {
  local backend_dir=${1:?backend_dir is required}
  local image_name
  image_name="$(backend_image_name)"

  require_command docker

  docker rmi "$image_name" || true
  docker build --no-cache \
    --build-arg BACKEND_TAG="${BACKEND_TAG}" \
    --build-arg BACKEND_CONTAINER_PORT="${BACKEND_CONTAINER_PORT}" \
    -f "${backend_dir}/API/Docker/Native/Dockerfile" \
    -t "$image_name" \
    "${backend_dir}"
}

create_data_folders() {
  echo "Creating data folders"

  create_dir true "$DATA_DIR"

  # TODO: Add more data folders as needed
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
    BACKEND_TAG

    CERT_SECRET

    API_GATEWAY_CONTAINER_NAME
    BACKEND_CONTAINER_NAME
    BACKEND_CONTAINER_PORT

    K8S_CLUSTER_NAME
    K8S_NAMESPACE

    # TODO: Add more variables as needed
  )

  for var in "${vars[@]}"; do
    echo "$var=\"${!var}\"" >> "${ENV_DIR}/.env"
  done

  echo "Env file created successfully."
}

create_files_from_templates() {
  echo "Creating files from templates"

  local templates=(
    "${ENV_DIR}/envoy/templates/api-gateway.template:${ENV_DIR}/envoy/api-gateway.yaml"
    "${ENV_DIR}/envoy/templates/api-gateway.local.template:${ENV_DIR}/envoy/api-gateway.local.yaml"
    "${ENV_DIR}/k8s/templates/backend-deployment.template:${ENV_DIR}/k8s/backend-deployment.yaml"
    "${ENV_DIR}/k8s/templates/backend-service.template:${ENV_DIR}/k8s/backend-service.yaml"
    "${ENV_DIR}/k8s/templates/api-gateway-deployment.template:${ENV_DIR}/k8s/api-gateway-deployment.yaml"
    "${ENV_DIR}/k8s/templates/api-gateway-service.template:${ENV_DIR}/k8s/api-gateway-service.yaml"

    # TODO: Add more templates as needed, pattern is "source:destination"
  )

  for item in "${templates[@]}"; do
    local src
    local dest
    IFS=":" read -r src dest <<< "$item"

    envsubst < "$src" > "$dest"
    echo "$src --> $dest"
  done

  echo "Files created successfully."
}

ensure_root_ca() {
  if [[ -f "${CERTS_DIR}/ca/ca.crt" && -f "${CERTS_DIR}/ca/ca.key" ]]; then
    echo "Root CA already exists."
    return 0
  fi

  generate_root_ca
}

generate_api_gateway_cert() {
  generate_cert_with_keystore_and_truststore "api-gateway" "api-gateway" "${BACKEND_HOSTNAME}"
}

ensure_kind_cluster() {
  require_command kind
  require_command kubectl

  if kind get clusters | grep -qx "${K8S_CLUSTER_NAME}"; then
    echo "Kind cluster '${K8S_CLUSTER_NAME}' already exists."
  else
    kind create cluster --name "${K8S_CLUSTER_NAME}"
  fi

  kubectl config use-context "kind-${K8S_CLUSTER_NAME}" >/dev/null
}

load_backend_image_to_kind() {
  require_command kind

  kind load docker-image "$(backend_image_name)" --name "${K8S_CLUSTER_NAME}"
}

deploy_k8s_resources() {
  require_command kubectl

  kubectl apply -k "${ENV_DIR}"
  kubectl -n "${K8S_NAMESPACE}" rollout status deployment/backend --timeout=120s
  kubectl -n "${K8S_NAMESPACE}" rollout status deployment/api-gateway --timeout=120s
}

delete_k8s_resources() {
  require_command kubectl

  kubectl -n "" delete deployment backend api-gateway --ignore-not-found=true
  kubectl -n "" delete service backend api-gateway --ignore-not-found=true
  kubectl -n "" delete configmap api-gateway-config --ignore-not-found=true
  kubectl -n "" delete secret api-gateway-certs --ignore-not-found=true
  kubectl delete namespace "" --ignore-not-found=true
}

delete_kind_cluster() {
  require_command kind

  kind delete cluster --name "${K8S_CLUSTER_NAME}"
}

port_forward_api_gateway() {
  require_command kubectl

  kubectl -n "${K8S_NAMESPACE}" port-forward "svc/${API_GATEWAY_CONTAINER_NAME}" "${GATEWAY_PORT}:${GATEWAY_PORT}"
}
