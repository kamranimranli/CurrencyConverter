# --------------------------
# BUILD STAGE
# --------------------------
    FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
    WORKDIR /src
    
    # Copy csproj files and restore packages
    COPY *.sln .
    COPY CurrencyConverter.Api/*.csproj ./CurrencyConverter.Api/
    COPY CurrencyConverter.Tests/*.csproj ./CurrencyConverter.Tests/
    RUN dotnet restore
    
    # Copy full source and publish
    COPY . .
    WORKDIR /src/CurrencyConverter.Api
    RUN dotnet publish -c Release -o /app/publish
    
    # --------------------------
    # RUNTIME STAGE
    # --------------------------
    FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
    WORKDIR /app
    
    # Copy build output from previous stage
    COPY --from=build /app/publish .
    
    # Set runtime port and environment
    ENV ASPNETCORE_URLS=http://+:8080
    EXPOSE 8080
    
    # Entry point
    ENTRYPOINT ["dotnet", "CurrencyConverter.Api.dll"]
    