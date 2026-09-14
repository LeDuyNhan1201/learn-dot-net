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
kubectl create namespace monitoring
kubectl create namespace keycloak-cluster
kubectl create namespace kafka-cluster
kubectl get namespaces

# Install Prometheus using Helm
helm repo add prometheus-community https://prometheus-community.github.io/helm-charts
helm repo update
helm search repo prometheus-community/kube-prometheus-stack

# Install Prometheus using Helm with custom values
helm upgrade --install monitoring prometheus-community/kube-prometheus-stack --namespace monitoring --values monitoring/values.yaml

# Check the status of Prometheus pods, services and deployments
kubectl get pods -n monitoring -w -o wide
kubectl get svc -n monitoring -w
kubectl get deployment -n monitoring -w

kubectl get prometheus -n monitoring -w
kubectl get alertmanager -n monitoring -w

# Port-forward Prometheus service to localhost:9090 and log output to grafana.log
nohup kubectl port-forward svc/monitoring-kube-prometheus-prometheus 9090:9090 -n monitoring > tmp/logs/monitoring/prometheus.log 2>&1 &
pkill -f 'kubectl.*port-forward.*9090:9090'

nohup kubectl port-forward svc/monitoring-grafana 3456:80 -n monitoring > tmp/logs/monitoring/grafana.log 2>&1 &
pkill -f 'kubectl.*port-forward.*3456:80'

nohup kubectl port-forward svc/monitoring-kube-prometheus-alertmanager 9093:9093 -n monitoring > tmp/logs/monitoring/alertmanager.log 2>&1 &
pkill -f 'kubectl.*port-forward.*9093:9093'

# Test Prometheus alerting rules by applying test-alert.yaml
kubectl apply -f monitoring/templates/test-alert.yaml

# Clean up monitoring resources
helm uninstall monitoring -n monitoring && kubectl delete namespace monitoring

# Install Strimzi Kafka Operator using Helm
helm repo add strimzi https://strimzi.io/charts/
helm repo update
helm search repo strimzi
helm install strimzi-kafka-operator strimzi/strimzi-kafka-operator --namespace kafka

# Check the status of the Strimzi Kafka Operator
kubectl get pods -n kafka-cluster -w
kubectl get svc -n kafka-cluster -w
kubectl get deployment -n kafka-cluster
kubectl get crd | grep kafka

# Enable ingress addon in minikube
minikube addons enable ingress

# Check the status of the ingress controller
kubectl get pods -n ingress-nginx -w
kubectl get svc -n ingress-nginx -w

# Enable SSL passthrough in the ingress controller
kubectl patch configmap ingress-nginx-controller -n ingress-nginx --type merge -p '{"data":{"enable-ssl-passthrough":"true"}}'
kubectl get configmap ingress-nginx-controller -n ingress-nginx -o jsonpath='{.data.enable-ssl-passthrough}{"\n"}'
kubectl rollout restart deployment ingress-nginx-controller -n ingress-nginx
kubectl rollout status deployment ingress-nginx-controller -n ingress-nginx

# Enable ingress-dns addon in minikube
minikube addons enable ingress-dns
kubectl get pods -n kube-system | grep ingress-dns

# Create secrets for Kafka and OAuth
kubectl create secret generic oauth-ca --from-file=ca.crt=certs/oauth-ca/ca.crt -n kafka-cluster
kubectl get secret oauth-ca -n kafka-cluster
kubectl create secret generic kafka-ca --from-file=ca.crt=certs/kafka-ca/ca.crt -n kafka-cluster
kubectl get secret kafka-ca -n kafka-cluster

# Create TLS certs for Kafka brokers
kubectl create secret generic broker-0-tls --from-file=server.crt=certs/broker-0/server.crt --from-file=server.key=certs/broker-0/server.key -n kafka-cluster
kubectl get secret broker-0-tls -n kafka-cluster
kubectl create secret generic broker-1-tls --from-file=server.crt=certs/broker-1/server.crt --from-file=server.key=certs/broker-1/server.key -n kafka-cluster
kubectl get secret broker-1-tls -n kafka-cluster
kubectl create secret generic broker-2-tls --from-file=server.crt=certs/broker-2/server.crt --from-file=server.key=certs/broker-2/server.key -n kafka-cluster
kubectl get secret broker-2-tls -n kafka-cluster

# Create TLS certs for Keycloak instances
kubectl create secret generic keycloak-0-tls --from-file=server.crt=certs/keycloak-0/server.crt --from-file=server.key=certs/keycloak-0/server.key -n kafka-cluster
kubectl get secret keycloak-0-tls -n kafka-cluster
kubectl create secret generic keycloak-1-tls --from-file=server.crt=certs/keycloak-1/server.crt --from-file=server.key=certs/keycloak-1/server.key -n kafka-cluster
kubectl get secret keycloak-1-tls -n kafka-cluster
kubectl create secret generic keycloak-2-tls --from-file=server.crt=certs/keycloak-2/server.crt --from-file=server.key=certs/keycloak-2/server.key -n kafka-cluster
kubectl get secret keycloak-2-tls -n kafka-cluster

# Create secrets for Keycloak database credentials
kubectl create secret generic keycloak-db-secret -n keycloak-cluster --from-literal=username=testuser --from-literal=password=testpassword
kubectl get secret keycloak-db-secret -n keycloak-cluster

# Create secrets for Kafka SCRAM-SHA-512 authentication
kubectl create secret generic scram-sha512 --from-file=scram-sha512-password.txt=secrets/scram-sha512-password.txt -n kafka-cluster
kubectl get secret scram-sha512 -n kafka-cluster

# Apply Kafka cluster configuration
helm template cluster-0 ./kafka -n kafka-cluster > ./tmp/kafka-rendered.yaml
helm upgrade --install cluster-0 ./kafka -n kafka-cluster

helm template cluster-0 ./keycloak -n keycloak-cluster > ./tmp/keycloak-rendered.yaml
helm upgrade --install cluster-0 ./keycloak -n keycloak-cluster

# Check the status of the Kafka cluster
kubectl get pods -n kafka-cluster -o wide
kubectl get kafkanodepool -n kafka-cluster

kubectl get configmap kafka-metrics -n kafka-cluster -o yaml
kubectl get configmap cluster-0-dual-broker-0 -n kafka-cluster -o yaml

kubectl get kafka -n kafka-cluster
kubectl describe kafka cluster-0 -n kafka-cluster

kubectl get secret -n kafka-cluster
kubectl get configmap -n kafka-cluster

kubectl get all -n kafka-cluster
kubectl get strimzipodset -n kafka-cluster
kubectl get pods,svc,sts,deploy,job,cm,secret,pvc -n kafka-cluster
kubectl get kafka,kafkanodepool,strimzipodset,pvc,secret,configmap -n kafka-cluster

kubectl get keycloak -n keycloak-cluster -o go-template='{{range .status.conditions}}CONDITION: {{.type}}{{"\n"}}  STATUS: {{.status}}{{"\n"}}  MESSAGE: {{.message}}{{"\n"}}{{end}}'

# Check the logs of the Kafka brokers
kubectl logs cluster-0-dual-broker-2 -n kafka-cluster --tail=200
kubectl logs cluster-0-dual-broker-0 -n kafka-cluster --tail=300 | grep -i -E 'oauth|token|login|callback|exception|error|failed|warn'

# Clean up
helm uninstall cluster-0 -n kafka-cluster
kubectl delete kafka cluster-0 -n kafka-cluster --ignore-not-found
kubectl delete kafkanodepool dual-broker -n kafka-cluster --ignore-not-found
kubectl delete strimzipodset -n kafka-cluster --all --ignore-not-found     
kubectl delete secret kafka-ca oauth-ca -n kafka-cluster
kubectl delete pvc data-0-cluster-0-dual-broker-0 data-0-cluster-0-dual-broker-1 data-0-cluster-0-dual-broker-2 -n kafka-cluster
kubectl delete pvc -n kafka-cluster --all


username: my-connect-username
passwordSecret:
    secretName: scram-sha512
    password: scram-sha512-password
```