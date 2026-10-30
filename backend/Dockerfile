# A local development image, not a production one: production container images
# are operations work and are out of scope for the technical foundation.
FROM mcr.microsoft.com/dotnet/sdk:10.0

# curl is here only so that Compose can poll the readiness endpoint.
RUN apt-get update \
    && apt-get install --yes --no-install-recommends curl \
    && rm -rf /var/lib/apt/lists/*

WORKDIR /src

# .editorconfig carries the analyzer configuration, so the build needs it too.
COPY .editorconfig Directory.Build.props Directory.Packages.props Platform.slnx global.json ./
COPY src/ ./src/
COPY tests/ ./tests/

RUN dotnet restore Platform.slnx

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

CMD ["dotnet", "run", "--project", "src/Platform.Host/Platform.Host.csproj", "--no-launch-profile"]
