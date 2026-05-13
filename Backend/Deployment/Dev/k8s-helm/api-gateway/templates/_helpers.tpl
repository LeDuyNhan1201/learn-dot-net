{{/*
Expand the chart name.
*/}}
{{- define "api-gateway.name" -}}
{{- default .Chart.Name .Values.nameOverride | trunc 63 | trimSuffix "-" -}}
{{- end }}

{{/*
Create a stable resource name. The default is "api-gateway".
*/}}
{{- define "api-gateway.fullname" -}}
{{- if .Values.fullnameOverride -}}
{{- .Values.fullnameOverride | trunc 63 | trimSuffix "-" -}}
{{- else -}}
{{- include "api-gateway.name" . -}}
{{- end -}}
{{- end }}

{{/*
Namespace used by all rendered resources.
*/}}
{{- define "api-gateway.namespace" -}}
{{- default .Release.Namespace .Values.namespace.name -}}
{{- end }}

{{/*
Common labels.
*/}}
{{- define "api-gateway.labels" -}}
helm.sh/chart: {{ printf "%s-%s" .Chart.Name .Chart.Version | replace "+" "_" | trunc 63 | trimSuffix "-" }}
app.kubernetes.io/name: {{ include "api-gateway.name" . }}
app.kubernetes.io/instance: {{ .Release.Name }}
app.kubernetes.io/component: api-gateway
app.kubernetes.io/version: {{ .Chart.AppVersion | quote }}
app.kubernetes.io/managed-by: {{ .Release.Service }}
{{- end }}

{{/*
Selector labels must remain stable across upgrades.
*/}}
{{- define "api-gateway.selectorLabels" -}}
app: {{ include "api-gateway.fullname" . }}
app.kubernetes.io/name: {{ include "api-gateway.name" . }}
app.kubernetes.io/instance: {{ .Release.Name }}
app.kubernetes.io/component: api-gateway
{{- end }}

{{/*
Service account name.
*/}}
{{- define "api-gateway.serviceAccountName" -}}
{{- if .Values.serviceAccount.create -}}
{{- default (include "api-gateway.fullname" .) .Values.serviceAccount.name -}}
{{- else -}}
{{- default "" .Values.serviceAccount.name -}}
{{- end -}}
{{- end }}

{{/*
ConfigMap and Secret names.
*/}}
{{- define "api-gateway.configMapName" -}}
{{- default (printf "%s-config" (include "api-gateway.fullname" .)) .Values.envoy.configMapName -}}
{{- end }}

{{- define "api-gateway.tlsSecretName" -}}
{{- if .Values.tls.existingSecret -}}
{{- .Values.tls.existingSecret -}}
{{- else -}}
{{- printf "%s-certs" (include "api-gateway.fullname" .) -}}
{{- end -}}
{{- end }}
