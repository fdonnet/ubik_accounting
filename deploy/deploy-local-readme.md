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

## Install ingress-nginx 

Normal ingress will not work if you use wsl.

`helm repo add ingress-nginx https://kubernetes.github.io/ingress-nginx`

`helm upgrade --install ingress-nginx ingress-nginx/ingress-nginx --set controller.config.allow-snippet-annotations="true"`

## Keycloak

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

### Install keycloack values-dev.yaml:

Contains config to access the already existing postgres db

`helm install keycloak-local bitnami/keycloak -f ./keycloack/values-dev.yaml`

### Generate self signed certificate (if you want to change them in ingress)

To remember the commands only (you can skip this part):

`openssl req -newkey rsa:2048 -nodes -keyout tls.key -out tls.csr`

`openssl x509 -req -sha256 -days 365 -in tls.csr -signkey tls.key -out tls.crt`

### Apply ingress deployment

`kubectl apply -f ./keycloack/ingress-for-keycloack.yaml`

## Redis

`helm install ubik-cache bitnami/redis -f ./redis/values-dev.yaml`

## Accounting Structure Api

Build image

`docker build -t ubik-accounting-structure-api-test:latest -f ../src/Ubik.Accounting.Structure.Api/Dockerfile ../`

Load image in minikube

`minikube image load ubik-accounting-structure-api-test:latest`

Apply deployement

`kubectl apply -f ./accounting-structure-api/accounting-structure-api-deploy.yaml`

## Accounting Transaction Api

Build image

`docker build -t ubik-accounting-tx-api-test:latest -f ../src/Ubik.Accounting.Transaction.Api/Dockerfile ../`

Load image in minikube

`minikube image load ubik-accounting-tx-api-test:latest`

Apply deployement

`kubectl apply -f ./accounting-transaction-api/accounting-transaction-api-deploy.yaml`

## Accounting Sales Vat Tax Api

Build image

`docker build -t ubik-accounting-sales-vat-tax-api-test:latest -f ../src/Ubik.Accounting.SalesOrVatTax.Api/Dockerfile ../`

Load image in minikube

`minikube image load ubik-accounting-sales-vat-tax-api-test:latest`

Apply deployement

`kubectl apply -f ./accounting-tax-api/accounting-tax-api-deploy.yaml`

## Security api

Build image

`docker build -t ubik-security-api-test:latest -f ../src/Ubik.Security.Api/Dockerfile ../`

Load image in minikube

`minikube image load ubik-security-api-test:latest`

Apply deployement

`kubectl apply -f ./security-api/security-api-deploy.yaml`

## Ubik proxy (Yarp)

Build image

`docker build -t ubik-proxy-test:latest -f ../src/Ubik.YarpProxy/Dockerfile ../`

Load image in minikube

`minikube image load ubik-proxy-test:latest`

Apply deployement

`kubectl apply -f ./ubik-proxy/proxy-api-deploy.yaml`

## Ubik webapp

Build image

`docker build -t ubik-webapp-test:latest -f ../src/Ubik.Accounting.WebApp/Dockerfile ../`

Load image in minikube

`minikube image load ubik-webapp-test:latest`

Apply deployement

`kubectl apply -f ./ubik-webapp/webapp-deploy.yaml`

## Add param in your local hosts file

- 127.0.0.1  keycloak-local
- 127.0.0.1  ubik-proxy
- 127.0.0.1  ubik-webapp

## Start minikube tunnel

It will open your nginx/ingress services to your local host.

`minikube tunnel`

## The End

After all this configuration, now, you can access backend Swagger here:

http://ubik-proxy/swagger

and the Blazor webapp here:

https://ubik-webapp