# Learn Dot Net Helm Chart

This chart is generated from the development deployment files under `Deployment/Dev`.
It deploys the native backend image and the Envoy API gateway with the same defaults used by the scripts and compose files.

Run commands from the repository root unless a step says otherwise.

## Defaults

| Setting | Value |
| --- | --- |
| Kubernetes namespace | `learn-dot-net` |
| Backend image | `leduynhan1201/learn-dot-net/backend:1.0.0` |
| Backend service port | `60001` |
| Gateway host | `backend.learn-dot-net.dev` |
| Gateway port for local testing | `60000` |
| Envoy HTTPS port | `443` |
| Envoy admin port | `9901` |
| Envoy image | `envoyproxy/envoy:tools-dev` |
| Envoy backend health check | `/health/info` |
| API gateway certificate secret | `api-gateway-certs` |

## Recommended Flow

The deployment scripts are the preferred local path because they build the backend image, prepare certificates, create the kind cluster, load the image, create the Kubernetes secret, and install this chart:

```bash
Backend/Deployment/Dev/scripts/k8s-deploy.sh
```

Apply chart or certificate changes again without rebuilding:

```bash
Backend/Deployment/Dev/scripts/k8s-apply.sh
```

Remove the release, generated secret, and namespace:

```bash
Backend/Deployment/Dev/scripts/k8s-clean.sh
```

## Manual Install

Build and load the backend image the same way the deployment scripts do:

```bash
Backend/Deployment/Dev/scripts/build-image.sh
kind load docker-image leduynhan1201/learn-dot-net/backend:1.0.0 --name learn-dot-net
```

Create the API gateway certificate secret from generated deployment certificates:

```bash
kubectl create namespace learn-dot-net --dry-run=client -o yaml | kubectl apply -f -
kubectl -n learn-dot-net create secret generic api-gateway-certs \
  --from-file=api-gateway.cert.pem=Backend/Deployment/Dev/certs/api-gateway/api-gateway.cert.pem \
  --from-file=api-gateway.key.pem=Backend/Deployment/Dev/certs/api-gateway/api-gateway.key.pem \
  --dry-run=client -o yaml | kubectl apply -f -
```

Install or upgrade the chart:

```bash
helm upgrade --install learn-dot-net Backend/charts/learn-dot-net \
  --namespace learn-dot-net \
  --create-namespace
```

`namespace.create` defaults to `false` because Helm should create or target the release namespace with `--create-namespace`.

Port-forward the API gateway for local testing:

```bash
kubectl -n learn-dot-net port-forward svc/api-gateway 60000:443
```

Call the API through Envoy:

```bash
curl -k --resolve backend.learn-dot-net.dev:60000:127.0.0.1 \
  https://backend.learn-dot-net.dev:60000/health/info
```

## TLS Options

By default, the deployment references an existing secret named `api-gateway-certs`.
To let Helm create the secret, set `apiGateway.tls.create=true` and provide the PEM files:

```bash
helm upgrade --install learn-dot-net Backend/charts/learn-dot-net \
  --namespace learn-dot-net \
  --create-namespace \
  --set apiGateway.tls.create=true \
  --set apiGateway.tls.existingSecret= \
  --set-file apiGateway.tls.certificate=Backend/Deployment/Dev/certs/api-gateway/api-gateway.cert.pem \
  --set-file apiGateway.tls.privateKey=Backend/Deployment/Dev/certs/api-gateway/api-gateway.key.pem
```
