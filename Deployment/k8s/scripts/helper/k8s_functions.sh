#!/usr/bin/env bash
set -euo pipefail

# --------- Kind Cluster Management --------
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

ensure_k8s_namespace() {
  require_command kubectl

  if ! kubectl get namespace "$K8S_NAMESPACE" >/dev/null 2>&1; then
    kubectl create namespace "$K8S_NAMESPACE"
  fi
}

load_backend_image_to_kind() {
  require_command kind

  kind load docker-image "$(backend_image_name)" --name "${K8S_CLUSTER_NAME}"
}

deploy_k8s_resources() {
  require_command kubectl
  require_command helm

  [ -d "${BACKEND_HELM_CHART_DIR}" ] || {
    echo "Backend chart not found"
    exit 1
  }

  [ -d "${GATEWAY_HELM_CHART_DIR}" ] || {
    echo "Gateway chart not found"
    exit 1
  }

  ensure_k8s_namespace
  ensure_api_gateway_cert_secret

  echo "Deploy backend..."
  helm upgrade --install "${BACKEND_HELM_RELEASE_NAME}" "${BACKEND_HELM_CHART_DIR}" \
    --namespace "${K8S_NAMESPACE}" \
    --wait \
    --timeout "${HELM_TIMEOUT}"

  echo "Deploy gateway..."
  helm upgrade --install "${GATEWAY_HELM_RELEASE_NAME}" "${GATEWAY_HELM_CHART_DIR}" \
    --namespace "${K8S_NAMESPACE}" \
    --wait \
    --timeout "${HELM_TIMEOUT}" \
    --set-string "tls.existingSecret=${GATEWAY_CERT_SECRET_NAME}" \
    --set-string "tls.certFileName=${GATEWAY_CERT_FILE_NAME}" \
    --set-string "tls.keyFileName=${GATEWAY_KEY_FILE_NAME}"

  echo "Waiting backend rollout..."
  kubectl -n "${K8S_NAMESPACE}" rollout status deployment/"${BACKEND_HELM_RELEASE_NAME}" \
    --timeout="${HELM_TIMEOUT}"

  echo "Waiting gateway rollout..."
  kubectl -n "${K8S_NAMESPACE}" rollout status deployment/"${GATEWAY_HELM_RELEASE_NAME}" \
    --timeout="${HELM_TIMEOUT}"
}

delete_k8s_resources() {
  require_command kubectl
  require_command helm

  echo "Uninstalling gateway..."
  helm uninstall "${GATEWAY_HELM_RELEASE_NAME}" \
    --namespace "${K8S_NAMESPACE}" \
    --wait \
    --timeout "${HELM_TIMEOUT}" \
    || true

  echo "Uninstalling backend..."
  helm uninstall "${BACKEND_HELM_RELEASE_NAME}" \
    --namespace "${K8S_NAMESPACE}" \
    --wait \
    --timeout "${HELM_TIMEOUT}" \
    || true

  echo "Deleting secrets..."
  kubectl -n "${K8S_NAMESPACE}" delete secret "${GATEWAY_CERT_SECRET_NAME}" --ignore-not-found=true

  echo "Deleting namespace..."
  kubectl delete namespace "${K8S_NAMESPACE}" --ignore-not-found=true

  echo "Waiting namespace termination..."
  kubectl wait --for=delete namespace/"${K8S_NAMESPACE}" --timeout="${HELM_TIMEOUT}" 2>/dev/null || true
}

delete_kind_cluster() {
  require_command kind

  echo "Deleting kind cluster: ${K8S_CLUSTER_NAME}"
  kind delete cluster --name "${K8S_CLUSTER_NAME}"
}

# --------- API Gateway TLS Cert Secret Management --------
api_gateway_cert_file() {
  echo "${CERTS_DIR}/${GATEWAY_CONTAINER_NAME}/${GATEWAY_CERT_FILE_NAME}"
}

api_gateway_key_file() {
  echo "${CERTS_DIR}/${GATEWAY_CONTAINER_NAME}/${GATEWAY_KEY_FILE_NAME}"
}

ensure_api_gateway_cert_secret() {
  require_command kubectl

  local cert_file key_file
  cert_file="$(api_gateway_cert_file)"
  key_file="$(api_gateway_key_file)"

  if [[ ! -f "$cert_file" || ! -f "$key_file" ]]; then
    echo "Error: API gateway certificate files not found" >&2
    echo "cert: $cert_file" >&2
    echo "key : $key_file" >&2
    return 1
  fi

  kubectl -n "${K8S_NAMESPACE}" create secret generic "${GATEWAY_CERT_SECRET_NAME}" \
    --from-file="${GATEWAY_CERT_FILE_NAME}=${cert_file}" \
    --from-file="${GATEWAY_KEY_FILE_NAME}=${key_file}" \
    --dry-run=client -o yaml | kubectl apply -f -
}

port_forward_api_gateway() {
  require_command kubectl

  echo "Starting port-forward..."

  kubectl -n "${K8S_NAMESPACE}" port-forward \
    "svc/${GATEWAY_HELM_RELEASE_NAME}" \
    "${GATEWAY_PORT}:${GATEWAY_HTTPS_PORT}" \
    >/tmp/gateway-pf.log 2>&1 &

  echo $! > /tmp/gateway-pf.pid
}
