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
  
  # TODO: Add more data folders as needed
}

create_env_file() {
  echo "Creating env file"

  : > "${ENV_DIR}/.env"

  local vars=(
    MODE
    LOCAL_IP
    GATEWAY_PORT
    APP_HOSTNAME

    NAMESPACE
    REPOSITORY_NAME
    BACKEND_TAG
    
    ENVOY_REPO_NAME
    GRAFANA_REPO_NAME
    LOKI_REPO_NAME
    TEMPO_REPO_NAME
    PROMETHEUS_REPO_NAME
    OTEL_REPO_NAME
        
    ENVOY_TAG
    GRAFANA_TAG
    LOKI_TAG
    TEMPO_TAG
    PROMETHEUS_TAG
    OTEL_TAG

    CERT_SECRET
    DATA_DIR
    SERVICES_DIR
    
    GRAFANA_DATA_DIR
    LOKI_DATA_DIR
    TEMPO_DATA_DIR
    PROMETHEUS_DATA_DIR

    GATEWAY_CERT_SECRET_NAME
    GATEWAY_CERT_FILE_NAME
    GATEWAY_KEY_FILE_NAME
    GATEWAY_HTTPS_PORT
    GATEWAY_ADMIN_PORT

    GRAFANA_PORT
    GRAFANA_ADMIN_USER
    GRAFANA_ADMIN_PASSWORD
  
    LOKI_PORT
    TEMPO_PORT
    
    PROMETHEUS_PORT
    PROMETHEUS_RETENTION_TIME
    PROMETHEUS_RETENTION_SIZE
    
    OTEL_COLLECTOR_OTLP_GRPC_PORT
    OTEL_COLLECTOR_OTLP_HTTP_PORT
    OTEL_COLLECTOR_HEALTH_PORT
    
    OBSERVABILITY_UID
    OBSERVABILITY_GID
    
    DOCKER_LOG_MAX_SIZE
    DOCKER_LOG_MAX_FILE

    K8S_CLUSTER_NAME
    K8S_NAMESPACE
    K8S_HELM_DIR
    
    BACKEND_HELM_CHART_DIR
    GATEWAY_HELM_CHART_DIR
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
    "${ENV_DIR}/services/envoy/templates/gateway.template:${ENV_DIR}/services/envoy/gateway.yaml"
    "${ENV_DIR}/services/envoy/templates/gateway.local.template:${ENV_DIR}/services/envoy/gateway.local.yaml"

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

# --------- Secrets generation functions ---------

# ===== Example usage =====
# generate_root_ca "LDNhanCA"
# generate_root_ca "ExampleCA" 730
generate_root_ca() {
  local ca_name=${CA_NAME:? CA_NAME is not set}
  local ca_days="${1:-3650}"   # default 10 years
  local certs_dir=${CERTS_DIR:? CERTS_DIR is not set}

  local ca_dir="$certs_dir/ca"
  local ca_key="$ca_dir/ca.key"
  local ca_cert="$ca_dir/ca.crt"

  # === Prepare CA folder ===
  if [[ -d "$ca_dir" ]]; then
    rm -rf "${ca_dir:? ca_dir is not set}"/*
  else
    mkdir -p "$ca_dir"
  fi

  # ============================================================
  # === Generate Root CA private key (RSA 4096) ===============
  # ============================================================
  echo "Generating Root CA private key (RSA 4096)..."
  openssl genpkey \
      -algorithm RSA \
      -pkeyopt rsa_keygen_bits:4096 \
      -out "$ca_key"

  # ============================================================
  # === Create OpenSSL config for CA ===========================
  # ============================================================
  cat > "$ca_dir/ca.openssl.cnf" << EOF
[ req ]
default_md         = sha512
prompt             = no
distinguished_name = dn
x509_extensions    = v3_ca

[ dn ]
C  = ${SUBJ_C}
ST = ${SUBJ_ST}
L  = ${SUBJ_L}
O  = ${SUBJ_O}
OU = ${SUBJ_OU}
CN = ${ca_name}

[ v3_ca ]
basicConstraints = critical, CA:TRUE
keyUsage = critical, keyCertSign, cRLSign
subjectKeyIdentifier = hash
authorityKeyIdentifier = keyid:always,issuer
EOF

  # ============================================================
  # === Generate self-signed Root CA certificate ===============
  # ============================================================
  echo "Generating self-signed Root CA certificate..."
  openssl req -x509 -new \
      -key "$ca_key" \
      -days "$ca_days" \
      -config "$ca_dir/ca.openssl.cnf" \
      -out "$ca_cert"

  chmod 600 "$ca_key"
  chmod 644 "$ca_cert"

  echo ""
  echo "Root CA generated successfully:"
  echo " - CA Key:  $ca_key"
  echo " - CA Cert: $ca_cert"
  echo "====================================================================="
}

# ===== Example usage =====
# export CERT_SECRET="your-pass"
# generate_keystore_and_truststore "/path/to/certs" "example.com" "rest_api.example.com" "admin.example.com"
generate_cert_with_keystore_and_truststore() {
  local certs_dir="$CERTS_DIR/$1"
  local main_domain="$2"
  local local_ip=${LOCAL_IP:? LOCAL_IP is not set}

  shift 2
  local sub_domains=("$@")

  local alias_name="${main_domain//./-}"
  local cert_secret=${CERT_SECRET:? CERT_SECRET is not set}
  local ca_cert="$CERTS_DIR/ca/ca.crt"
  local ca_key="$CERTS_DIR/ca/ca.key"

  # === Prepare cert folder ===
  if [[ -d "$certs_dir" ]]; then
    rm -rf "${certs_dir:? certs_dir is not set}"/*
  else
    mkdir -p "$certs_dir"
  fi

  # ============================================================
  # === Generate private key (RSA 4096) ========================
  # ============================================================
  echo "Generating private key (RSA 4096)..."
  openssl genpkey \
    -algorithm RSA \
    -pkeyopt rsa_keygen_bits:4096 \
    -out "$certs_dir/$alias_name.key.pem"

  # ============================================================
  # === Create OpenSSL config (SAN support) ====================
  # ============================================================
  echo "Creating OpenSSL config with SANs..."

  local san_text=""
  local count=1

  san_text+="DNS.${count} = ${main_domain}"$'\n'; ((count++))
  san_text+="DNS.${count} = localhost"$'\n'; ((count++))
  san_text+="DNS.${count} = host.docker.internal"$'\n'; ((count++))
  san_text+="IP.${count} = ${local_ip}"$'\n'; ((count++))
  san_text+="IP.${count} = 127.0.0.1"$'\n'; ((count++))

  if [[ ${#sub_domains[@]} -gt 0 ]]; then
    for sub in "${sub_domains[@]}"; do
      if [[ -n "$sub" ]]; then
        san_text+="DNS.${count} = ${sub}"$'\n'
        ((count++))
      fi
    done
  fi

  cat > "$certs_dir/$alias_name.openssl.cnf" << EOF
[ req ]
prompt             = no
default_md         = sha512
distinguished_name = dn
req_extensions     = req_ext

[ dn ]
C = ${SUBJ_C}
ST = ${SUBJ_ST}
L = ${SUBJ_L}
O = ${SUBJ_O}
OU = ${SUBJ_OU}
CN = ${main_domain}

[ req_ext ]
subjectAltName = @alt_names

[ alt_names ]
$(printf "%s" "$san_text")
EOF

  # ============================================================
  # === Generate CSR ===========================================
  # ============================================================
  echo "Generating CSR..."
  openssl req -new \
    -key "$certs_dir/$alias_name.key.pem" \
    -out "$certs_dir/$alias_name.csr.pem" \
    -config "$certs_dir/$alias_name.openssl.cnf"

  # ============================================================
  # === Sign with CA ===========================================
  # ============================================================
  echo "Signing certificate with CA..."
  openssl x509 -req \
    -in "$certs_dir/$alias_name.csr.pem" \
    -CA "$ca_cert" -CAkey "$ca_key" -CAcreateserial \
    -out "$certs_dir/$alias_name.cert.pem" \
    -days 365 \
    -sha512 \
    -extfile "$certs_dir/$alias_name.openssl.cnf" \
    -extensions req_ext

  # ============================================================
  # === Convert to PKCS8 DER for PostgreSQL ====================
  # ============================================================
  echo "Converting private key to PKCS8 DER (.pk8)..."
  openssl pkcs8 \
    -topk8 \
    -inform PEM \
    -outform DER \
    -in "$certs_dir/$alias_name.key.pem" \
    -out "$certs_dir/$alias_name.pk8" \
    -nocrypt

  # ============================================================
  # === Create PKCS#12 keystore ================================
  # ============================================================
  echo "Creating PKCS#12 keystore..."
  openssl pkcs12 -export \
    -inkey "$certs_dir/$alias_name.key.pem" \
    -in "$certs_dir/$alias_name.cert.pem" \
    -certfile "$ca_cert" \
    -passout pass:"$cert_secret" \
    -out "$certs_dir/$alias_name.keystore.p12" \
    -name "$alias_name"

  # ============================================================
  # === Create PKCS#12 truststore ==============================
  # ============================================================
  echo "Creating PKCS#12 truststore..."
  keytool -importcert \
    -noprompt \
    -trustcacerts \
    -alias "$CA_NAME" \
    -file "$ca_cert" \
    -keystore "$certs_dir/$alias_name.truststore.p12" \
    -storetype PKCS12 \
    -storepass "$cert_secret"

  # Provide stable Kafka-friendly aliases alongside service-specific filenames.
  ln -sf "$alias_name.keystore.p12" "$certs_dir/keystore.p12"
  ln -sf "$alias_name.truststore.p12" "$certs_dir/truststore.p12"

  # ============================================================
  # === Permissions ============================================
  # ============================================================
  chmod 644 "$certs_dir"/*.pem "$certs_dir"/*.pk8 "$certs_dir"/*.p12

  echo "[$main_domain] RSA cert + keystore + truststore generation complete!"
  echo "====================================================================="
}

generate_tls_certs() {
  generate_cert_with_keystore_and_truststore "gateway" "gateway" "${APP_HOSTNAME}"
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

build_backend_image() {
  local backend_dir=${1:?backend_dir is required}
  local image_name
  image_name="$(backend_image_name)"

  require_command docker

  docker rmi "$image_name" || true
  docker build --no-cache \
    --build-arg BACKEND_TAG="${BACKEND_TAG}" \
    -f "${backend_dir}/API/Docker/Native/Dockerfile" \
    -t "$image_name" \
    "${backend_dir}"
}
