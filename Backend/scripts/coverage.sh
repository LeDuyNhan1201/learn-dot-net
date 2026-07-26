#!/usr/bin/env bash
set -e

dotnet test Backend.slnx --settings cover.runsettings

reportgenerator \
    -reports:"**/TestResults/**/coverage.cobertura.xml" \
    -targetdir:"Coverage" \
    -reporttypes:"Html;HtmlSummary;MarkdownSummary"

echo "Coverage report: Coverage/index.html"