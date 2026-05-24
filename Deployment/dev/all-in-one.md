# Learn Dot Net Dev Deployment

This runbook covers the Docker native, Docker local, and local Kubernetes development flows for the backend API and API gateway.
The Docker Compose flows also start the observability stack: OpenTelemetry Collector, Grafana, Loki, Tempo, and Prometheus.

Run commands from the repository root unless a step says otherwise.

## Architecture

Docker Compose uses two networks:

| Network | Members | Purpose |
| --- | --- | --- |
| `dev-network` / `local-network` | API gateway, backend when containerized, OTel Collector | Request path and telemetry ingress |
| `observability-network` | OTel Collector, Prometheus, Loki, Tempo, Grafana | Internal observability traffic |

Telemetry flow:

```text
Backend OTLP traces/logs/metrics -> OTel Collector
OTel Collector traces -> Tempo
OTel Collector logs -> Loki native OTLP endpoint
OTel Collector metrics -> Prometheus scrape endpoint
Tempo metrics-generator -> Prometheus remote write
Grafana -> Prometheus, Loki, Tempo
```

## Services And Ports

| Service | Container | Host port | Internal port | Notes |
| --- | --- | ---: | ---: | --- |
| API gateway | `api-gateway` | `60000` | `443` | HTTPS entrypoint |
| Envoy admin | `api-gateway` | `9901` | `9901` | Local admin endpoint |
| Backend | `backend` | `60001` | `60001` | Native Docker flow only |
| Grafana | `grafana` | `3000` | `3000` | Provisioned datasources |
| Prometheus | `prometheus` | `9090` | `9090` | Scrapes collector and stack metrics |
| Loki | `loki` | `3100` | `3100` | Logs and LogQL |
| Tempo | `tempo` | `3200` | `3200` | Traces and TraceQL |
| OTel Collector OTLP gRPC | `otel-collector` | `4317` | `4317` | Host apps send OTLP here in local flow |
| OTel Collector OTLP HTTP | `otel-collector` | `4318` | `4318` | Browser/HTTP OTLP ingress |
| OTel Collector health | `otel-collector` | `13133` | `13133` | Collector health extension |

Collector metrics ports `8888` and `8889` are internal-only and scraped by Prometheus.

## Environment

Central defaults live in:

```bash
Backend/Deployment/Dev/scripts/helper/env_config.sh
```

The scripts render `Backend/Deployment/Dev/.env`; Docker Compose reads that file automatically.
Regenerate `.env` after changing environment defaults:

```bash
Backend/Deployment/Dev/scripts/re-create.sh
```

Important variables:

| Variable | Default | Purpose |
| --- | --- | --- |
| `GATEWAY_PORT` | `60000` | Host HTTPS port for Envoy |
| `BACKEND_CONTAINER_PORT` | `60001` | Backend container and native host port |
| `GRAFANA_TAG` | `13.0.1` | Grafana image tag |
| `LOKI_TAG` | `3.7.1` | Loki image tag |
| `TEMPO_TAG` | `2.10.5` | Tempo image tag |
| `PROMETHEUS_TAG` | `3.11.3` | Prometheus image tag |
| `OTEL_TAG` | `0.152.0` | OpenTelemetry Collector contrib image tag |
| `GRAFANA_PORT` | `3000` | Grafana host port |
| `PROMETHEUS_PORT` | `9090` | Prometheus host port |
| `LOKI_PORT` | `3100` | Loki host port |
| `TEMPO_PORT` | `3200` | Tempo host port |
| `OTEL_COLLECTOR_OTLP_GRPC_PORT` | `4317` | OTLP gRPC host/container port |
| `OTEL_COLLECTOR_OTLP_HTTP_PORT` | `4318` | OTLP HTTP host/container port |
| `PROMETHEUS_RETENTION_TIME` | `15d` | Prometheus TSDB retention time |
| `PROMETHEUS_RETENTION_SIZE` | `2GB` | Prometheus TSDB retention size |
| `OBSERVABILITY_UID` / `OBSERVABILITY_GID` | current user | UID/GID used by observability containers for bind-mounted data |
| `GRAFANA_ADMIN_USER` / `GRAFANA_ADMIN_PASSWORD` | `admin` / `admin` | Development login; override for shared environments |

Persistent data is stored under `Backend/Deployment/Dev/data/{grafana,loki,tempo,prometheus}` and is removed by `clean-all.sh`.

## Docker Native Flow

This flow runs the native AOT backend image, API gateway, and observability stack in Docker.

1. Initialize environment, certificates, data folders, and backend image:

```bash
Backend/Deployment/Dev/scripts/init.sh
```

2. Start Compose:

```bash
cd Backend/Deployment/Dev
docker compose -f native-compose.yaml up -d
```

3. Check service health:

```bash
docker compose -f native-compose.yaml ps
```

4. Call the API through the gateway:

```bash
curl -k --resolve backend.learn-dot-net.dev:60000:127.0.0.1 \
  https://backend.learn-dot-net.dev:60000/health/info
```

5. Open observability UIs:

```text
Grafana:    http://localhost:3000
Prometheus: http://localhost:9090
Loki:       http://localhost:3100/ready
Tempo:      http://localhost:3200/ready
```

6. Stop Compose:

```bash
cd Backend/Deployment/Dev
docker compose -f native-compose.yaml down
```

## Docker Local Flow

Use this when the backend runs directly on the host and Docker runs the API gateway plus observability stack.

1. Recreate generated files and certificates:

```bash
Backend/Deployment/Dev/scripts/re-create.sh
```

2. Start Docker services:

```bash
cd Backend/Deployment/Dev
docker compose -f local-compose.yaml up -d
```

3. Run the backend on the host with OTLP pointed at the collector:

```bash
export OTEL_SERVICE_NAME=backend
export OTEL_EXPORTER_OTLP_ENDPOINT=http://localhost:4317
export OTEL_EXPORTER_OTLP_PROTOCOL=grpc
export OTEL_RESOURCE_ATTRIBUTES=service.namespace=learn-dot-net,deployment.environment.name=dev,service.version=1.0.0
dotnet run --project Backend/API/API.csproj --urls=http://0.0.0.0:60001
```

4. Call the API through the gateway:

```bash
curl -k --resolve backend.learn-dot-net.dev:60000:127.0.0.1 \
  https://backend.learn-dot-net.dev:60000/health/info
```

5. Stop Docker services:

```bash
cd Backend/Deployment/Dev
docker compose -f local-compose.yaml down
```

## Kubernetes Flow

The Kubernetes scripts and Helm charts currently deploy the backend and API gateway only.
Use the Docker Compose flows above for the observability stack in this development setup.

Deploy everything for Kubernetes:

```bash
Backend/Deployment/Dev/scripts/k8s-deploy.sh
```

Apply Helm resources again without rebuilding the backend image:

```bash
Backend/Deployment/Dev/scripts/k8s-apply.sh
```

Port-forward the API gateway:

```bash
Backend/Deployment/Dev/scripts/k8s-port-forward.sh
```

Clean Kubernetes resources:

```bash
Backend/Deployment/Dev/scripts/k8s-clean.sh
```

Delete the kind cluster:

```bash
Backend/Deployment/Dev/scripts/k8s-delete-cluster.sh
```

## Maintenance

Rebuild only the backend image:

```bash
Backend/Deployment/Dev/scripts/build-images.sh
cd Backend/Deployment/Dev
docker compose -f native-compose.yaml up -d --force-recreate backend
```

Remove generated `.env` only:

```bash
Backend/Deployment/Dev/scripts/clean.sh
```

Remove generated certificates, data, `.env`, and backend image:

```bash
Backend/Deployment/Dev/scripts/clean-all.sh
```

## Troubleshooting

Regenerate `.env` after pulling config changes:

```bash
Backend/Deployment/Dev/scripts/re-create.sh
```

Inspect collector logs when telemetry is missing:

```bash
cd Backend/Deployment/Dev
docker compose -f native-compose.yaml logs -f otel-collector
```

Verify Prometheus targets:

```text
http://localhost:9090/targets
```

Verify Grafana datasources:

```text
http://localhost:3000/connections/datasources
```

If Loki or Tempo fail after upgrading from older local data, remove only their persisted data folders and restart:

```bash
cd Backend/Deployment/Dev
sudo rm -rf data/loki data/tempo
docker compose -f native-compose.yaml up -d loki tempo otel-collector
```

## Breaking Changes

Loki now writes new data with TSDB schema `v13` starting `2026-05-14`.
The config keeps the previous BoltDB shipper schema readable, but older Loki images cannot write the new schema.
Use `LOKI_TAG=3.7.1` or newer unless you also roll back `loki/loki.yaml`.
