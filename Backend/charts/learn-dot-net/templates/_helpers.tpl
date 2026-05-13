{{/*
Common chart labels.
*/}}
{{- define "learn-dot-net.labels" -}}
helm.sh/chart: {{ printf "%s-%s" .Chart.Name .Chart.Version | replace "+" "_" | trunc 63 | trimSuffix "-" }}
app.kubernetes.io/instance: {{ .Release.Name }}
app.kubernetes.io/managed-by: {{ .Release.Service }}
{{- end }}

{{/*
Namespace used by all rendered resources.
*/}}
{{- define "learn-dot-net.namespace" -}}
{{- default .Release.Namespace .Values.namespace.name -}}
{{- end }}

{{/*
Stable resource names mirror the existing Deployment/Dev manifests.
*/}}
{{- define "learn-dot-net.backendName" -}}
{{- default "backend" .Values.backend.name -}}
{{- end }}

{{- define "learn-dot-net.apiGatewayName" -}}
{{- default "api-gateway" .Values.apiGateway.name -}}
{{- end }}

{{- define "learn-dot-net.apiGatewayConfigName" -}}
{{- default "api-gateway-config" .Values.apiGateway.configMapName -}}
{{- end }}

{{- define "learn-dot-net.apiGatewaySecretName" -}}
{{- if .Values.apiGateway.tls.existingSecret -}}
{{- .Values.apiGateway.tls.existingSecret -}}
{{- else -}}
{{- printf "%s-certs" (include "learn-dot-net.apiGatewayName" .) -}}
{{- end -}}
{{- end }}

{{- define "learn-dot-net.backendSelectorLabels" -}}
app: {{ include "learn-dot-net.backendName" . }}
app.kubernetes.io/name: {{ include "learn-dot-net.backendName" . }}
app.kubernetes.io/component: backend
app.kubernetes.io/instance: {{ .Release.Name }}
{{- end }}

{{- define "learn-dot-net.apiGatewaySelectorLabels" -}}
app: {{ include "learn-dot-net.apiGatewayName" . }}
app.kubernetes.io/name: {{ include "learn-dot-net.apiGatewayName" . }}
app.kubernetes.io/component: api-gateway
app.kubernetes.io/instance: {{ .Release.Name }}
{{- end }}
