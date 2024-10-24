NEW BIG PR implemented:

- security api and tenant management
- remove authorization via roles from keycloack
- implement Yarp as a proxy and authorization manager (call the security api and forward request to backend apis, authorization via policy requierments for each route)
- remove masstransit for request/response, keep it only for events (pub/sub)
- review and simplify the test parts (complete review) => only integration tests for the moment. No real backend functions to unit test.
- Blazor app now calls the Yarp proxy

# Ubik - Accounting

A .net8 project to manage double entry accounting. (it's the very beginning of a business use case)

## The Goal

Microservices arch and supports multi-tenants.

But for now, it's an experimental project that references a lot of things about .net 8 - Backend and Frontend sides of things -.

## Not ready for production

At this stage, **DO NOT USE THIS SYSTEM ON A PRODUCTION** environnement.

- **Don't forget to change all the "secrets" exposed here, and in the configuration files.**
- **Never re-use the Keycloak realm configuration file.**

## Build and Run

At the root of the repository. "Mount" the dependencies with Docker by running this command in your terminal:

`docker compose -f .\docker-compose.yml -f .\docker-integration-tests.yml up -d`

> It will "mount" all backend containers:
>
> - Redis: cache (one for webapp, one for Yarp proxy)
> - Rabbitmq: message bus
> - Keycloak: auth external provider with a example realm file loaded at the start
> - Postgres: database (one with serveral DBs)
> - Pgadmin: to admin your dbs if needed
> - Apis: backend apis (security/accounting) for integration testing

### Ready to play

#### Run backend Apis

`dotnet run --launch-profile https --project ./src/Ubik.Accounting.Api/Ubik.Accounting.Api.csproj`

`dotnet run --launch-profile https --project ./src/Ubik.Security.Api/Ubik.Security.Api.csproj`

#### Run Yarp proxy

`dotnet run --launch-profile https --project ./src/Ubik.YarpProxy/Ubik.YarpProxy.csproj`

#### Run Blazor app 

`dotnet run --launch-profile https --project ./src/Ubik.Accounting.WebApp/Ubik.Accounting.WebApp.csproj`

And now, you can access the very first version of a the Blazor 8 webapp here <https://localhost:7249>

*Don't change the ports of the api and the blazor apps. It's hard coded in the Blazor prj (need to be changed) because no service discovery for the moment.*
*In Debug, create a multiple project startup that runs all this stuff*

### All the things are up

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

## Yarp Proxy

-- Ubik.YarpProxy --

Manages all the authorization stuff by calling the security api and forward the requests to the backend. (cool af)

## Security Api

-- Ubik.Security.Api --

Used by the proxy to manage authorizations and admin and user endpoints to manages the authorizations/users/tenants config.

## Accounting Api

-- Ubik.Accounting.Api --

Some used external libs:

| Package | For what |
|----------- | -------- |
| [Asp.Versioning](https://github.com/dotnet/aspnet-api-versioning) | automatic versioning of API (controllers / endpoints) |
| EfCore | commands and ez queries + data init etc |
| [Dapper](https://github.com/DapperLib/Dapper) | some read queries |
| [EFCore.NamingConvention](https://github.com/efcore/EFCore.NamingConventions) | force EF core to postgres SnakeCase => way better if you need to use Dapper too  |
| [LanguageExt.Core](https://github.com/louthy/language-ext) | use Either<Left, Right> pattern |
| [Masstransit](https://github.com/MassTransit/MassTransit) | message bus abstraction + inbox/outbox pattern |

## Features folder in backend Apis

Contains the core features (in Vertical Slices mode).

- Command and query services
- When a command is executed with success, an event is published to the message bus (pub/sub)
- Functional `Either<Error, Result>` patterns in all layers to transfer errors between layer and to keep the code not too dirty. (not an expert but I like it)

## Frontend Blazor

-- Ubik.Accounting.WebApp -- / -- Ubik.Accounting.WebApp.Client -- / -- Ubik.Accounting.WebApp.Shared --

First you can maybe uncomment this lines in Ubik.Accounting.WebApp.csproj, if you want to play with Tailwind, or go your own way with Tailwind cli and dotnet watch:

```xml
  <Target Name="PostBuild" AfterTargets="PostBuildEvent">
    <Exec Command="npx tailwindcss -i ./Styles/app.css -o ./wwwroot/css/app.css" />
  </Target>
```

I don't know why, but my build fails in Github actions if I let this Tailwind instructions (related to the forms plugin). If someone has an idea...
`EXEC : error : Cannot find module '@tailwindcss/forms'`

**To use Tailwind in DEV**, you need to have node installed, Tailwind and Tailwind forms... I let you go on their site for installation instructions. Maybe, `npm install` command can be sufficient in the WebApp (server) project, cannot be sure because it seems to trigger a weird error in the dotnet build github actions.

| Package | For what |
|----------- | -------- |
| [BalzorPageScript](https://github.com/MackinnonBuck/blazor-page-script) | small tool that allows Tailwind to apply its Dark or Light theme on each page and component |
| [IdentityModel](https://github.com/IdentityModel) | Small extension to refresh token in OpenIdC |

Send some love on github to this projects...

### Ubik.Accounting.WebApp

(server side)

- Static components and pages
- Typed HttpClient to access the backend api
- Tailwind config - Tailwind Flowbite design layout etc
- A very minimal reverse proxy controller that allows components (automode) to call the backend api when they are WASM.
- Some stuff about security (Token cache service, UserService with circuit, middleware)

In program.cs, you can access the config of:

- Redis for token caching
- CascadingAuthenticationState
- Cookie auth with OpenIdC (connection + refresh token in OnValidatePrincipal)
- ...

### Ubik.Accounting.WebApp.Client

(client-auto side)

- All components are able to run in auto mode (InteractiveServer or InteractiveWasm)
- Authorization components (depending on authorized state)
- Minimal common components (Alerts, Buttons, Form Inputs, Grid *(Microsoft inspired/copied)*, Modal, Spinners)
- Tailwind Flowbite design for components
- The implementation of the facade that call the reverse proxy controller that call the Backend api for automode
- Error components that manage problemdetails returns from backend api (try to add a booking account with an existing code as an example) 

### Ubik.Accounting.WebApp.Shared

(shared side)

- All the facades (interfaces) that need to be implemented on server and client side
- All the services implementation that can run both sides without specific implementation

## Tests

-- Ubik.Accounting.Api.Tests.Integration -- 

- In integration tests => test all proxy endpoints

## Others

-- Ubik.Accounting.ApiService.Common -- / -- Ubik.Db.Common -- / -- Ubik.Accounting.Contracts --

- Common config things that can be reused in other projects
- This part will maybe grow
- Contracts prj is to store all the communication records (command, queries, results etc)
