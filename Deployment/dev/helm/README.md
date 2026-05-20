# Learn Dot Net Helm Charts

This folder contains the development Helm charts used by the Kubernetes scripts:

- `backend`: deploys the native AOT backend API image.
- `api-gateway`: deploys Envoy, renders `envoy.yaml`, and mounts API gateway TLS certificates.

The Docker Compose observability stack is documented in `Backend/Deployment/Dev/all-in-one.md`.
These Helm charts do not currently deploy Grafana, Loki, Tempo, Prometheus, or the OpenTelemetry Collector.

Run commands from the repository root unless a step says otherwise.

## Defaults

| Setting | Value |
| --- | --- |
| Kubernetes namespace | `learn-dot-net` |
| Backend release | `backend` |
| Backend chart | `Backend/Deployment/Dev/k8s-helm/backend` |
| API gateway release | `api-gateway` |
| API gateway chart | `Backend/Deployment/Dev/k8s-helm/api-gateway` |
| Backend image | `leduynhan1201/learn-dot-net/backend:1.0.0` |
| Backend service port | `60001` |
| Gateway host | `backend.learn-dot-net.dev` |
| Gateway local port | `60000` |
| Envoy HTTPS port | `443` |
| Envoy admin port | `9901` |
| API gateway certificate secret | `api-gateway-certs` |

## Scripted Flow

Deploy everything:

```bash
Backend/Deployment/Dev/scripts/k8s-deploy.sh
```

Apply chart or certificate changes again without rebuilding the backend image:

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

## Manual Render

Preview the backend chart:

```bash
helm template backend Backend/Deployment/Dev/k8s-helm/backend --namespace learn-dot-net
```

Preview the API gateway chart:

```bash
helm template api-gateway Backend/Deployment/Dev/k8s-helm/api-gateway --namespace learn-dot-net
```

## Legacy Templates

The files in `Backend/Deployment/Dev/k8s-helm/templates` are plain `envsubst` reference templates for the same backend and API gateway resources.
The Kubernetes scripts deploy from the Helm charts, not from these legacy templates.

## TLS Secret

The API gateway chart expects this secret by default:

```bash
kubectl -n learn-dot-net create secret generic api-gateway-certs \
  --from-file=api-gateway.cert.pem=Backend/Deployment/Dev/certs/api-gateway/api-gateway.cert.pem \
  --from-file=api-gateway.key.pem=Backend/Deployment/Dev/certs/api-gateway/api-gateway.key.pem \
  --dry-run=client -o yaml | kubectl apply -f -
```

The deployment scripts create this secret automatically after generating certificates.
