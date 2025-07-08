FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy solution file
COPY ["TravelRequests.sln", "."]

# Copy all project files
COPY ["src/TravelRequests.Api/consware-api.csproj", "src/TravelRequests.Api/"]
COPY ["src/TravelRequests.Application/TravelRequests.Application.csproj", "src/TravelRequests.Application/"]
COPY ["src/TravelRequests.Domain/TravelRequests.Domain.csproj", "src/TravelRequests.Domain/"]
COPY ["src/TravelRequests.Infrastructure/TravelRequests.Infrastructure.csproj", "src/TravelRequests.Infrastructure/"]
COPY ["tests/TravelRequests.Tests/TravelRequests.Tests.csproj", "tests/TravelRequests.Tests/"]

# Restore dependencies
RUN dotnet restore "src/TravelRequests.Api/consware-api.csproj"

# Copy all source code
COPY . .

# Build the application
WORKDIR "/src/src/TravelRequests.Api"
RUN dotnet build "consware-api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "consware-api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Create logs directory
RUN mkdir -p /app/logs

# Set environment variables
ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_HTTP_PORTS=8080
ENV ASPNETCORE_HTTPS_PORTS=8081

ENTRYPOINT ["dotnet", "consware-api.dll"]
