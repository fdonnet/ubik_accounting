using Aspire.Hosting;

var builder = DistributedApplication.CreateBuilder(args);

//Postgres
var postgresUsername = builder.AddParameter("postgres-username", secret: true);
var postgresPassword = builder.AddParameter("postgres-password", secret: true);

var db = builder.AddPostgres("ubik-postgres-aspire", postgresUsername, postgresPassword)
    .WithDataVolume(isReadOnly: false)
    .WithPgAdmin()
    .WithLifetime(ContainerLifetime.Persistent);

//Keycloak
var keycloakUsername = builder.AddParameter("keycloak-username", secret: true);
var keycloakPassword = builder.AddParameter("keycloak-password", secret: true);

var keycloak = builder.AddKeycloak("ubik-keycloak-aspire", 8080, keycloakUsername, keycloakPassword)
    .WithRealmImport("../../tests/Ubik.Api.Tests.Integration/import")
    .WithLifetime(ContainerLifetime.Persistent);

//RabbitMQ
var rabbitUsername = builder.AddParameter("rabbit-username", secret: true);
var rabbitPassword = builder.AddParameter("rabbit-password", secret: true);
var rabbitmq = builder.AddRabbitMQ("ubik-rabbitmq-aspire",rabbitPassword,rabbitPassword)
    .WithLifetime(ContainerLifetime.Persistent);

//Redis
var redis = builder.AddRedis("ubik-redis-aspire")
    .WithLifetime(ContainerLifetime.Persistent);

//Accounting SalesOrVatTax API
var accountingSalesVatTaxDb = db.AddDatabase("ubik-db-accounting-salesorvattax-aspire");
var accountingSalesVatTaxApi = builder.AddProject<Projects.Ubik_Accounting_SalesOrVatTax_Api>("ubik-api-accounting-salesorvattax-aspire")
    .WaitFor(keycloak)
    .WaitFor(rabbitmq)
    .WithEnvironment("ConnectionStrings__AccountingSalesTaxDbContext", accountingSalesVatTaxDb.Resource.ConnectionStringExpression)
    .WithEnvironment("MessageBroker__Host", rabbitmq.Resource.ConnectionStringExpression);

//Accounting Structure
var accountingStructureDb = db.AddDatabase("ubik-db-accounting-structure-aspire");
var accountingStructureApi = builder.AddProject<Projects.Ubik_Accounting_Structure_Api>("ubik-api-accounting-structure-aspire")
    .WaitFor(keycloak)
    .WaitFor(rabbitmq)
    .WaitFor(redis)
    .WithEnvironment("ConnectionStrings__AccountingContext", accountingStructureDb.Resource.ConnectionStringExpression)
    .WithEnvironment("MessageBroker__Host", rabbitmq.Resource.ConnectionStringExpression);

//Accounting Transaction
var accountingTxDb = db.AddDatabase("ubik-db-accounting-tx-aspire");
var accountingTxApi = builder.AddProject<Projects.Ubik_Accounting_Transaction_Api>("ubik-api-accounting-tx-aspire")
    .WaitFor(keycloak)
    .WaitFor(rabbitmq)
    .WaitFor(redis)
    .WithEnvironment("ConnectionStrings__AccountingTxContext", accountingTxDb.Resource.ConnectionStringExpression)
    .WithEnvironment("MessageBroker__Host", rabbitmq.Resource.ConnectionStringExpression);

//Security API
var securityDb = db.AddDatabase("ubik-db-security-aspire");
var securityApi = builder.AddProject<Projects.Ubik_Security_Api>("ubik-api-security-aspire")
    .WaitFor(securityDb)
    .WaitFor(keycloak)
    .WaitFor(rabbitmq)
    .WaitFor(redis)
    .WithEnvironment("ConnectionStrings__SecurityDbContext", securityDb.Resource.ConnectionStringExpression)
    .WithEnvironment("MessageBroker__Host", rabbitmq.Resource.ConnectionStringExpression);

//Yarp proxy
var yarp = builder.AddProject<Projects.Ubik_YarpProxy>("ubik-proxy-yarp-aspire")
    .WaitFor(securityDb)
    .WaitFor(keycloak)
    .WaitFor(rabbitmq)
    .WaitFor(redis)
    .WaitFor(securityApi)
    .WaitFor(accountingSalesVatTaxApi)
    .WaitFor(accountingStructureApi)
    .WaitFor(accountingTxApi)
    .WithEnvironment("RedisCache__ConnectionString", redis.Resource.ConnectionStringExpression);

//Webapp
builder.AddProject<Projects.Ubik_Accounting_WebApp>("ubik-webapp-aspire")
    .WaitFor(yarp)
    .WithEnvironment("RedisCache__ConnectionString", redis.Resource.ConnectionStringExpression);

builder.Build().Run();
