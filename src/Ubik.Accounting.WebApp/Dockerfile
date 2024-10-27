# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /
COPY ["src/Ubik.Accounting.WebApp/Ubik.Accounting.WebApp.csproj", "src/Ubik.Accounting.WebApp/"]
COPY ["src/Ubik.Accounting.WebApp.Client/Ubik.Accounting.WebApp.Client.csproj", "src/Ubik.Accounting.WebApp.Client/"]
COPY ["src/Ubik.Accounting.Webapp.Shared/Ubik.Accounting.Webapp.Shared.csproj", "src/Ubik.Accounting.Webapp.Shared/"]
COPY ["src/Ubik.Accounting.Contracts/Ubik.Accounting.Contracts.csproj", "src/Ubik.Accounting.Contracts/"]
COPY ["src/Ubik.Contracts.Common/Ubik.Contracts.Common.csproj", "src/Ubik.Contracts.Common/"]
COPY ["src/Ubik.ApiService.Common/Ubik.ApiService.Common.csproj", "src/Ubik.ApiService.Common/"]
COPY ["src/Ubik.Security.Contracts/Ubik.Security.Contracts.csproj", "src/Ubik.Security.Contracts/"]
#COPY ["src/mycert.cer", "src/"]
#ADD ./src/mycert.cer /usr/local/share/ca-certificates/mycert.cer
#ADD ./src/mycert.cer /etc/ssl/certs/mycert.cer
#RUN chmod 644 /usr/local/share/ca-certificates/mycert.cer && update-ca-certificates
RUN dotnet restore "./src/Ubik.Accounting.WebApp/Ubik.Accounting.WebApp.csproj"
COPY . .
WORKDIR "/src/Ubik.Accounting.WebApp"
RUN cp ./wwwroot/appsettings-minikube.json ./wwwroot/appsettings.json
RUN dotnet build "./Ubik.Accounting.WebApp.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Ubik.Accounting.WebApp.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Ubik.Accounting.WebApp.dll"]
