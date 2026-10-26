```shell
# Install cert-manager using Helm
helm install cert-manager oci://quay.io/jetstack/charts/cert-manager --version v1.21.2 --namespace cert-manager --create-namespace --set crds.enabled=true --wait --timeout 5m

# Verify cert-manager installation
kubectl get pods -n cert-manager
kubectl get crd | grep cert-manager.io
kubectl get deployments -n cert-manager
helm status cert-manager -n cert-manager

kubectl wait --for=condition=Ready pod --all -n cert-manager --timeout=180s

# View cert-manager logs and describe pods for debugging
kubectl logs -n cert-manager deploy/cert-manager
kubectl logs -n cert-manager deploy/cert-manager-webhook
kubectl describe pods -n cert-manager

# Apply the cert-manager templates for self-signed issuer, root CA, CA cluster issuer
kubectl apply -f cert-manager/templates/00-selfsigned-issuer.yaml
kubectl apply -f cert-manager/templates/01-root-ca.yaml
kubectl apply -f cert-manager/templates/02-ca-cluster-issuer.yaml

# Verify the resources created by cert-manager
kubectl get clusterissuer selfsigned-bootstrap

kubectl get certificate -n cert-manager
kubectl describe certificate ldnhan-root-ca -n cert-manager
kubectl get secret ldnhan-root-ca-secret -n cert-manager

kubectl get clusterissuer ldnhan-root-ca-issuer
kubectl describe clusterissuer ldnhan-root-ca-issuer
```