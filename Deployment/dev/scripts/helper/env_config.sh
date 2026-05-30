export NAMESPACE="leduynhan1201"
export REPOSITORY_NAME="learn-dot-net"
export BACKEND_TAG="1.0.0"

export ENVOY_REPO_NAME="envoyproxy" 
export GRAFANA_REPO_NAME="grafana" 
export LOKI_REPO_NAME="grafana" 
export TEMPO_REPO_NAME="grafana" 
export PROMETHEUS_REPO_NAME="prom"
export OTEL_REPO_NAME="otel"
export KEYCLOAK_REPO_NAME="quay.io/keycloak"
export POSTGRES_REPO_NAME=""

export ENVOY_TAG="tools-dev" # https://hub.docker.com/r/envoyproxy/envoy/tags
export GRAFANA_TAG="main-distroless-slim" # https://hub.docker.com/r/grafana/grafana/tags
export LOKI_TAG="3.7.2" # https://hub.docker.com/r/grafana/loki/tags
export TEMPO_TAG="2.9.2" # https://hub.docker.com/r/grafana/tempo/tags
export PROMETHEUS_TAG="main-distroless" # https://hub.docker.com/r/prom/prometheus/tags
export OTEL_TAG="nightly" # https://hub.docker.com/r/otel/opentelemetry-collector-contrib/tags
export KEYCLOAK_TAG="26.6" # https://quay.io/repository/keycloak/keycloak?tab=tags
export POSTGRES_TAG="14.23" # https://hub.docker.com/_/postgres/tags

# For hardened images, use the following repository names and tags:
#export ENVOY_REPO_NAME="dhi" 
#export GRAFANA_REPO_NAME="dhi" 
#export LOKI_REPO_NAME="dhi" 
#export TEMPO_REPO_NAME="dhi" 
#export PROMETHEUS_REPO_NAME="dhi"
#export OTEL_REPO_NAME="dhi"
#export ENVOY_TAG="1-debian13-dev" # https://hub.docker.com/hardened-images/catalog/dhi/envoy
#export GRAFANA_TAG="13-debian13-dev" # https://hub.docker.com/hardened-images/catalog/dhi/grafana
#export LOKI_TAG="3" # https://hub.docker.com/hardened-images/catalog/dhi/loki
#export TEMPO_TAG="2" # https://hub.docker.com/hardened-images/catalog/dhi/tempo
#export PROMETHEUS_TAG="3.11" # https://hub.docker.com/hardened-images/catalog/dhi/prometheus
#export OTEL_TAG="0-contrib-dev" # https://hub.docker.com/hardened-images/catalog/dhi/opentelemetry-collector

LOCAL_IP="${LOCAL_IP:-$(hostname -I 2>/dev/null | awk '{print $1}')}"
LOCAL_IP="${LOCAL_IP:-127.0.0.1}"
export LOCAL_IP
export GATEWAY_PORT=60000
export APP_HOSTNAME="${NAMESPACE}.${REPOSITORY_NAME}.${MODE}"
export CA_NAME="LDNhanRootCA"
export SUBJ_C="VN"
export SUBJ_ST="BinhTriDong"
export SUBJ_L="HCM"
export SUBJ_O="SGU"
export SUBJ_OU="Dev"

export CERT_SECRET='@N120103#'
export ENV_DIR="${ENV_DIR:-$(pwd)}"
SERVICES_DIR="$(cd -- "${ENV_DIR}/../services" && pwd)" && export SERVICES_DIR
export SECRETS_DIR="${SECRETS_DIR:-${ENV_DIR}/secrets}"
export DATA_DIR="${DATA_DIR:-${ENV_DIR}/data}"
export CERTS_DIR="${CERTS_DIR:-${ENV_DIR}/certs}"
export KEYPAIR_DIR="${KEYPAIR_DIR:-${ENV_DIR}/keypair}"
export GRAFANA_DATA_DIR="${GRAFANA_DATA_DIR:-${DATA_DIR}/grafana}"
export LOKI_DATA_DIR="${LOKI_DATA_DIR:-${DATA_DIR}/loki}"
export TEMPO_DATA_DIR="${TEMPO_DATA_DIR:-${DATA_DIR}/tempo}"
export PROMETHEUS_DATA_DIR="${PROMETHEUS_DATA_DIR:-${DATA_DIR}/prometheus}"
export POSTGRES_DATA_DIR="${POSTGRES_DATA_DIR:-${DATA_DIR}/postgres}"

export GATEWAY_CERT_SECRET_NAME="gateway-certs"
export GATEWAY_CERT_FILE_NAME="gateway.cert.pem"
export GATEWAY_KEY_FILE_NAME="gateway.key.pem"
export GATEWAY_HTTPS_PORT=443
export GATEWAY_ADMIN_PORT=9901

export POSTGRES_PORT=5438
export POSTGRES_USER="leduynhan1201"
export POSTGRES_PASSWORD='@N120103#'

export KEYCLOAK_PORT=8081
export KEYCLOAK_MANAGEMENT_PORT=9991
export KEYCLOAK_JGROUPS_PORT=7801
export KC_BOOTSTRAP_ADMIN_USERNAME="leduynhan1201"
export KC_BOOTSTRAP_ADMIN_PASSWORD='@N120103#'

export GRAFANA_PORT=3001
export GRAFANA_ADMIN_USER="${GRAFANA_ADMIN_USER:-admin}"
export GRAFANA_ADMIN_PASSWORD="${GRAFANA_ADMIN_PASSWORD:-admin}"

export LOKI_PORT=3100
export TEMPO_PORT=3200

export PROMETHEUS_PORT=9090
export PROMETHEUS_RETENTION_TIME="${PROMETHEUS_RETENTION_TIME:-15d}"
export PROMETHEUS_RETENTION_SIZE="${PROMETHEUS_RETENTION_SIZE:-2GB}"

export OTEL_COLLECTOR_OTLP_GRPC_PORT=4317
export OTEL_COLLECTOR_OTLP_HTTP_PORT=4318
export OTEL_COLLECTOR_HEALTH_PORT=13133

export APP_IDP_URL=https://${APP_HOSTNAME}:${GATEWAY_PORT}/auth
export APP_IDP_REALM=learn-dot-net
export APP_IDP_TOKEN_ENDPOINT=${APP_IDP_URL}realms/${APP_IDP_REALM}/protocol/openid-connect/token
export APP_IDP_JWKS_ENDPOINT=${APP_IDP_URL}realms/${APP_IDP_REALM}/protocol/openid-connect/certs
export APP_IDP_EXPECTED_ISSUER=${APP_IDP_URL}realms/${APP_IDP_REALM}
export APP_IDP_AUTH_ENDPOINT=${APP_IDP_URL}realms/${APP_IDP_REALM}/protocol/openid-connect/auth
export APP_IDP_AUTH_DEVICE_ENDPOINT=${APP_IDP_URL}realms/${APP_IDP_REALM}/protocol/openid-connect/auth/device
export APP_IDP_SUB_CLAIM_NAME=sub
export APP_IDP_SCOPE_CLAIM_NAME=scope
export APP_IDP_GROUP_CLAIM_NAME=groups
export APP_IDP_EXPECTED_AUDIENCE=account

export OBSERVABILITY_UID="${OBSERVABILITY_UID:-$(id -u 2>/dev/null || printf '1000')}"
export OBSERVABILITY_GID="${OBSERVABILITY_GID:-$(id -g 2>/dev/null || printf '1000')}"

export DOCKER_LOG_MAX_SIZE="${DOCKER_LOG_MAX_SIZE:-10m}"
export DOCKER_LOG_MAX_FILE="${DOCKER_LOG_MAX_FILE:-3}"

export K8S_CLUSTER_NAME="learn-dot-net"
export K8S_NAMESPACE="learn-dot-net"
export K8S_HELM_DIR="${K8S_HELM_DIR:-${ENV_DIR}/k8s-helm}"
export HELM_TIMEOUT="${HELM_TIMEOUT:-120s}"

export BACKEND_HELM_RELEASE_NAME="${BACKEND_HELM_RELEASE_NAME:-backend}"
export GATEWAY_HELM_RELEASE_NAME="${GATEWAY_HELM_RELEASE_NAME:-api-gateway}"

export BACKEND_HELM_CHART_DIR="${BACKEND_HELM_CHART_DIR:-${K8S_HELM_DIR}/backend}"
export GATEWAY_HELM_CHART_DIR="${GATEWAY_HELM_CHART_DIR:-${K8S_HELM_DIR}/api-gateway}"


