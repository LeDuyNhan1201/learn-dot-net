```shell
# Cleanup minikube and docker containers
docker rm -f minikube
minikube delete
docker ps -a | grep minikube
docker network ls | grep minikube

# Start minikube with docker driver, 6 CPUs and 12GB memory
minikube start --driver=docker --cpus=6 --memory=12g

# Check minikube status and kubectl nodes
kubectl get nodes
minikube status

# Enable ingress addon in minikube
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