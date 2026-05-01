FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY src/ ./src/
RUN find src -name "*.csproj" -exec dotnet restore {} \; && \
    find src -name "*.csproj" -exec dotnet build {} --no-restore -c Release \;

FROM node:20-alpine AS portal
WORKDIR /app
COPY src/portal-shell/package*.json ./
# Use npm install instead of npm ci (works without package-lock.json)
RUN npm install
COPY src/portal-shell/ ./
RUN npm run build

FROM alpine:latest AS runtime
WORKDIR /app
COPY --from=build /src/src/services/*/bin/Release/* ./services/ || true
COPY --from=portal /app/.next ./
COPY --from=portal /app/public ./public || true
CMD ["echo", "IT Platform Portal - All services built"]
