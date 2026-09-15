#!/usr/bin/env bash
set -euo pipefail

# --------- Files and folders generation functions ---------
create_data_folders() {
  echo "Creating data folders"

  create_dir true "$DATA_DIR"
  create_dir true "$GRAFANA_DATA_DIR"
  create_dir true "$LOKI_DATA_DIR"
  create_dir true "$TEMPO_DATA_DIR"
  create_dir true "$PROMETHEUS_DATA_DIR"
  create_dir true "$POSTGRES_DATA_DIR"
  
  # TODO: Add more data folders as needed
}

create_env_file() {
  echo "Creating env file"

  : > "${ENV_DIR}/.env"

  local vars=(
    MODE
    LOCAL_IP
    NAMESPACE
    CERT_SECRET

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
    "${SERVICES_DIR}/envoy/templates/gateway.template:${SERVICES_DIR}/envoy/gateway.yaml"
    "${SERVICES_DIR}/envoy/templates/gateway.local.template:${SERVICES_DIR}/envoy/gateway.local.yaml"

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

# --------- Secrets generation functions ---------\

# ===== Example usage ===== 
# generate_ca "LDNhanCA" 
# generate_ca "ExampleCA" 730

generate_ca() {
  local ca_alias=${1:?ca_alias is not set}
  local ca_days=${2:-365}
  local ca_dir="${CERTS_DIR:?CERTS_DIR is not set}/ca"
  local ca_key="$ca_dir/$ca_alias.ca.key"
  local ca_cert="$ca_dir/$ca_alias.ca.crt"
  local ca_conf="$ca_dir/$ca_alias.ca.openssl.cnf"

  mkdir -p "$ca_dir"

  echo "Generating Root CA..."

  openssl genpkey \
    -algorithm RSA \
    -pkeyopt rsa_keygen_bits:4096 \
    -out "$ca_key"

  cat > "$ca_conf" <<EOF
[req]
default_md = sha512
prompt = no
distinguished_name = dn
x509_extensions = v3_ca

[dn]
C = $SUBJ_C
ST = $SUBJ_ST
L = $SUBJ_L
O = $SUBJ_O
OU = $SUBJ_OU
CN = $ca_alias

[v3_ca]
basicConstraints = critical, CA:TRUE
keyUsage = critical, keyCertSign, cRLSign
subjectKeyIdentifier = hash
authorityKeyIdentifier = keyid:always,issuer
EOF

  openssl req -x509 -new \
    -key "$ca_key" \
    -days "$ca_days" \
    -config "$ca_conf" \
    -out "$ca_cert"

  chmod 600 "$ca_key"
  chmod 644 "$ca_cert"

  echo "Root CA generated:"
  echo "  Key : $ca_key"
  echo "  Cert: $ca_cert"
}

# ===== Example usage =====
# export CERT_SECRET="your-pass"
# generate_server_cert "LDNhanCA" "server" "example.com" "rest_api.example.com" "admin.example.com"

generate_pkcs12_stores() {
  local ca_alias=${1:?ca_alias is not set}
  local certs_dir=${2:?certs_dir is not set}
  local alias_name=${3:?alias_name is not set}
  local cert_secret=${CERT_SECRET:?CERT_SECRET is not set}

  local ca_cert="$CERTS_DIR/ca/$ca_alias.ca.crt"
  local key="$certs_dir/$alias_name.key.pem"
  local cert="$certs_dir/$alias_name.cert.pem"

  openssl pkcs12 -export \
    -inkey "$key" \
    -in "$cert" \
    -certfile "$ca_cert" \
    -passout pass:"$cert_secret" \
    -out "$certs_dir/$alias_name.keystore.p12" \
    -name "$alias_name"

  keytool -importcert \
    -noprompt \
    -trustcacerts \
    -alias "$ca_alias" \
    -file "$ca_cert" \
    -keystore "$certs_dir/$alias_name.truststore.p12" \
    -storetype PKCS12 \
    -storepass "$cert_secret"

  ln -sf "$alias_name.keystore.p12" "$certs_dir/keystore.p12"
  ln -sf "$alias_name.truststore.p12" "$certs_dir/truststore.p12"
}

generate_server_cert() {
  local ca_alias=${1:?ca_alias is not set}
  local certs_dir="$CERTS_DIR/${2:?certs_dir is not set}"
  local main_domain=${3:?main_domain is not set}
  local local_ip=${LOCAL_IP:?LOCAL_IP is not set}
  local cert_secret=${CERT_SECRET:?CERT_SECRET is not set}

  shift 3
  local sub_domains=("$@")

  local alias_name="${main_domain%%.*}"
  local ca_dir="$CERTS_DIR/ca"
  local ca_cert="$ca_dir/$ca_alias.ca.crt"
  local ca_key="$ca_dir/$ca_alias.ca.key"

  local key="$certs_dir/$alias_name.key.pem"
  local csr="$certs_dir/$alias_name.csr.pem"
  local cert="$certs_dir/$alias_name.cert.pem"
  local conf="$certs_dir/$alias_name.openssl.cnf"

  mkdir -p "$certs_dir"
  rm -rf "${certs_dir:?}"/*

  echo "Generating server key..."
  openssl genpkey \
    -algorithm RSA \
    -pkeyopt rsa_keygen_bits:4096 \
    -out "$key"

  local san_text
  san_text=$(
    cat <<EOF
IP.1 = $local_ip
IP.2 = 127.0.0.1
DNS.3 = $alias_name
DNS.4 = $main_domain
DNS.5 = $main_domain.svc
DNS.6 = $main_domain.svc.cluster.local
DNS.7 = localhost
DNS.8 = host.docker.internal
EOF
  )

  local count=9
  for sub in "${sub_domains[@]}"; do
    [[ -n "$sub" ]] || continue
    san_text+="
DNS.$count = $sub"
    ((count++))
  done

  cat > "$conf" <<EOF
[req]
prompt = no
default_md = sha512
distinguished_name = dn
req_extensions = req_ext

[dn]
C = $SUBJ_C
ST = $SUBJ_ST
L = $SUBJ_L
O = $SUBJ_O
OU = $SUBJ_OU
CN = $main_domain

[req_ext]
subjectAltName = @alt_names
extendedKeyUsage = serverAuth
keyUsage = digitalSignature, keyEncipherment

[alt_names]
$san_text
EOF

  echo "Generating CSR..."
  openssl req -new \
    -key "$key" \
    -out "$csr" \
    -config "$conf"

  echo "Signing certificate..."
  openssl x509 -req \
    -in "$csr" \
    -CA "$ca_cert" \
    -CAkey "$ca_key" \
    -CAcreateserial \
    -out "$cert" \
    -days 365 \
    -sha512 \
    -extfile "$conf" \
    -extensions req_ext

  echo "Converting private key to PKCS8 DER..."
  openssl pkcs8 \
    -topk8 \
    -inform PEM \
    -outform DER \
    -in "$key" \
    -out "$certs_dir/$alias_name.pk8" \
    -nocrypt

  echo "Creating PKCS#12 keystore and truststore..."
  generate_pkcs12_stores "$ca_alias" "$certs_dir" "$alias_name"

  chmod 644 "$certs_dir"/*.pem "$certs_dir"/*.pk8 "$certs_dir"/*.p12

  echo "[$main_domain] Certificate generation complete!"
}

generate_client_cert() {
  local ca_alias=${1:?ca_alias is not set}
  local certs_dir="$CERTS_DIR/${2:?certs_dir is not set}"
  local client_name=${3:?client_name is not set}

  shift 3
  local client_domains=("$@")

  local ca_dir="$CERTS_DIR/ca"
  local ca_cert="$ca_dir/$ca_alias.ca.crt"
  local ca_key="$ca_dir/$ca_alias.ca.key"

  local key="$certs_dir/$client_name.key.pem"
  local csr="$certs_dir/$client_name.csr.pem"
  local cert="$certs_dir/$client_name.cert.pem"
  local conf="$certs_dir/$client_name.openssl.cnf"

  mkdir -p "$certs_dir"
  rm -rf "${certs_dir:?}"/*

  echo "Generating client key..."
  openssl genpkey \
    -algorithm RSA \
    -pkeyopt rsa_keygen_bits:4096 \
    -out "$key"

  local san_text=""
  local count=1

  for domain in "${client_domains[@]}"; do
    [[ -n "$domain" ]] || continue

    san_text+="DNS.$count = $domain
"
    ((count++))
  done

  cat > "$conf" <<EOF
[req]
prompt = no
default_md = sha512
distinguished_name = dn
req_extensions = req_ext

[dn]
C = $SUBJ_C
ST = $SUBJ_ST
L = $SUBJ_L
O = $SUBJ_O
OU = $SUBJ_OU
CN = $client_name

[req_ext]
subjectAltName = @alt_names
extendedKeyUsage = clientAuth
keyUsage = digitalSignature, keyEncipherment

[alt_names]
$san_text
EOF

  echo "Generating client CSR..."
  openssl req -new \
    -key "$key" \
    -out "$csr" \
    -config "$conf"

  echo "Signing client certificate..."
  openssl x509 -req \
    -in "$csr" \
    -CA "$ca_cert" \
    -CAkey "$ca_key" \
    -CAcreateserial \
    -out "$cert" \
    -days 365 \
    -sha512 \
    -extfile "$conf" \
    -extensions req_ext

  echo "Converting private key to PKCS8 DER..."
  openssl pkcs8 \
    -topk8 \
    -inform PEM \
    -outform DER \
    -in "$key" \
    -out "$certs_dir/$client_name.pk8" \
    -nocrypt

  chmod 644 \
    "$certs_dir"/*.pem \
    "$certs_dir"/*.pk8

  echo "[$client_name] Client certificate generation complete!"
}

generate_tls_certs() {
  local grafana_namespace="monitoring"
  local keycloak_namespace="keycloak-cluster"
  local kafka_namespace="kafka-cluster"
    
  local grafana_ca_alias="grafana"
  local keycloak_ca_alias="keycloak"
  local kafka_ca_alias="kafka"
  
  generate_ca "$grafana_ca_alias"
  generate_ca "$keycloak_ca_alias"
  generate_ca "$kafka_ca_alias"
  
  generate_server_cert "$grafana_ca_alias" "grafana" "grafana.${grafana_namespace}"
  generate_server_cert "$grafana_ca_alias" "prometheus" "prometheus.${grafana_namespace}" "monitoring-kube-prometheus-prometheus" "monitoring-kube-prometheus-prometheus.monitoring" "monitoring-kube-prometheus-prometheus.monitoring.svc" "monitoring-kube-prometheus-prometheus.monitoring.svc.cluster.local"
  generate_server_cert "$grafana_ca_alias" "alertmanager" "alertmanager.${grafana_namespace}" "monitoring-kube-prometheus-alertmanager" "monitoring-kube-prometheus-alertmanager.monitoring" "monitoring-kube-prometheus-alertmanager.monitoring.svc" "monitoring-kube-prometheus-alertmanager.monitoring.svc.cluster.local"
  
  generate_server_cert "$keycloak_ca_alias" "keycloak0" "keycloak0.${keycloak_namespace}"
  generate_server_cert "$kafka_ca_alias" "kafka0" "kafka0.${kafka_namespace}"
  generate_server_cert "$kafka_ca_alias" "kafka1" "kafka1.${kafka_namespace}"
  generate_server_cert "$kafka_ca_alias" "kafka2" "kafka2.${kafka_namespace}"
  
  generate_client_cert "$grafana_ca_alias" "grafana" "grafana-client"
}

# ===== Example usage =====
# generate_jwt_keypair auth auth-service
generate_jwt_keypair() {
  local output_dir="${1:-.}"
  local name="${2:-}"

  if [[ -z "$name" ]]; then
    echo "Error: key name is required"
    return 1
  fi

  if ! command -v openssl >/dev/null 2>&1; then
    echo "Error: openssl is not installed"
    return 1
  fi

  mkdir -p "${KEYPAIR_DIR}/${output_dir}"

  local private_key="${KEYPAIR_DIR}/${output_dir}/${name}.key.pem"
  local public_key="${KEYPAIR_DIR}/${output_dir}/${name}.pub.pem"

  echo "Generating RSA keypair for JWT (RS256)..."

  # Generate private key
  openssl genpkey \
    -algorithm RSA \
    -pkeyopt rsa_keygen_bits:2048 \
    -out "$private_key"

  # Extract public key
  openssl rsa \
    -pubout \
    -in "$private_key" \
    -out "$public_key"

  chmod 600 "$private_key"
  chmod 644 "$public_key"

  echo "Private key: $private_key"
  echo "Public key : $public_key"
}

# --------- Docker image generation functions ---------
backend_image_name() {
  echo "${NAMESPACE}/${REPOSITORY_NAME}/backend:${BACKEND_TAG}"
}

keycloak_image_name() {
  echo "${NAMESPACE}/${REPOSITORY_NAME}/keycloak:${KEYCLOAK_TAG}"
}

postgres_image_name() {
  echo "${NAMESPACE}/${REPOSITORY_NAME}/postgres:${POSTGRES_TAG}"
}

build_backend_image() {
  local backend_dir=${1:?backend_dir is required}
  local image_name
  image_name="$(backend_image_name)"

  require_command docker
  
  cp "${CERTS_DIR}/ca/ca.crt" "${backend_dir}/rootCA.crt"

  docker rmi "$image_name" || true
  docker build --no-cache \
    --build-arg BACKEND_TAG="${BACKEND_TAG}" \
    -f "${backend_dir}/Modules/Restaurant/Restaurant.API/Docker/Dockerfile" \
    -t "$image_name" \
    "${backend_dir}"
}

build_keycloak_image() {
  local env_dir=${1:?env_dir is required}
  local image_name
  image_name="$(keycloak_image_name)"

  require_command docker

  docker rmi "$image_name" || true
  docker build --no-cache \
    --build-arg KEYCLOAK_TAG="${KEYCLOAK_TAG}" \
    -f "${env_dir}/docker/keycloak/Dockerfile" \
    -t "$image_name" \
    "${env_dir}"
}

build_postgres_image() {
  local env_dir=${1:?env_dir is required}
  local image_name
  image_name="$(postgres_image_name)"

  require_command docker

  docker rmi "$image_name" || true
  docker build --no-cache \
    --build-arg POSTGRES_TAG="${POSTGRES_TAG}" \
    -f "${env_dir}/docker/postgres/Dockerfile" \
    -t "$image_name" \
    "${env_dir}"
}
