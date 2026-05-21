#!/usr/bin/env bash
set -euo pipefail

ENV_NAME="${1:?Missing environment}"

ROOT_DIR="$(cd -- "$(dirname -- "${BASH_SOURCE[0]}")" && pwd)"
SCRIPT_DIR="$ROOT_DIR/$ENV_NAME/scripts"

chmod +x "$SCRIPT_DIR"/*.sh

export PATH="$SCRIPT_DIR:$PATH"

echo "PATH updated:"
echo "$SCRIPT_DIR"