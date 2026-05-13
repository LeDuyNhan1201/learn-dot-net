# Changelog

## 2026-05-13

- Added Helm chart content under `Backend/Deployment/Dev/k8s-helm/backend` and `Backend/Deployment/Dev/k8s-helm/api-gateway`.
- Updated Kubernetes scripts to install, upgrade, and uninstall the backend and API gateway Helm releases instead of using kustomize manifests.
- Added Helm-related environment defaults, Kubernetes secret creation for API gateway TLS files, and namespace-aware cleanup.
- Updated deployment documentation to use the Helm chart flow consistently.

## 2026-05-09

- Added Kubernetes deploy, apply, clean, delete-cluster, and port-forward scripts plus an all-in-one Docker and Kubernetes runbook.
- Fixed `init.sh` to build the native API image from the Backend context and restored the Dockerfile-specific ignore file for that context.
- Standardized gateway naming on `api-gateway` across compose, Envoy configs, templates, scripts, generated cert paths, and Kubernetes manifests.
- Renamed Kubernetes gateway manifests and generated resources from Envoy-centric names to `api-gateway` for consistent naming.
- Replaced the hand-copied Kubernetes Envoy ConfigMap with a kustomize-generated ConfigMap sourced from `DevopsTools/Dev/envoy/api-gateway.yaml`, and added Kubernetes Envoy mounts for config and TLS certificates.
- Kept `appsettings.Development.json` in the native backend image and set the compose runtime environment to Development for both ASP.NET Core and .NET host configuration.
- Fixed the native backend image build to use the Backend directory as Docker context so API, Application, Domain, and Infrastructure projects are available during restore and publish.
- Added a Native Dockerfile-specific ignore file so `build-image.sh` excludes generated, local, and secret development files from the Backend build context.

## 2026-05-07

- Refactored the native API Dockerfile to build `API/API.csproj`, publish the `API` native AOT executable, and listen on port `60000` in the runtime container.
- Added a Native Docker ignore file to keep generated, local, and editor files out of that Docker target.
- Aligned the native API Dockerfile with `DevopsTools/Dev/scripts/init.sh` so it builds from the API directory context and tags image metadata with `BACKEND_TAG`.
