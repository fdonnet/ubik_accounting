using Projects;

var builder = DistributedApplication.CreateBuilder(args);

//Auth
var keycloak = builder.AddKeycloak("auth-keycloak", 8083);

//Security API, dependencies and APP
var securityCache = builder.AddRedis("security-cache");
var postgres = builder.AddPostgres("security-postgres");
var postgresdb = postgres.AddDatabase("security-postgresdb");

var apiService = builder.AddProject<Ubik_Security_Api>("security-api")
    .WithExternalHttpEndpoints()
    .WithReference(securityCache)
    .WithReference(keycloak)
    .WithReference(postgresdb);


builder.Build().Run();
