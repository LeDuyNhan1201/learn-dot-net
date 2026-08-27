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

# Create namespace for Kafka
kubectl create namespace learn-kafka
kubectl get namespaces

# Install Strimzi Kafka Operator using Helm
helm repo add strimzi https://strimzi.io/charts/
helm repo update
helm search repo strimzi
helm install strimzi-kafka-operator strimzi/strimzi-kafka-operator --namespace learn-kafka

# Check the status of the Strimzi Kafka Operator
kubectl get pods -n learn-kafka
kubectl get deployment -n learn-kafka
kubectl get crd | grep kafka

# Enable ingress addon in minikube
minikube addons enable ingress

# Check the status of the ingress controller
kubectl get pods -n ingress-nginx
kubectl get svc -n ingress-nginx

# Enable SSL passthrough in the ingress controller
kubectl patch configmap ingress-nginx-controller -n ingress-nginx --type merge -p '{"data":{"enable-ssl-passthrough":"true"}}'
kubectl get configmap ingress-nginx-controller -n ingress-nginx -o jsonpath='{.data.enable-ssl-passthrough}{"\n"}'
kubectl rollout restart deployment ingress-nginx-controller -n ingress-nginx
kubectl rollout status deployment ingress-nginx-controller -n ingress-nginx

# Enable ingress-dns addon in minikube
minikube addons enable ingress-dns
kubectl get pods -n kube-system | grep ingress-dns

# Install dnsmasq for DNS resolution
sudo apt update
sudo apt install dnsmasq
sudo nvim /etc/NetworkManager/NetworkManager.conf
[main]
plugins=ifupdown,keyfile
dns=dnsmasq

[ifupdown]
managed=false

[device]
wifi.scan-rand-mac-address=no

# Configure dnsmasq for minikube ingress
sudo mkdir -p /etc/NetworkManager/dnsmasq.d
sudo nvim /etc/NetworkManager/dnsmasq.d/minikube.conf
address=/cluster-0-ingress.com/192.168.49.2
sudo systemctl restart NetworkManager

# Create secrets for Kafka and OAuth
kubectl create secret generic oauth-ca --from-file=ca.crt=certs/oauth-ca/ca.crt -n learn-kafka
kubectl create secret generic kafka-ca --from-file=ca.crt=certs/kafka-ca/ca.crt -n learn-kafka
kubectl get secret oauth-ca -n learn-kafka
kubectl get secret kafka-ca -n learn-kafka

kubectl create secret generic server-certs --from-file=certs/server.crt=server.crt --from-file=certs/server.key=server.key -n learn-kafka
kubectl create secret generic scram-sha512 --from-file=secrets/scram-sha512-password.txt=secrets/scram-sha512-password.txt -n learn-kafka

# Apply Kafka nodes configuration
kubectl apply -f kafka/templates/kafka-nodes.yaml -n learn-kafka
kubectl get kafkanodepool -n learn-kafka

# Apply Kafka metrics configuration
kubectl apply -f kafka/templates/kafka-metrics.yaml -n learn-kafka
kubectl get configmap kafka-metrics -n learn-kafka

# Apply Kafka cluster configuration
helm template cluster-0 ./kafka -n learn-kafka > ./tmp/kafka-rendered.yaml
helm upgrade --install cluster-0 ./kafka -n learn-kafka

# Check the status of the Kafka cluster
kubectl get pods -n learn-kafka -o wide
kubectl get kafkanodepool -n learn-kafka
kubectl get configmap kafka-metrics -n learn-kafka
kubectl get kafka -n learn-kafka
kubectl describe kafka cluster-0 -n learn-kafka

kubectl logs cluster-0-dual-broker-1 -n learn-kafka -c kafka --previous | tail -100

username: my-connect-username
passwordSecret:
    secretName: scram-sha512
    password: scram-sha512-password
```