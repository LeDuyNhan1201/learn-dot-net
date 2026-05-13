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
    API_GATEWAY_CERT_SECRET_NAME
    API_GATEWAY_CERT_FILE_NAME
    API_GATEWAY_KEY_FILE_NAME
    API_GATEWAY_HTTPS_PORT
    API_GATEWAY_ADMIN_PORT
    BACKEND_CONTAINER_NAME
    BACKEND_CONTAINER_PORT

    K8S_CLUSTER_NAME
    K8S_NAMESPACE
    K8S_HELM_DIR
    BACKEND_HELM_RELEASE_NAME
    API_GATEWAY_HELM_RELEASE_NAME
    BACKEND_HELM_CHART_DIR
    API_GATEWAY_HELM_CHART_DIR
    HELM_TIMEOUT

    # TODO: Add more variables as needed
  )

  for var in "${vars[@]}"; do
    echo "$var=\"${!var}\"" >> "${ENV_DIR}/.env"
  done

  echo "Env file created successfully."
}

create_files_from_templates() {
  echo "Creating files from templates"

  require_command envsubst

  local templates=(
    "${ENV_DIR}/envoy/templates/api-gateway.template:${ENV_DIR}/envoy/api-gateway.yaml"
    "${ENV_DIR}/envoy/templates/api-gateway.local.template:${ENV_DIR}/envoy/api-gateway.local.yaml"

    # TODO: Add more templates as needed, pattern is "source:destination"
  )

  for item in "${templates[@]}"; do
    local src
    local dest
    IFS=":" read -r src dest <<< "$item"

    if [[ ! -f "$src" ]]; then
      echo "Skipping missing template: $src"
      continue
    fi

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

generate_tls_certs() {
  generate_cert_with_keystore_and_truststore "${API_GATEWAY_CONTAINER_NAME}" "${API_GATEWAY_CONTAINER_NAME}" "${BACKEND_HOSTNAME}"
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

api_gateway_cert_file() {
  echo "${CERTS_DIR}/${API_GATEWAY_CONTAINER_NAME}/${API_GATEWAY_CERT_FILE_NAME}"
}

api_gateway_key_file() {
  echo "${CERTS_DIR}/${API_GATEWAY_CONTAINER_NAME}/${API_GATEWAY_KEY_FILE_NAME}"
}

ensure_helm_chart() {
  local chart_dir=${1:?chart_dir is required}

  if [[ ! -f "${chart_dir}/Chart.yaml" ]]; then
    echo "Error: Helm chart was not found at '${chart_dir}'." >&2
    return 1
  fi
}

ensure_k8s_namespace() {
  require_command kubectl

  kubectl create namespace "${K8S_NAMESPACE}" --dry-run=client -o yaml | kubectl apply -f -
}

ensure_api_gateway_cert_secret() {
  require_command kubectl

  local cert_file
  local key_file
  cert_file="$(api_gateway_cert_file)"
  key_file="$(api_gateway_key_file)"

  if [[ ! -f "$cert_file" || ! -f "$key_file" ]]; then
    echo "Error: API gateway certificate files were not found." >&2
    echo "Expected: $cert_file" >&2
    echo "Expected: $key_file" >&2
    return 1
  fi

  ensure_k8s_namespace
  kubectl -n "${K8S_NAMESPACE}" create secret generic "${API_GATEWAY_CERT_SECRET_NAME}" \
    --from-file="${API_GATEWAY_CERT_FILE_NAME}=${cert_file}" \
    --from-file="${API_GATEWAY_KEY_FILE_NAME}=${key_file}" \
    --dry-run=client -o yaml | kubectl apply -f -
}

deploy_k8s_resources() {
  require_command kubectl
  require_command helm

  ensure_helm_chart "${BACKEND_HELM_CHART_DIR}"
  ensure_helm_chart "${API_GATEWAY_HELM_CHART_DIR}"
  ensure_k8s_namespace
  ensure_api_gateway_cert_secret

  helm upgrade --install "${BACKEND_HELM_RELEASE_NAME}" "${BACKEND_HELM_CHART_DIR}" \
    --namespace "${K8S_NAMESPACE}" \
    --wait \
    --timeout "${HELM_TIMEOUT}" \
    --set "namespace.name=${K8S_NAMESPACE}" \
    --set "fullnameOverride=${BACKEND_CONTAINER_NAME}" \
    --set-string "image.repository=${NAMESPACE}/${REPOSITORY_NAME}/backend" \
    --set-string "image.tag=${BACKEND_TAG}" \
    --set "containerPort=${BACKEND_CONTAINER_PORT}" \
    --set "service.port=${BACKEND_CONTAINER_PORT}" \
    --set-string "ingress.hosts[0].host=${BACKEND_HOSTNAME}"

  helm upgrade --install "${API_GATEWAY_HELM_RELEASE_NAME}" "${API_GATEWAY_HELM_CHART_DIR}" \
    --namespace "${K8S_NAMESPACE}" \
    --wait \
    --timeout "${HELM_TIMEOUT}" \
    --set "namespace.name=${K8S_NAMESPACE}" \
    --set "fullnameOverride=${API_GATEWAY_CONTAINER_NAME}" \
    --set-string "image.tag=${ENVOY_TAG}" \
    --set "containerPorts.https=${API_GATEWAY_HTTPS_PORT}" \
    --set "containerPorts.admin=${API_GATEWAY_ADMIN_PORT}" \
    --set "service.httpsPort=${API_GATEWAY_HTTPS_PORT}" \
    --set "service.adminPort=${API_GATEWAY_ADMIN_PORT}" \
    --set-string "gateway.host=${BACKEND_HOSTNAME}" \
    --set "gateway.externalPort=${GATEWAY_PORT}" \
    --set-string "backend.serviceName=${BACKEND_CONTAINER_NAME}" \
    --set "backend.servicePort=${BACKEND_CONTAINER_PORT}" \
    --set-string "tls.existingSecret=${API_GATEWAY_CERT_SECRET_NAME}" \
    --set-string "tls.certFileName=${API_GATEWAY_CERT_FILE_NAME}" \
    --set-string "tls.keyFileName=${API_GATEWAY_KEY_FILE_NAME}" \
    --set-string "ingress.hosts[0].host=${BACKEND_HOSTNAME}"

  kubectl -n "${K8S_NAMESPACE}" rollout status deployment/"${BACKEND_CONTAINER_NAME}" --timeout="${HELM_TIMEOUT}"
  kubectl -n "${K8S_NAMESPACE}" rollout status deployment/"${API_GATEWAY_CONTAINER_NAME}" --timeout="${HELM_TIMEOUT}"
}

delete_k8s_resources() {
  require_command kubectl
  require_command helm

  if helm status "${API_GATEWAY_HELM_RELEASE_NAME}" --namespace "${K8S_NAMESPACE}" >/dev/null 2>&1; then
    helm uninstall "${API_GATEWAY_HELM_RELEASE_NAME}" --namespace "${K8S_NAMESPACE}" --wait --timeout "${HELM_TIMEOUT}"
  else
    echo "Helm release '${API_GATEWAY_HELM_RELEASE_NAME}' was not found in namespace '${K8S_NAMESPACE}'."
  fi

  if helm status "${BACKEND_HELM_RELEASE_NAME}" --namespace "${K8S_NAMESPACE}" >/dev/null 2>&1; then
    helm uninstall "${BACKEND_HELM_RELEASE_NAME}" --namespace "${K8S_NAMESPACE}" --wait --timeout "${HELM_TIMEOUT}"
  else
    echo "Helm release '${BACKEND_HELM_RELEASE_NAME}' was not found in namespace '${K8S_NAMESPACE}'."
  fi

  kubectl -n "${K8S_NAMESPACE}" delete secret "${API_GATEWAY_CERT_SECRET_NAME}" --ignore-not-found=true
  kubectl delete namespace "${K8S_NAMESPACE}" --ignore-not-found=true
}

delete_kind_cluster() {
  require_command kind

  kind delete cluster --name "${K8S_CLUSTER_NAME}"
}

port_forward_api_gateway() {
  require_command kubectl

  kubectl -n "${K8S_NAMESPACE}" port-forward "svc/${API_GATEWAY_CONTAINER_NAME}" "${GATEWAY_PORT}:${API_GATEWAY_HTTPS_PORT}"
}
