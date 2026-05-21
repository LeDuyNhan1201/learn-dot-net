#!/usr/bin/env bash

set -e

ENV_NAME="$1"

if [ -z "$ENV_NAME" ]; then
  echo "Usage: source ./prepare.sh <env>"
  exit 1
fi

ROOT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
TARGET_DIR="$ROOT_DIR/$ENV_NAME/scripts"

if [ ! -d "$TARGET_DIR" ]; then
  echo "Directory not found: $TARGET_DIR"
  exit 1
fi

chmod +x "$TARGET_DIR"/*.sh

for filepath in "$TARGET_DIR"/*.sh; do
  [ -f "$filepath" ] || continue

  file="$(basename "$filepath")"
  cmd="${file%.sh}"

  eval "
  $cmd() {
    \"$filepath\" \"\$@\"
  }
  "

  echo "Registered command: $cmd"
done

echo ""
echo "Done."
echo "Use commands directly now."