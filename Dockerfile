# Use the .NET SDK image to build the application (ARM64 version)
FROM mcr.microsoft.com/dotnet/sdk:6.0-bullseye-slim-arm64v8 AS build
WORKDIR /src

# Copy the solution file and restore the dependencies
COPY *.sln ./
COPY ECommerce/ECommerce.csproj ECommerce/
COPY ECommerce.Data/ECommerce.Data.csproj ECommerce.Data/
COPY PostgresqlMigrations/PostgresqlMigrations.csproj PostgresqlMigrations/
COPY SqlServerMigrations/SqlServerMigrations.csproj SqlServerMigrations/
COPY ECommerce.Integration.Tests/ECommerce.Integration.Tests.csproj ECommerce.Integration.Tests/
COPY ECommerce.Tests/ECommerce.Tests.csproj ECommerce.Tests/
RUN dotnet restore

# Copy the entire source code
COPY . .

# Build the main project (WebApp)
WORKDIR /src/ECommerce
RUN dotnet publish -c Release -o /app/publish

# Use the ASP.NET Core runtime image (ARM64 version) for the final stage
FROM mcr.microsoft.com/dotnet/aspnet:6.0-bullseye-slim-arm64v8 AS final
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 80
ENTRYPOINT ["dotnet", "ECommerce.dll"]