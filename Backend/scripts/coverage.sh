#!/usr/bin/env bash
set -euo pipefail

rm -rf Coverage
find . -name "*.cobertura.xml" -delete

dotnet test \
  --solution Backend.slnx \
  --coverage \
  --coverage-output-format cobertura \
  --coverage-settings cover.settings.xml

reportgenerator \
    -reports:"**/TestResults/*.cobertura.xml" \
    -targetdir:"Coverage" \
    -reporttypes:"Html;HtmlSummary;MarkdownSummary"

echo "Coverage report: Coverage/index.html"