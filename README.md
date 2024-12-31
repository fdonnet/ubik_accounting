# News

## Info

2024-12-31: I am currently working on another side project: a Security API + FluentUI Blazor frontend. This project will enable the configuration of an authorization layer for YARP with multi-tenancy support etc. I may integrate that into this repository or create a new one for it.

## LAST PR implemented:

- Aspire 9.0 (very minimal implementation without service discovery or test project)
- Now, you can run the apphost project and have fun

## Next steps or in progress:

~~- Hybrid cache for users and tokens (done)~~
- Vat-sales tax module implementation
- .Net 9 (Blazor adaptations)
- SingalR hub to trace tx status
- Blazor double-entry ui
- Aspire, enhance service defaults and see for discovery and maybe tests project
- will see...

# Ubik - Accounting

A .net9 project to manage double entry accounting. (it's the very beginning of a business use case)

## The Goal

Microservices arch and supports multi-tenants.

But for now, it's an experimental project that references a lot of things about .net 9 - Backend and Frontend sides of things -.

## Not ready for production

At this stage, **DO NOT USE THIS SYSTEM ON A PRODUCTION** environnement.

- **Don't forget to change all the "secrets" exposed here, and in the configuration files.**
- **Never re-use the Keycloak realm configuration file.**

# Run - Debug

Choose between section 1), 2) or 3)

## 1) For the Aspire guys

You can run all the stuff with the host project:

`dotnet run --project .\src\Ubik.AppHost\Ubik.AppHost.csproj`

Or, in Visual Studio, you can set the Aspire Host project as project startup and you will be able to play/debug with all the dependencies.

Rerun after the first initial load. The keycloack container is not fully ready the first time. But after a rerun of the AppHost project, it will works.

## 2) For the Kubernetes/Minikube guys

For detailed instructions on deploying locally with Minikube, please refer to the [local deployment guide](./deploy/deploy-local-readme.md).

## 3) For the Docker guys

**At the root of the repository**, "Mount" the dependencies with Docker by running this command in your terminal:

`docker compose -f .\docker-compose.yml -f .\docker-integration-tests.yml up -d`

> It will "mount" all backend containers:
>
> - Redis: cache (one for webapp, one for Yarp proxy)
> - Rabbitmq: message bus
> - Keycloak: auth external provider with a example realm file loaded at the start
> - Postgres: database (one with serveral schemas)
> - Pgadmin: to admin your dbs if needed
> - Apis: backend apis (security/accounting) for integration testing

#### Run backend Apis or define a multiple projects startup to debug

`dotnet run --launch-profile https --project ./src/Ubik.Accounting.SalesOrVatTax.Api/Ubik.Accounting.SalesOrVatTax.Api.csproj`

`dotnet run --launch-profile https --project ./src/Ubik.Accounting.Transaction.Api/Ubik.Accounting.Transaction.Api.csproj`

`dotnet run --launch-profile https --project ./src/Ubik.Accounting.Structure.Api/Ubik.Accounting.Structure.Api.csproj`

`dotnet run --launch-profile https --project ./src/Ubik.Security.Api/Ubik.Security.Api.csproj`

#### Run Yarp proxy

`dotnet run --launch-profile https --project ./src/Ubik.YarpProxy/Ubik.YarpProxy.csproj`

#### Run Blazor app 

`dotnet run --launch-profile https --project ./src/Ubik.Accounting.WebApp/Ubik.Accounting.WebApp.csproj`

## For All

After choosing your prefered way 1), 2) 3) you can access the Blazor app here: 

<https://localhost:7249> for Aspire and Docker and here <https://ubik-webapp> for the brave Minikube users.

# All the things are up

If you try to access the Blazor app, you will be redirected on a Keycloak login page

Login credentials:

| User/Pass | Role |
|----------- | -------- |
| testrw@test.com/test | full access |
| testro@test.com/test | read only access |
| testnorole@test.com/test| access the app but has no role in it |
| testothertenant@test.com/test| access the app but with rights on another tenant |
| admin@test.com/test| megaadmin right to call admin endpoints |

Try to log with different access rights and play with the only available "Accounts" and "Classifications" pages.

Now you can run your preferred code editor and start to deep dive... (see below)

# Integration tests

If you want to contribute, see section "3) For the Docker guys" to be able to mount the dependencies and run the integration tests project. No time to update to Aspire Test and change all the related stuff (github actions etc.). 

At the end, simply run `docker compose -f .\docker-compose.yml -f .\docker-integration-tests.yml up -d`, and you will be able to run the tests project.

## External libs

| Package | For what |
|----------- | -------- |
| [Asp.Versioning](https://github.com/dotnet/aspnet-api-versioning) | automatic versioning of API (controllers / endpoints) |
| EfCore | commands and ez queries + data init etc |
| [Dapper](https://github.com/DapperLib/Dapper) | some read queries |
| [EFCore.NamingConvention](https://github.com/efcore/EFCore.NamingConventions) | force EF core to postgres SnakeCase => way better if you need to use Dapper too  |
| [LanguageExt.Core](https://github.com/louthy/language-ext) | use Either<Left, Right> pattern |
| [Masstransit](https://github.com/MassTransit/MassTransit) | message bus abstraction + inbox/outbox pattern |

Send some love on github to this projects...

## Yarp Proxy

-- Ubik.YarpProxy --

Manages all the authorization stuff by calling the security api and forward the requests to the backend.

## Security Api

-- Ubik.Security.Api --

Used by the proxy to manage authorizations and exposes admin and user endpoints to manage the authorizations/users/tenants config.

## Accounting structure Api

-- Ubik.Accounting.Structure.Api --

Manages the accounting structure.

## Accounting tx Api

-- Ubik.Accounting.Transaction.Api --

For the moment, implement the submit Tx endpoint and create some states and events related to future Txs management.

## Accounting sales or VAT tax

-- Ubik.Accounting.SalesOrVatTax.Api --

Will implement all the rules related to sales or VAT taxes and validate a Tx when needed. Will be used to declare and enforce the rules. (for each country etc)

## Features folder in backend Apis

Contains the core features (in Vertical Slices mode).

- Command and query services
- When a command is executed with success, an event is published to the message bus (pub/sub)
- Functional `Either<Error, Result>` patterns in all layers to transfer errors between layer and to keep the code not too dirty.

## Frontend Blazor

-- Ubik.Accounting.WebApp -- / -- Ubik.Accounting.WebApp.Client -- / -- Ubik.Accounting.WebApp.Shared --

First you can maybe uncomment this lines in Ubik.Accounting.WebApp.csproj, if you want to play with Tailwind, or go your own way with Tailwind cli and dotnet watch:

```xml
  <Target Name="PostBuild" AfterTargets="PostBuildEvent">
    <Exec Command="npx tailwindcss -i ./Styles/app.css -o ./wwwroot/css/app.css" />
  </Target>
```

**To use Tailwind in DEV**, you need to have node installed, Tailwind and Tailwind forms... I let you go on their site for installation instructions. Maybe, `npm install` command can be sufficient in the WebApp (server) project, cannot be sure because it seems to trigger a weird error in the dotnet build github actions.

| Package | For what |
|----------- | -------- |
| [BalzorPageScript](https://github.com/MackinnonBuck/blazor-page-script) | small tool that allows Tailwind to apply its Dark or Light theme on each page and component |

Send some love on github to this projects...

### Ubik.Accounting.WebApp

(server side)

- Static components and pages
- A very minimal reverse proxy controller that allows components (automode) to call the backend api when they are WASM.
- Some stuff about security (Token cache service)

=> next, implement new .Net 9 Blazor stuff related to authentication and render modes.

### Ubik.Accounting.WebApp.Client

(client-auto side)

- **All components are able to run in auto mode (InteractiveServer or InteractiveWasm)**
- Authorization components (depending on authorized state)
- Minimal common components (Alerts, Buttons, Form Inputs, Grid *(Microsoft inspired/copied)*, Modal, Spinners)
- Tailwind Flowbite design for components
- Error components that manage problemdetails returns from backend api (try to add a booking account with an existing code as an example) 

### Ubik.Accounting.WebApp.Shared

(shared side)

- All the facades (interfaces) that need to be implemented on server and client side
- All the services implementation that can run both sides without specific implementation

## Tests

-- Ubik.Accounting.Api.Tests.Integration -- 

- In integration tests => test all proxy endpoints 

## Others

-- Ubik.Accounting.ApiService.Common -- / -- Ubik.Db.Common -- / -- Ubik.xxx.Contracts --

- Common config things that can be reused in other projects
- This part will maybe grow
- Contracts prj is to store all the communication records (command, queries, results etc)
