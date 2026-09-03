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

# Create namespace for Kafka & Keycloak
kubectl create namespace learn-kafka
kubectl create namespace keycloak-cluster
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

# Create secrets for Kafka and OAuth
kubectl create secret generic oauth-ca --from-file=ca.crt=certs/oauth-ca/ca.crt -n learn-kafka
kubectl get secret oauth-ca -n learn-kafka
kubectl create secret generic kafka-ca --from-file=ca.crt=certs/kafka-ca/ca.crt -n learn-kafka
kubectl get secret kafka-ca -n learn-kafka

# Create TLS certs for Kafka brokers
kubectl create secret generic broker-0-tls --from-file=server.crt=certs/broker-0/server.crt --from-file=server.key=certs/broker-0/server.key -n learn-kafka
kubectl get secret broker-0-tls -n learn-kafka
kubectl create secret generic broker-1-tls --from-file=server.crt=certs/broker-1/server.crt --from-file=server.key=certs/broker-1/server.key -n learn-kafka
kubectl get secret broker-1-tls -n learn-kafka
kubectl create secret generic broker-2-tls --from-file=server.crt=certs/broker-2/server.crt --from-file=server.key=certs/broker-2/server.key -n learn-kafka
kubectl get secret broker-2-tls -n learn-kafka

# Create TLS certs for Keycloak instances
kubectl create secret generic keycloak-0-tls --from-file=server.crt=certs/keycloak-0/server.crt --from-file=server.key=certs/keycloak-0/server.key -n learn-kafka
kubectl get secret keycloak-0-tls -n learn-kafka
kubectl create secret generic keycloak-1-tls --from-file=server.crt=certs/keycloak-1/server.crt --from-file=server.key=certs/keycloak-1/server.key -n learn-kafka
kubectl get secret keycloak-1-tls -n learn-kafka
kubectl create secret generic keycloak-2-tls --from-file=server.crt=certs/keycloak-2/server.crt --from-file=server.key=certs/keycloak-2/server.key -n learn-kafka
kubectl get secret keycloak-2-tls -n learn-kafka

# Create secrets for Kafka SCRAM-SHA-512 authentication
kubectl create secret generic scram-sha512 --from-file=scram-sha512-password.txt=secrets/scram-sha512-password.txt -n learn-kafka
kubectl get secret scram-sha512 -n learn-kafka

# Create secrets for Keycloak database credentials
kubectl create secret generic keycloak-db-secret -n keycloak-cluster --from-literal=username=testuser --from-literal=password=testpassword
kubectl get secret keycloak-db-secret -n keycloak-cluster

# Apply Kafka cluster configuration
helm template cluster-0 ./kafka -n learn-kafka > ./tmp/kafka-rendered.yaml
helm upgrade --install cluster-0 ./kafka -n learn-kafka

helm template cluster-0 ./keycloak -n keycloak-cluster > ./tmp/keycloak-rendered.yaml
helm upgrade --install cluster-0 ./keycloak -n keycloak-cluster

# Check the status of the Kafka cluster
kubectl get pods -n learn-kafka -o wide
kubectl get kafkanodepool -n learn-kafka

kubectl get configmap kafka-metrics -n learn-kafka -o yaml
kubectl get configmap cluster-0-dual-broker-0 -n learn-kafka -o yaml

kubectl get kafka -n learn-kafka
kubectl describe kafka cluster-0 -n learn-kafka

kubectl get secret -n learn-kafka
kubectl get configmap -n learn-kafka

kubectl get all -n learn-kafka
kubectl get strimzipodset -n learn-kafka
kubectl get pods,svc,sts,deploy,job,cm,secret,pvc -n learn-kafka
kubectl get kafka,kafkanodepool,strimzipodset,pvc,secret,configmap -n learn-kafka

kubectl get keycloak -n keycloak-cluster -o go-template='{{range .status.conditions}}CONDITION: {{.type}}{{"\n"}}  STATUS: {{.status}}{{"\n"}}  MESSAGE: {{.message}}{{"\n"}}{{end}}'

# Check the logs of the Kafka brokers
kubectl logs cluster-0-dual-broker-2 -n learn-kafka --tail=200
kubectl logs cluster-0-dual-broker-0 -n learn-kafka --tail=300 | grep -i -E 'oauth|token|login|callback|exception|error|failed|warn'

# Clean up
helm uninstall cluster-0 -n learn-kafka
kubectl delete kafka cluster-0 -n learn-kafka --ignore-not-found
kubectl delete kafkanodepool dual-broker -n learn-kafka --ignore-not-found
kubectl delete strimzipodset -n learn-kafka --all --ignore-not-found     
kubectl delete secret kafka-ca oauth-ca -n learn-kafka
kubectl delete pvc data-0-cluster-0-dual-broker-0 data-0-cluster-0-dual-broker-1 data-0-cluster-0-dual-broker-2 -n learn-kafka
kubectl delete pvc -n learn-kafka --all


username: my-connect-username
passwordSecret:
    secretName: scram-sha512
    password: scram-sha512-password
```