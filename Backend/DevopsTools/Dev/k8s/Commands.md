# Kind / Kubernetes dev commands

```bash
kind create cluster --name learn-dot-net
```

```bash
Backend/DevopsTools/Dev/scripts/init.sh
Backend/DevopsTools/Dev/scripts/build-image.sh
kind load docker-image leduynhan1201/learn-dot-net/backend:1.0.0 --name learn-dot-net
```

Apply all Kubernetes resources from the kustomize root. This generates `api-gateway-config` from `envoy/api-gateway.yaml` and mounts it into the api-gateway pod.

```bash
kubectl apply -k Backend/DevopsTools/Dev
```

Expose the gateway locally on the same port used by the generated Envoy config.

```bash
kubectl -n learn-dot-net port-forward svc/api-gateway 60000:60000
```
