# Multi-Environment Deployment Guide

## Overview

This project supports three environments with the following configuration:

| Environment | URL | Branch | Auto Deploy | Database |
|-------------|-----|--------|-------------|----------|
| UAT | uat.it-platform.internal | main | ✅ Yes | postgres-uat |
| Staging | staging.it-platform.internal | staging | ✅ Yes | postgres-staging |
| Production | it-platform.internal | release/v* | ❌ Manual | postgres-prod |

## Quick Start

### Local Development

```bash
# UAT environment
docker-compose -f docker-compose.yml -f docker-compose.uat.yml up

# Staging environment
docker-compose -f docker-compose.yml -f docker-compose.staging.yml up

# Production environment
docker-compose -f docker-compose.yml -f docker-compose.prod.yml up -d
```

### CI/CD Deployment

1. **UAT**: Push to `main` branch → Auto-deploys to UAT
2. **Staging**: Push to `staging` branch → Auto-deploys to Staging
3. **Production**: Create tag `release/v*` → Requires manual approval

## Docker Compose File Structure

```
docker-compose.yml          # Main entry point (imports base + env)
docker-compose.base.yml     # Base services (LDAP, Keycloak, PostgreSQL)
docker-compose.uat.yml      # UAT-specific overrides
docker-compose.staging.yml  # Staging-specific overrides
docker-compose.prod.yml     # Production-specific overrides
```

## Environment Variables

Copy `.env.uat`, `.env.staging`, or `.env.prod` to `.env` in each environment:

```bash
cp .env.uat .env
```

**Important**: Never commit `.env` files with real credentials to version control.

## Database Configuration

Each environment has its own PostgreSQL instance:

- **UAT**: `postgres-uat` (port 5432), `postgres-uat-audit` (port 5433)
- **Staging**: `postgres-staging` (port 5432), `postgres-staging-audit` (port 5433)
- **Production**: `postgres-prod` (port 5432), `postgres-prod-audit` (port 5433)

Database initialization scripts are in `infra/postgres/`:

- `init-uat.sql`
- `init-staging.sql`
- `init-prod.sql`

## GitHub Actions Workflows

| Workflow | Trigger | Action |
|----------|---------|--------|
| `ci.yml` | Push to main/staging, PRs | Build & Test |
| `deploy-uat.yml` | Push to main | Deploy to UAT |
| `deploy-staging.yml` | Push to staging | Deploy to Staging |
| `deploy-production.yml` | Tag release/v* | Build & Push (requires approval) |

## Required Secrets

Configure these in GitHub repository settings:

### Registry
- `REGISTRY_URL`: Container registry URL
- `REGISTRY_USER`: Registry username
- `REGISTRY_TOKEN`: Registry access token

### Deployment Users
- `UAT_DEPLOY_USER`: SSH user for UAT server
- `STAGING_DEPLOY_USER`: SSH user for Staging server

### Production (Manual Approval Required)
- `PROD_POSTGRES_USER`: Production database user
- `POSTGRES_PASSWORD`: Production database password
- `LDAP_ADMIN_PASSWORD`: Production LDAP password
- `KEYCLOAK_ADMIN`: Production Keycloak admin
- `KEYCLOAK_ADMIN_PASSWORD`: Production Keycloak password

## Branch Strategy

```
main        → UAT (auto-deploy)
staging     → Staging (auto-deploy)
release/v*  → Production (manual approval)
```

## Health Check Endpoints

After deployment, verify services are running:

- UAT: https://uat.it-platform.internal/health
- Staging: https://staging.it-platform.internal/health
- Production: https://it-platform.internal:8443/health
