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
COPY ["src/Ubik.Accounting.Transaction.Api/Ubik.Accounting.Transaction.Api.csproj", "src/Ubik.Accounting.Transaction.Api/"]
COPY ["src/Ubik.Accounting.SalesOrVatTax.Contracts/Ubik.Accounting.SalesOrVatTax.Contracts.csproj", "src/Ubik.Accounting.SalesOrVatTax.Contracts/"]
COPY ["src/Ubik.Accounting.Structure.Contracts/Ubik.Accounting.Structure.Contracts.csproj", "src/Ubik.Accounting.Structure.Contracts/"]
COPY ["src/Ubik.Accounting.Transaction.Contracts/Ubik.Accounting.Transaction.Contracts.csproj", "src/Ubik.Accounting.Transaction.Contracts/"]
COPY ["src/Ubik.ApiService.Common/Ubik.ApiService.Common.csproj", "src/Ubik.ApiService.Common/"]
COPY ["src/Ubik.Db.Common/Ubik.DB.Common/Ubik.DB.Common.csproj", "src/Ubik.Db.Common/Ubik.DB.Common/"]
RUN dotnet restore "./src/Ubik.Accounting.Transaction.Api/Ubik.Accounting.Transaction.Api.csproj"
COPY . .
WORKDIR "/src/src/Ubik.Accounting.Transaction.Api"
RUN dotnet build "./Ubik.Accounting.Transaction.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Ubik.Accounting.Transaction.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Ubik.Accounting.Transaction.Api.dll"]
