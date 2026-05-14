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

  kubectl create namespace "${K8S_NAMESPACE}" --dry-run=client -o yaml | kubectl apply -f -
}

load_backend_image_to_kind() {
  require_command kind

  kind load docker-image "$(backend_image_name)" --name "${K8S_CLUSTER_NAME}"
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

# --------- API Gateway TLS Cert Secret Management --------
api_gateway_cert_file() {
  echo "${CERTS_DIR}/${API_GATEWAY_CONTAINER_NAME}/${API_GATEWAY_CERT_FILE_NAME}"
}

api_gateway_key_file() {
  echo "${CERTS_DIR}/${API_GATEWAY_CONTAINER_NAME}/${API_GATEWAY_KEY_FILE_NAME}"
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

port_forward_api_gateway() {
  require_command kubectl

  kubectl -n "${K8S_NAMESPACE}" port-forward "svc/${API_GATEWAY_CONTAINER_NAME}" "${GATEWAY_PORT}:${API_GATEWAY_HTTPS_PORT}"
}
