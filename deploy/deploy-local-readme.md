# Minikube Deploy

## Prerequisites

- Minikube (local kubernetes) started
- Helm installed
- Bitnami repo added in Helm

> **Warning**: Ensure you are in the `deploy` folder before running the following commands.

## Postgres

`helm install ubik-postgres bitnami/postgresql -f ./postgresql/values-dev.yaml`

## RabbitMQ

`helm install ubik-rabbitmq bitnami/rabbitmq -f ./rabbitmq/values-dev.yaml`


## Keycloak

### Install ingress-nginx because normal ingress will not work if you use wsl.

`helm upgrade --install ingress-nginx ingress-nginx \
  --repo https://kubernetes.github.io/ingress-nginx`

### Import the test ubik realm in a secret

`kubectl create secret generic realm-secret --from-file=../tests/Ubik.Api.Tests.Integration/import/ubik-realm.json`

*Dont'forget to reload it if you change some configs.*

### Create keycloakc db in your exising postgres

Look at the command when you created your postges via helm. It can change because I use the last bitnami img (version chan change). But you know....

Export your password

`export POSTGRES_PASSWORD=$(kubectl get secret --namespace default ubik-postgres-postgresql -o jsonpath="{.data.postgres-password}" | base64 -d)`

Run postgres client

```bash
kubectl run ubik-postgres-postgresql-client --rm --tty -i --restart='Never' --namespace default --image docker.io/bitnami/postgresql:17.0.0-debian-12-r9 --env="PGPASSWORD=$POSTGRES_PASSWORD" \
    --command -- psql --host ubik-postgres-postgresql -U postgres -d postgres -p 5432
```

Create the db

`CREATE DATABASE keycloak;`

### Install keycloack values-dev.yaml: contains config to access the already existing postgres db

`helm install keycloak-local bitnami/keycloak -f ./keycloack/values-dev.yaml`

### Apply ingress deployment

`kubectl apply -f ./keycloack/ingress-for-keycloack.yaml`


