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
