# Learn Dot Net Dev Runbook

This guide covers both local Docker Compose and Kubernetes development flows for the backend API and API gateway.

Run commands from the repository root unless a step says otherwise.

## Prerequisites

Install these tools before running the flows:

- Docker and Docker Compose
- .NET SDK 10
- `openssl`
- `keytool`
- `envsubst`
- `kind` for the Kubernetes flow
- `kubectl` for the Kubernetes flow

The shared dev settings live in:

```bash
Backend/Deployment/Dev/scripts/helper/env_config.sh
```

Important defaults:

```text
Gateway host: backend.learn-dot-net.dev
Gateway port: 60000
Backend container port: 60001
Backend image: leduynhan1201/learn-dot-net/backend:1.0.0
Kind cluster: learn-dot-net
Kubernetes namespace: learn-dot-net
```

## Docker Native Flow

This flow runs the native AOT backend image and the API gateway with Docker Compose.

### 1. Initialize dev files, certificates, and image

```bash
Backend/Deployment/Dev/scripts/init.sh
```

This creates:

- `Backend/Deployment/Dev/.env`
- `Backend/Deployment/Dev/envoy/api-gateway.yaml`
- `Backend/Deployment/Dev/envoy/api-gateway.local.yaml`
- `Backend/Deployment/Dev/certs/ca/*`
- `Backend/Deployment/Dev/certs/api-gateway/*`
- Docker image `leduynhan1201/learn-dot-net/backend:1.0.0`

### 2. Start Docker Compose

```bash
cd Backend/Deployment/Dev
docker compose -f native-compose.yaml up -d
```

### 3. Call the API through the gateway

```bash
curl -k --resolve backend.learn-dot-net.dev:60000:127.0.0.1 \
  https://backend.learn-dot-net.dev:60000/health/info
```

### 4. Stop Docker Compose

```bash
cd Backend/Deployment/Dev
docker compose -f native-compose.yaml down
```

### 5. Rebuild only the backend image

```bash
Backend/Deployment/Dev/scripts/build-image.sh
```

Then recreate the backend container:

```bash
cd Backend/Deployment/Dev
docker compose -f native-compose.yaml up -d --force-recreate backend
```

## Docker Local Gateway Flow

Use this when you want to run the backend directly on the host and only run the API gateway in Docker.

### 1. Recreate gateway files and certificates

```bash
Backend/Deployment/Dev/scripts/re-create.sh
```

### 2. Run the backend on the host

```bash
dotnet run --project Backend/API/API.csproj --urls=http://0.0.0.0:60001
```

### 3. Start the local gateway compose file

In another terminal:

```bash
cd Backend/Deployment/Dev
docker compose -f local-compose.yaml up -d
```

### 4. Call the API through the gateway

```bash
curl -k --resolve backend.learn-dot-net.dev:60000:127.0.0.1 \
  https://backend.learn-dot-net.dev:60000/health/info
```

### 5. Stop the local gateway

```bash
cd Backend/Deployment/Dev
docker compose -f local-compose.yaml down
```

## Kubernetes Flow

This flow deploys the native backend image and API gateway to a local kind cluster.

### 1. Deploy everything

```bash
Backend/Deployment/Dev/scripts/k8s-deploy.sh
```

This script:

- Creates `.env`
- Renders Envoy config from templates
- Ensures the root CA exists
- Generates API gateway certificates
- Builds the backend native AOT Docker image
- Creates the kind cluster if it does not exist
- Loads the backend image into kind
- Applies `kubectl apply -k Backend/Deployment/Dev`
- Waits for backend and API gateway rollouts

### 2. Port-forward the API gateway

```bash
Backend/Deployment/Dev/scripts/k8s-port-forward.sh
```

Keep this command running while testing.

### 3. Call the API through the gateway

In another terminal:

```bash
curl -k --resolve backend.learn-dot-net.dev:60000:127.0.0.1 \
  https://backend.learn-dot-net.dev:60000/health/info
```

### 4. Apply k8s resources again without rebuilding

Use this after editing k8s YAML, Envoy templates, or certificates when the backend image already exists locally:

```bash
Backend/Deployment/Dev/scripts/k8s-apply.sh
```

### 5. Delete k8s resources

```bash
Backend/Deployment/Dev/scripts/k8s-clean.sh
```

### 6. Delete the kind cluster

```bash
Backend/Deployment/Dev/scripts/k8s-delete-cluster.sh
```

## Rendered Kubernetes Resources

The k8s flow uses kustomize from this root:

```bash
Backend/Deployment/Dev/kustomization.yaml
```

Preview rendered manifests:

```bash
kubectl kustomize Backend/Deployment/Dev
```

Client-side dry run:

```bash
kubectl apply --dry-run=client --validate=false -k Backend/Deployment/Dev
```

The API gateway ConfigMap is generated from:

```bash
Backend/Deployment/Dev/envoy/api-gateway.yaml
```

The API gateway certificate Secret is generated from:

```bash
Backend/Deployment/Dev/certs/api-gateway/api-gateway.cert.pem
Backend/Deployment/Dev/certs/api-gateway/api-gateway.key.pem
```

## Cleanup

Remove generated environment files only:

```bash
Backend/Deployment/Dev/scripts/clean.sh
```

Remove generated certs, data, `.env`, and backend image:

```bash
Backend/Deployment/Dev/scripts/clean-all.sh
```

For Kubernetes, run `k8s-clean.sh` before `clean-all.sh` if the resources are still deployed.
