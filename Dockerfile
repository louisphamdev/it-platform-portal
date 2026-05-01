# Multi-stage build for IT Platform Portal
# Stage 1: Build .NET services
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS dotnet-build
WORKDIR /src
COPY src/ .
RUN find . -name "*.csproj" -exec dotnet restore {} \; && \
    find . -name "*.csproj" -exec dotnet build {} --no-restore -c Release \;

# Stage 2: Build React portal
FROM node:20-alpine AS portal-build
WORKDIR /app
COPY src/portal-shell/package*.json ./
RUN npm ci
COPY src/portal-shell/ ./
RUN npm run build

# Stage 3: Final runtime image (placeholder for actual deployment)
FROM alpine:latest AS runtime
WORKDIR /app
COPY --from=dotnet-build /src/services/*/bin/Release/* ./services/
COPY --from=portal-build /app/.next ./
CMD ["echo", "IT Platform Portal - All services built"]
