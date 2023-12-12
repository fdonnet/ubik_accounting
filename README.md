# Ubik - Accounting

A .net8 project to manage double entry accounting.

## The Goal

Try to design a system that can be used to manage small company business.
But for now, it's an experimental project that references a lot of things about .net 8. Backend and Frontend sides of things.

## How you can help

- Look at the open issues and make a PR.
- Correct bad decisions by opening an issue to say "No, not this way".
- Share your "business" accounting skills and point out where we are conceptually wrong. (no expert here)

## Not ready for production

At this stage, DO NOT USE THIS SYSTEM on a production environnement.

- **Don't forget to change all the "secrets" exposed here, and in the configuration files.**
- **Never re-use the Keycloak realm configuration file.**

## Build and Run

At the root of the repository. "Mount" the dependencies with Docker:

`docker compose up`

It will run 4 containers:

- Redis: cache
- Rabbitmq: message bus
- Keycloak: auth external provider with a example realm file loaded at the start
- Postgres: database

### Now you are ready to run the systems

At the root folder again:

`dotnet run --launch-profile https --project ./src/Ubik.Accounting.Api/Ubik.Accounting.Api.csproj` for the backend api.

You can now access your pretty swagger tool here <https://localhost:7289/swagger>

In another terminal windows, at the root again:

`dotnet run --launch-profile https --project ./src/Ubik.Accounting.WebApp/Ubik.Accounting.WebApp.csproj`

And now, you can access the very first version of a the Blazor 8 web app here <https://localhost:7249>

### If all the things are up, you will be redirected on a Keycloak login page

3 user credentials are available (user/pass):

- testrw/test => full access
- testro/test => read only access
- testnorole/test => access the app but has no role in it

Try to log with different access rights and play with the only available "Accounts" page. Cool, now, you can dig below and in the code.

## Structure and code
