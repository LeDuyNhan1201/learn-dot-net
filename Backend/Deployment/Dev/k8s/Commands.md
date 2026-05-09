# Kubernetes dev commands

The scripted flow is preferred:

```bash
Backend/DevopsTools/Dev/scripts/k8s-deploy.sh
Backend/DevopsTools/Dev/scripts/k8s-port-forward.sh
```

Manual equivalent:

```bash
kind create cluster --name learn-dot-net
Backend/DevopsTools/Dev/scripts/init.sh
kind load docker-image leduynhan1201/learn-dot-net/backend:1.0.0 --name learn-dot-net
kubectl apply -k Backend/DevopsTools/Dev
kubectl -n learn-dot-net port-forward svc/api-gateway 60000:60000
```

Test through the gateway:

```bash
curl -k --resolve backend.learn-dot-net.dev:60000:127.0.0.1 \
  https://backend.learn-dot-net.dev:60000/health/info
```

Clean up:

```bash
Backend/DevopsTools/Dev/scripts/k8s-clean.sh
Backend/DevopsTools/Dev/scripts/k8s-delete-cluster.sh
```
