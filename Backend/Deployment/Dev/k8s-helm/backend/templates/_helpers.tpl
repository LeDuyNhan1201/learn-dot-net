{{/*
Expand the chart name.
*/}}
{{- define "backend.name" -}}
{{- default .Chart.Name .Values.nameOverride | trunc 63 | trimSuffix "-" -}}
{{- end }}

{{/*
Create a stable resource name. The default is "backend" to match Envoy routing.
*/}}
{{- define "backend.fullname" -}}
{{- if .Values.fullnameOverride -}}
{{- .Values.fullnameOverride | trunc 63 | trimSuffix "-" -}}
{{- else -}}
{{- include "backend.name" . -}}
{{- end -}}
{{- end }}

{{/*
Namespace used by all rendered resources.
*/}}
{{- define "backend.namespace" -}}
{{- default .Release.Namespace .Values.namespace.name -}}
{{- end }}

{{/*
Common labels.
*/}}
{{- define "backend.labels" -}}
helm.sh/chart: {{ printf "%s-%s" .Chart.Name .Chart.Version | replace "+" "_" | trunc 63 | trimSuffix "-" }}
app.kubernetes.io/name: {{ include "backend.name" . }}
app.kubernetes.io/instance: {{ .Release.Name }}
app.kubernetes.io/component: backend
app.kubernetes.io/version: {{ .Chart.AppVersion | quote }}
app.kubernetes.io/managed-by: {{ .Release.Service }}
{{- end }}

{{/*
Selector labels must remain stable across upgrades.
*/}}
{{- define "backend.selectorLabels" -}}
app: {{ include "backend.fullname" . }}
app.kubernetes.io/name: {{ include "backend.name" . }}
app.kubernetes.io/instance: {{ .Release.Name }}
app.kubernetes.io/component: backend
{{- end }}

{{/*
Service account name.
*/}}
{{- define "backend.serviceAccountName" -}}
{{- if .Values.serviceAccount.create -}}
{{- default (include "backend.fullname" .) .Values.serviceAccount.name -}}
{{- else -}}
{{- default "" .Values.serviceAccount.name -}}
{{- end -}}
{{- end }}
