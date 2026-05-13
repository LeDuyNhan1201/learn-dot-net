export NAMESPACE="leduynhan1201"
export REPOSITORY_NAME="learn-dot-net"

export ENVOY_TAG=tools-dev # https://hub.docker.com/r/envoyproxy/envoy/tags
export BACKEND_TAG="1.0.0"

LOCAL_IP=$(hostname -I | awk '{print $1}')
export LOCAL_IP
export GATEWAY_PORT=60000
export BACKEND_HOSTNAME="backend.${REPOSITORY_NAME}.${MODE}"
export CA_NAME="LDNhanRootCA"
export SUBJ_C="VN"
export SUBJ_ST="BinhTriDong"
export SUBJ_L="HCM"
export SUBJ_O="SGU"
export SUBJ_OU="Dev"

export CERT_SECRET='@N120103#'
export ENV_DIR="${ENV_DIR:-$(pwd)}"
export SECRETS_DIR="${SECRETS_DIR:-${ENV_DIR}/secrets}"
export DATA_DIR="${DATA_DIR:-${ENV_DIR}/data}"
export CERTS_DIR="${CERTS_DIR:-${ENV_DIR}/certs}"
export KEYPAIR_DIR="${KEYPAIR_DIR:-${ENV_DIR}/keypair}"

export API_GATEWAY_CONTAINER_NAME="api-gateway"
export API_GATEWAY_CERT_SECRET_NAME="api-gateway-certs"
export API_GATEWAY_CERT_FILE_NAME="api-gateway.cert.pem"
export API_GATEWAY_KEY_FILE_NAME="api-gateway.key.pem"
export API_GATEWAY_HTTPS_PORT=443
export API_GATEWAY_ADMIN_PORT=9901
export BACKEND_CONTAINER_NAME="backend"
export BACKEND_CONTAINER_PORT=60001

export K8S_CLUSTER_NAME="learn-dot-net"
export K8S_NAMESPACE="learn-dot-net"
export K8S_HELM_DIR="${K8S_HELM_DIR:-${ENV_DIR}/k8s-helm}"
export BACKEND_HELM_RELEASE_NAME="${BACKEND_HELM_RELEASE_NAME:-backend}"
export API_GATEWAY_HELM_RELEASE_NAME="${API_GATEWAY_HELM_RELEASE_NAME:-api-gateway}"
export BACKEND_HELM_CHART_DIR="${BACKEND_HELM_CHART_DIR:-${K8S_HELM_DIR}/backend}"
export API_GATEWAY_HELM_CHART_DIR="${API_GATEWAY_HELM_CHART_DIR:-${K8S_HELM_DIR}/api-gateway}"
export HELM_TIMEOUT="${HELM_TIMEOUT:-120s}"
