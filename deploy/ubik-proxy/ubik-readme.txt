build and pullproxy image in your docker and Apply deployment

minikube image load ubik-proxy-test:latest
kubectl apply -f proxy-api-deploy.yaml

kubectl port-forward service/ubik-proxy 5080:80
