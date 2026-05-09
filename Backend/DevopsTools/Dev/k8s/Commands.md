kind create cluster --name learn-dot-net
kind load docker-image leduynhan1201/learn-dot-net/backend:1.0.0 --name learn-dot-net
kubectl create namespace learn-dot-net