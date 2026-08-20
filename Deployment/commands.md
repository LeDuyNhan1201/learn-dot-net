```shell
minikube start --driver=docker --cpus=6 --memory=12g

kubectl get nodes
minikube status

kubectl create namespace kafka
kubectl get namespaces

helm repo add strimzi https://strimzi.io/charts/
helm repo update
helm search repo strimzi
helm install strimzi-kafka-operator strimzi/strimzi-kafka-operator --namespace kafka

kubectl get pods -n kafka
kubectl get deployment -n kafka
kubectl get crd | grep kafka
```