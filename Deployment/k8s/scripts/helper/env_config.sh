export NAMESPACE="leduynhan1201"

LOCAL_IP="${LOCAL_IP:-$(hostname -I 2>/dev/null | awk '{print $1}')}"
LOCAL_IP="${LOCAL_IP:-127.0.0.1}"
export LOCAL_IP
export CA_NAME="LDNhanRootCA"
export SUBJ_C="VN"
export SUBJ_ST="BinhTriDong"
export SUBJ_L="HCM"
export SUBJ_O="SGU"
export SUBJ_OU="Dev"

export K8S_DIR="${K8S_DIR:-$(pwd)}"
export SECRETS_DIR="${SECRETS_DIR:-${K8S_DIR}/secrets}"
export CERTS_DIR="${CERTS_DIR:-${K8S_DIR}/certs}"

export CERT_SECRET='@N120103#'



