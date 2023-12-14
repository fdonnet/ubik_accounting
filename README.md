# Ubik - Accounting

A .net8 project to manage double entry accounting. (it's the very beginning of a business use case)

## The Goal

Design a system that can be used to manage small company business (accounting = first step).

But for now, it's an experimental project that references a lot of things about .net 8 - Backend and Frontend sides of things -.

Not perfect at all, but the very first goal is to have a base architecture that I can come back on when needed (for others projects etc.)

## How you can help

- Look at the open issues and make a PR.
- Correct bad technical decisions by opening an issue or propose a PR.
- Share your "business" accounting skills and point out where I m conceptually wrong. (no expert here)
- A lot of things can be upgraded in every layers, so don't hesitate, I made this repo public for that => to receive feedback.

## Not ready for production

At this stage, **DO NOT USE THIS SYSTEM ON A PRODUCTION** environnement.

- **Don't forget to change all the "secrets" exposed here, and in the configuration files.**
- **Never re-use the Keycloak realm configuration file.**

## Build and Run

At the root of the repository. "Mount" the dependencies with Docker by running this command in your terminal:

`docker compose up`

> It will "mount" 4 containers:
>
> - Redis: cache
> - Rabbitmq: message bus
> - Keycloak: auth external provider with a example realm file loaded at the start
> - Postgres: database

### Ready to play

#### For the api, in another terminal windows, at the root again

`dotnet run --launch-profile https --project ./src/Ubik.Accounting.Api/Ubik.Accounting.Api.csproj`

You can now access Swagger here <https://localhost:7289/swagger> (click on authorize before trying an endpoint, see credentials below)

#### For the Blazor app, in another terminal windows, at the root again

`dotnet run --launch-profile https --project ./src/Ubik.Accounting.WebApp/Ubik.Accounting.WebApp.csproj`

And now, you can access the very first version of a the Blazor 8 webapp here <https://localhost:7249>

*Don't change the ports of the api and the blazor apps. It's hard coded in the Blazor prj (need to be changed) because no service discovery for the moment.*

### All the things are up

If you click on authorize in Swagger or if you try to access the Blazor app, you will be redirected on a Keycloak login page

Login credentials:

| User/Pass | Role |
|----------- | -------- |
| testrw/test | full access |
| testro/test | read only access |
| testnorole/test| access the app but has no role in it |

Try to log with different access rights and play with the only available "Accounts" page.

Now you can run your preferred code editor and start to deep dive... (see below)

## Backend Api

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

Send some love on github for this projects...

In program.cs, you can access the config of:

- Jwt auth
- Ef 8
- Masstransit + outbox + filters
- Api versioning
- Swagger gen and UI with enabled auth

### Data folder

Ef 8 stuff to create and init the database and db context.

### Models folder

Ef 8 models

### Features folder

Contains the core features (in Vertical Slices mode).

- Commands and queries "pattern"
- All commands go to message bus (request/response) and are "consumed" by Masstransit consumers
- All queries access the service layer directly but can be called from another service via message bus (request/response)
- When a command is executed with success, an event is published to the message bus (pub/sub)
- Manual mapping between models and contract
- Controllers versioning
- Service and very minimal service manager (need to be reviewed)

### Functional things

In all backend layers, you find a little bit of functional programming. I m not expert at all but I think it's very useful for an API that prefers "transferring" errors than raising exceptions and to keep the code not too dirty. (see LanguageExt on github)

#### In Features / service layer

You will find some stuff like that:

```cs
public async Task<Either<IServiceAndFeatureError, AccountGroup>> AddAsync(AccountGroup accountGroup)
{
    return await ValidateIfNotAlreadyExistsAsync(accountGroup).ToAsync()
        .Bind(ac => ValidateIfParentAccountGroupExists(ac).ToAsync())
        .Bind(ac => ValidateIfClassificationExists(ac).ToAsync())
        .MapAsync(async ac =>
        {
            ac.Id = NewId.NextGuid();
            await _context.AccountGroups.AddAsync(ac);
            _context.SetAuditAndSpecialFields()
            return ac;
        });
}
```

It uses the `Either<Left,Right>` pattern and `bind` and `map` functions a lot. If at any stage of the process an `IServiceAndFeatureError` occurs, it returns to the calling function as a `LEFT` result.

Example above with an add operation:

1) Validate if not already exists
2) Validate if the specified parent account exists
3) Validate if the specified classification exists
4) Add in EF context

#### In Features / commands layer

In Masstransit consumers, the use of `.Match`:

```cs
public async Task Consume(ConsumeContext<AddAccountGroupCommand> context)
{
    var account = context.Message.ToAccountGroup()
    var result = await _serviceManager.AccountGroupService.AddAsync(account)
    await result.Match(
        Right: async r =>
        {
            //Store and publish AccountGroupAdded event
            await _publishEndpoint.Publish(r.ToAccountGroupAdded(), CancellationToken.None);
            await _serviceManager.SaveAsync();
            await context.RespondAsync(r.ToAddAccountGroupResult());
        },
        Left: async err => await context.RespondAsync(err));
}
```

Example above with an add command:

1) Receive a message from the message bus and map it to EF Model
2) Call the service
3) Match the `Either<Left,Right>`
4) If right => publish an Added event (pub/sub), SaveAsync in db
5) If right => with masstransit outbox enabled, the message is not published if the db is not updated
6) If right => send the response on message bus
7) If left => send the err response on message bus (Masstransit authorize to send `<result,error>`msg too) and it allows to not trigger an exception that will finish in an error queue if your error is a predictable one.

#### In Features / controller layer

Response from masstransit request:

```cs
[Authorize(Roles = "ubik_accounting_accountgroup_write")]
[HttpPost]
[ProducesResponseType(201)]
[ProducesResponseType(typeof(CustomProblemDetails), 400)]
[ProducesResponseType(typeof(CustomProblemDetails), 409)]
[ProducesResponseType(typeof(CustomProblemDetails), 500)]
public async Task<ActionResult<AddAccountGroupResult>> Add(AddAccountGroupCommand command, IRequestClient<AddAccountGroupCommand> client)
{
    var (result, error) = await client.GetResponse<AddAccountGroupResult, IServiceAndFeatureError>(command)
    if (result.IsCompletedSuccessfully)
    {
        var addedAccountGroup = (await result).Message;
        return CreatedAtAction(nameof(Get), new { id = addedAccountGroup.Id }, addedAccountGroup);
    }
    else
    {
        var problem = await error;
        return new ObjectResult(problem.Message.ToValidationProblemDetails(HttpContext));
    }
}
```

You can look at both case `(result,error)` tuple and separate it with `.IsCompletedSuccessfully`.

In case of error, you can send a ProblemDetails of your choice as endpoint result. And before that, access the masstransit error payload with `await error`.

At the end, your global exception handler will take care of the real exceptions raised by your system and will not trigger on predictable error cases.

## Frontend Blazor

-- Ubik.Accounting.WebApp -- / -- Ubik.Accounting.WebApp.Client -- / -- Ubik.Accounting.WebApp.Shared --

| Package | For what |
|----------- | -------- |
| [BalzorPageScript](https://github.com/MackinnonBuck/blazor-page-script) | small tool that allows Tailwind to apply its Dark or Light theme on each page an compo |
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
- The very first business components about booking accounts management
- Authorization components (depending on authorize state)
- Minimal common components (Alerts, Buttons, Form Inputs, Grid *(Microsoft inspired/copied)*, Modal, Spinners)
- Tailwind Flowbite design for components
- The implementation of the facade that call the reverse proxy controller that call the Backend api for automode

### Ubik.Accounting.WebApp.Shared

(shared side)

- All the facades (interfaces) that need to be implemented on server and client side
- All the services implementation that can run both side without specific implementation

## Tests

-- Ubik.Accounting.Api.Tests.Integration -- / -- Ubik.Accounting.Api.Tests.UnitTest --

- Unit test all the bus messages with masstransit (be sure about request/ response and publish)
- In integration tests, usage of testcontainers to test the backend api layers (controllers, service, and queries because they are not called from controllers)
- Not a lot of unit tests now, because the business logic is very very limited
- Not very clean part, can do better ....

## Others

-- Ubik.Accounting.ApiService.Common -- / -- Ubik.Db.Common -- / -- Ubik.Accounting.Contracts --

- Common config things that can be reuse in other projects
- This part will maybe grow
- Contracts prj is to store all the communication records (command, queries, results etc)
