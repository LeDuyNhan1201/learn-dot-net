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

kubectl create secret generic oauth-ca --from-file=certs/ca.crt=ca.crt -n kafka
kubectl create secret generic server-certs --from-file=certs/server.crt=server.crt --from-file=certs/server.key=server.key -n kafka
kubectl create secret generic scram-sha512 --from-file=secrets/scram-sha512-password.txt=secrets/scram-sha512-password.txt -n kafka

username: my-connect-username
passwordSecret:
    secretName: scram-sha512
    password: scram-sha512-password
```