# IT Platform Portal

Enterprise-grade multi-tenant IT platform with identity management, authentication, and authorization.

## 🏗️ Architecture

```
┌─────────────────────────────────────────────────────────────────────────────┐
│                              CLIENT LAYER                                    │
│                           (Browser / Mobile)                                 │
└─────────────────────────────────────────────────────────────────────────────┘
                                      │
                                      ▼
┌─────────────────────────────────────────────────────────────────────────────┐
│                          PORTAL SHELL (Next.js)                             │
│                        http://localhost:3000                                 │
│                    Frontend SPA - React + TypeScript                        │
└─────────────────────────────────────────────────────────────────────────────┘
                                      │
                                      ▼
┌─────────────────────────────────────────────────────────────────────────────┐
│                         API GATEWAY (YARP)                                   │
│                          http://localhost:7000                              │
│                   Reverse Proxy + JWT Validation                            │
│                                                                             │
│   ┌──────────┐   ┌───────────┐   ┌──────────────┐                          │
│   │ /auth/* │──▶│  bff-auth │   │    /api/*    │                          │
│   └──────────┘   └───────────┘   └──────────────┘                          │
│                                              │                              │
│                                              ▼                              │
│                                    ┌─────────────────┐                      │
│                                    │   bff-portal    │                      │
│                                    └─────────────────┘                      │
└─────────────────────────────────────────────────────────────────────────────┘
                                      │
         ┌────────────────────────────┼────────────────────────────────┐
         │                            │                                │
         ▼                            ▼                                ▼
┌─────────────────┐         ┌─────────────────┐              ┌─────────────────┐
│  Keycloak       │         │  OpenLDAP       │              │  PostgreSQL     │
│  (Identity)     │         │  (Directory)   │              │  (Databases)    │
│  :8080          │         │  :389           │              │  :5432, :5433   │
└─────────────────┘         └─────────────────┘              └─────────────────┘
```

## 📋 Prerequisites

- **Docker** 20.10+ with Docker Compose
- **.NET SDK** 8.0+ (for local development)
- **Node.js** 18+ with npm (for portal-shell development)
- **Git** 2.30+

## 🚀 Quick Start

### 1. Clone and Setup

```bash
git clone https://github.com/louisphamdev/it-platform-portal.git
cd it-platform-portal
```

### 2. Configure Environment

Create a `.env.local` file for local development:

```bash
# Database
POSTGRES_USER=postgres
POSTGRES_PASSWORD=postgres123
POSTGRES_DB=itplatform
POSTGRES_AUDIT_DB=itaudit

# Keycloak
KEYCLOAK_ADMIN=admin
KEYCLOAK_ADMIN_PASSWORD=admin123
KEYCLOAK_PORT=8080

# LDAP
LDAP_ADMIN_PASSWORD=admin123
LDAP_PORT=389

# JWT
JWT_SECRET=YourSuperSecretKeyThatIsAtLeast32CharsLong!

# Service Ports
API_GATEWAY_PORT=7000
BFF_AUTH_PORT=7001
BFF_PORTAL_PORT=7002
PORTAL_SHELL_PORT=3000
```

### 3. Start All Services

```bash
docker-compose up -d
```

Wait for all services to be healthy (check with `docker-compose ps`).

### 4. Verify Services

| Service | URL | Description |
|---------|-----|-------------|
| Portal Shell | http://localhost:3000 | Frontend application |
| API Gateway | http://localhost:7000 | Reverse proxy |
| Keycloak | http://localhost:8080 | Identity provider |
| BFF Auth | http://localhost:7001 | Authentication service |
| BFF Portal | http://localhost:7002 | Main API service |
| OpenLDAP | ldap://localhost:389 | Directory service |
| PostgreSQL | localhost:5432 | Main database |
| PostgreSQL Audit | localhost:5433 | Audit database |

### 5. Default Credentials

**Keycloak:**
- URL: http://localhost:8080
- Admin: `admin` / `admin123`

**LDAP:**
- Base DN: `dc=itplatform,dc=internal`
- Admin: `cn=admin,dc=itplatform,dc=internal` / `admin123`

**Seed Users:**
| Username | Password | Role |
|----------|----------|------|
| admin | admin123 | Administrator |
| auditor | auditor123 | Auditor |
| john.doe | johndoe123 | User (Alpha Tenant) |
| jane.smith | janesmith123 | User (Beta Tenant) |

## 🔧 Service Details

### Services

| Service | Port | Description | Tech Stack |
|---------|------|-------------|------------|
| portal-shell | 3000 | Frontend SPA | Next.js 14 |
| api-gateway | 7000 | Reverse proxy & gateway | ASP.NET Core + YARP |
| bff-auth | 7001 | Authentication BFF | ASP.NET Core |
| bff-portal | 7002 | Main API BFF | ASP.NET Core |
| keycloak | 8080 | Identity Provider | Keycloak 24 |
| openldap | 389 | Directory Service | OpenLDAP 1.5.0 |
| postgres-main | 5432 | Main database | PostgreSQL 16 |
| postgres-audit | 5433 | Audit database | PostgreSQL 16 |

### Database Schema

The main database (`postgres-main`) includes these core tables:

- `tenants` - Multi-tenant support
- `users` - User accounts synchronized from Keycloak/LDAP
- `roles` - Role definitions (system and tenant-scoped)
- `permissions` - Fine-grained permission definitions
- `user_roles` - User-role assignments
- `role_permissions` - Role-permission mappings
- `audit_log` - Comprehensive audit trail
- `api_keys` - Service-to-service authentication
- `sessions` - User session management

## 📁 Project Structure

```
it-platform-portal/
├── docker-compose.yml          # Main compose file
├── docker-compose.base.yml    # Base services (dev/staging/uat)
├── docker-compose.prod.yml    # Production overrides
├── docker-compose.staging.yml  # Staging overrides
├── docker-compose.uat.yml     # UAT overrides
├── .env.local                 # Local environment (gitignored)
├── .env.staging              # Staging environment
├── .env.uat                  # UAT environment
├── .env.prod                 # Production environment
├── infra/
│   ├── keycloak/
│   │   └── it-platform-realm.json  # Keycloak realm config
│   ├── ldap/
│   │   └── ldap-seed.ldif         # LDAP seed data
│   └── postgres/
│       └── init-*.sql             # DB init scripts
├── scripts/
│   └── init-db.sql                # Main database schema
└── src/
    ├── ItPlatformPortal.sln      # .NET Solution
    ├── modules/                   # Core domain modules
    │   ├── Auth/
    │   ├── User/
    │   ├── Tenant/
    │   ├── Permission/
    │   └── Audit/
    ├── services/                  # BFF and gateway services
    │   ├── api-gateway/           # YARP reverse proxy
    │   ├── bff-auth/             # Auth BFF
    │   └── bff-portal/           # Portal BFF
    └── portal-shell/             # Next.js frontend
        ├── src/
        │   ├── app/             # Next.js App Router pages
        │   ├── components/       # React components
        │   ├── lib/             # Utilities
        │   └── styles/          # Global styles
        ├── Dockerfile
        ├── package.json
        └── tsconfig.json
```

## 🔐 Authentication Flow

```
1. User opens portal → redirected to Keycloak login
2. Keycloak authenticates user (credentials checked)
3. Keycloak issues JWT access token + refresh token
4. Frontend stores tokens (httpOnly cookies preferred)
5. Frontend includes JWT in Authorization header
6. API Gateway validates JWT signature and claims
7. BFF services receive validated principal
8. Audit trail created for each action
```

## 🛠️ Local Development

### Running Services Individually

```bash
# Start only infrastructure (DB, LDAP, Keycloak)
docker-compose up -d postgres-main postgres-audit openldap keycloak

# Run API Gateway locally
cd src/services/api-gateway
dotnet run

# Run BFF services locally
cd src/services/bff-auth
dotnet run

# Run Portal Shell locally
cd src/portal-shell
npm run dev
```

### Running Tests

```bash
# .NET tests
dotnet test src/ItPlatformPortal.sln

# Frontend tests
cd src/portal-shell
npm test
```

## 🔄 Environment-Specific Deployment

```bash
# Staging
docker-compose -f docker-compose.yml -f docker-compose.staging.yml up -d

# UAT
docker-compose -f docker-compose.yml -f docker-compose.uat.yml up -d

# Production (requires secrets)
docker-compose -f docker-compose.yml -f docker-compose.prod.yml up -d
```

## 🔍 Troubleshooting

### Services Won't Start

```bash
# Check service status
docker-compose ps

# View service logs
docker-compose logs [service-name]

# Restart a specific service
docker-compose restart [service-name]
```

### Database Connection Issues

```bash
# Check if PostgreSQL is ready
docker-compose exec postgres-main pg_isready

# Connect to database
docker-compose exec postgres-main psql -U postgres -d itplatform
```

### LDAP Connection Issues

```bash
# Test LDAP connectivity
docker-compose exec openldap ldapsearch -x -H ldap://localhost:389 -b "dc=itplatform,dc=internal"
```

### Keycloak Issues

```bash
# Check Keycloak logs
docker-compose logs keycloak

# Access Keycloak admin console
# http://localhost:8080/auth/admin/
```

### Rebuilding Services

```bash
# Rebuild and restart
docker-compose build [service-name]
docker-compose up -d [service-name]
```

## 📚 API Documentation

### Authentication Endpoints

| Method | Path | Description |
|--------|------|-------------|
| POST | /auth/login | Authenticate user |
| POST | /auth/logout | Logout user |
| POST | /auth/refresh | Refresh access token |
| GET | /auth/me | Get current user info |

### Portal API Endpoints

| Method | Path | Description |
|--------|------|-------------|
| GET | /api/users | List users |
| GET | /api/users/{id} | Get user by ID |
| POST | /api/users | Create user |
| PUT | /api/users/{id} | Update user |
| DELETE | /api/users/{id} | Delete user |
| GET | /api/tenants | List tenants |
| GET | /api/roles | List roles |
| GET | /api/audit/logs | Query audit logs |

## 🔒 Security Notes

- Change all default passwords in production
- Use strong JWT secrets (min 32 characters)
- Configure HTTPS/TLS for all services
- Enable audit logging for compliance
- Regular security updates for containers

## 📄 License

Internal use only - IT Platform Corp

## 🚀 Production Deployment

### Prerequisites
- Docker 20.10+ with Docker Compose
- GHCR container registry access
- Domain: `it-platform.internal`

### Environment Setup
```bash
# Copy environment template
cp .env.example .env

# Edit with production values
vim .env
```

### Deploy to Production
```bash
# Pull latest image
docker pull ghcr.io/louisphamdev/it-platform-portal:prod-latest

# Start services
docker-compose -f docker-compose.yml -f docker-compose.prod.yml up -d

# Verify health
curl http://localhost:7000/health
curl http://localhost:3000/health
```

### Production Services
| Service | Port | URL |
|---------|------|-----|
| Portal Shell | 3000 | http://it-platform.internal:3000 |
| API Gateway | 7000 | http://it-platform.internal:7000 |
| BFF Auth | 7001 | http://it-platform.internal:7001 |
| BFF Portal | 7002 | http://it-platform.internal:7002 |
| Keycloak | 8443 | https://it-platform.internal:8443 |
| OpenLDAP | 389 | ldap://it-platform.internal:389 |
| PostgreSQL | 5432 | postgres://it-platform.internal:5432 |

### Release Process
1. Merge to `staging` → auto-deploy staging
2. Test staging environment
3. Create tag `release/v*` → triggers production deploy
4. Verify production health checks pass

### Backup
```bash
# Database backup
docker exec postgres-prod pg_dump -U postgres itportal_prod > backup_$(date +%Y%m%d).sql

# Volume backup
docker run --rm -v postgres-prod-data:/data -v $(pwd):/backup alpine tar czf /backup/postgres-backup.tar.gz -C /data .
```
