```shell
# Cleanup minikube and docker containers
docker rm -f minikube
minikube delete
docker ps -a | grep minikube
docker network ls | grep minikube

# Start minikube with docker driver, 6 CPUs and 12GB memory
minikube start --driver=docker --cpus=6 --memory=12g

# Verify minikube status and kubectl context
kubectl config current-context
kubectl get nodes
helm version
kubectl get pods -A

# Check minikube status and kubectl nodes
kubectl get nodes
minikube status

# (Optional) Pull the required images for ingress-nginx controller and webhook certgen
docker pull registry.k8s.io/ingress-nginx/controller:v1.14.3
docker pull registry.k8s.io/ingress-nginx/kube-webhook-certgen:v1.6.7

minikube image load registry.k8s.io/ingress-nginx/controller:v1.14.3
minikube image load registry.k8s.io/ingress-nginx/kube-webhook-certgen:v1.6.7

# (Optional) Create a directory for Docker registry certificates in minikube and copy the CA certificate
minikube ssh
sudo mkdir -p /etc/docker/certs.d/registry.k8s.io
sudo install -m 0644 /usr/local/share/ca-certificates/tma-ADCA-CA.crt /etc/docker/certs.d/registry.k8s.io/ca.crt
sudo ls -l /etc/docker/certs.d/registry.k8s.io/ca.crt
exit

minikube ssh -- sudo crictl pull 'registry.k8s.io/ingress-nginx/kube-webhook-certgen:v1.6.7@sha256:7c74a715af2c94cb734785b4d3ea1357b4f02b88e1e123c622a9cb68b62f669c'

# Enable ingress and ingress-dns addons in minikube
minikube addons enable ingress
minikube addons enable ingress-dns

# Check the status of the ingress controller
kubectl get svc -n ingress-nginx -w
kubectl get pods -n ingress-nginx -w
kubectl get pods -n kube-system | grep ingress-dns

# Enable SSL passthrough in the ingress controller
kubectl patch configmap ingress-nginx-controller -n ingress-nginx --type merge -p '{"data":{"enable-ssl-passthrough":"true"}}'

kubectl rollout restart deployment ingress-nginx-controller -n ingress-nginx
kubectl rollout status deployment ingress-nginx-controller -n ingress-nginx

kubectl get configmap ingress-nginx-controller -n ingress-nginx -o jsonpath='{.data.enable-ssl-passthrough}{"\n"}'
```