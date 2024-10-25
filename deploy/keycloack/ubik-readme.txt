helm upgrade --install ingress-nginx ingress-nginx \
  --repo https://kubernetes.github.io/ingress-nginx 

kubectl create secret generic realm-secret --from-file=ubik-realm.json

helm install keycloak-local bitnami/keycloak -f values-dev.yaml

CREATE DATABASE bitnami_keycloak; on existing postgres

kubectl create ingress keycloak-local --class=nginx \
  --rule="keycloak-local/*=keycloak-local:8080"

kubectl port-forward service/keycloak-local 8080:8080

