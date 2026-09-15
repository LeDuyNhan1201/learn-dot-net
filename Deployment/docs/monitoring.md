```shell
# Create namespace for monitoring
kubectl create namespace monitoring

# Install helm charts
helm repo add prometheus-community https://prometheus-community.github.io/helm-charts
helm repo update
helm search repo prometheus-community/kube-prometheus-stack

# Create certificates for services
kubectl create secret generic monitoring-ca --from-file=ca.crt=certs/ca/grafana.ca.crt -n monitoring
kubectl create secret tls grafana-tls --cert=certs/grafana/grafana.cert.pem --key=certs/grafana/grafana.key.pem -n monitoring
kubectl create secret tls prometheus-tls --cert=certs/prometheus/prometheus.cert.pem --key=certs/prometheus/prometheus.key.pem -n monitoring
kubectl create secret tls alertmanager-tls --cert=certs/alertmanager/alertmanager.cert.pem --key=certs/alertmanager/alertmanager.key.pem -n monitoring
kubectl create secret generic grafana-client-tls --from-file=ca.crt=certs/ca/grafana.ca.crt --from-file=client.crt=certs/grafana/grafana-client.cert.pem --from-file=client.key=certs/grafana/grafana-client.key.pem -n monitoring

# Deploy Prometheus, Grafana and Alertmanager using Helm
helm upgrade --install monitoring prometheus-community/kube-prometheus-stack --namespace monitoring --values monitoring/values.yaml

# Check the status of Prometheus pods, services and deployments
kubectl get namespaces
kubectl get svc -n monitoring -w -o wide
kubectl get pods -n monitoring -w -o wide
kubectl get deployment -n monitoring -w -o wide

kubectl get grafana -n monitoring -w
kubectl get prometheus -n monitoring -w
kubectl get alertmanager -n monitoring -w

# Port-forward services to localhost and log output to tmp/logs/monitoring/ for debugging
nohup kubectl port-forward svc/monitoring-grafana 3456:80 -n monitoring > tmp/logs/monitoring/grafana.log 2>&1 &
nohup kubectl port-forward svc/monitoring-kube-prometheus-prometheus 9090:9090 -n monitoring > tmp/logs/monitoring/prometheus.log 2>&1 &
nohup kubectl port-forward svc/monitoring-kube-prometheus-alertmanager 9093:9093 -n monitoring > tmp/logs/monitoring/alertmanager.log 2>&1 &

# Stop port-forwarding processes if they are already running
pkill -f 'kubectl.*port-forward.*3456:80'
pkill -f 'kubectl.*port-forward.*9090:9090'
pkill -f 'kubectl.*port-forward.*9093:9093'

# Test Prometheus alerting rules by applying test-alert.yaml
kubectl apply -f monitoring/templates/test-alert.yaml

curl -s http://localhost:9090/api/v1/query --data-urlencode 'query=up' | jq
curl -s http://localhost:9093/api/v2/alerts | jq

# Clean up monitoring resources
helm uninstall monitoring -n monitoring && kubectl delete namespace monitoring

# Configure systemd-resolved to use minikube DNS for the monitoring domain
sudo resolvectl revert enp4s0
sudo mkdir -p /etc/systemd/resolved.conf.d
sudo tee /etc/systemd/resolved.conf.d/minikube.conf > /dev/null <<EOF
[Resolve]
DNS=192.168.49.2
Domains=~monitoring
EOF
sudo systemctl restart systemd-resolved

# Check the DNS resolution for the monitoring domain
resolvectl status
resolvectl query grafana.monitoring
```