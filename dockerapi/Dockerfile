# Multi Staged build in Production Environment
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
# Ensure we listen on any IP Address
ENV \
    # Configure web servers to bind to port 80 when present
    ASPNETCORE_URLS=http://+:80 \
    # Enable detection of running in a container
    DOTNET_RUNNING_IN_CONTAINER=true
WORKDIR /app

# Build project in this stage
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["dockerapi.csproj", "./"]
RUN dotnet restore "./dockerapi.csproj"
COPY . .
RUN dotnet publish "dockerapi.csproj" -c Release -o /app/publish

# Go back to initial base stage and set it final
FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "dockerapi.dll"]