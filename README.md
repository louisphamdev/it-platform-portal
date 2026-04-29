# IT Internal Platform Portal

Enterprise internal portal for integrating platform modules with shared authentication & authorization.

## 🏗️ Architecture

```
┌─────────────────────────────────────────┐
│           Portal Shell (React)          │
│         Webpack Module Federation        │
└──────────────────┬──────────────────────┘
                   │
┌──────────────────▼──────────────────────┐
│        API Gateway (YARP) + BFF         │
│      JWT Auth + Permission Engine       │
└──────────────────┬──────────────────────┘
                   │
┌──────────────────▼──────────────────────┐
│    Modular Monolith (.NET 10)           │
│  Auth │ User │ Tenant │ Audit │ Perms    │
└──────────────────┬──────────────────────┘
                   │
┌──────────────────▼──────────────────────┐
│  PostgreSQL + OpenLDAP + Keycloak        │
└─────────────────────────────────────────┘
```

## 🚀 Quick Start

```bash
# Clone the repo
git clone https://github.com/louisphamdev/it-platform-portal.git
cd it-platform-portal

# Start all services
docker compose up -d

# Check status
docker compose ps
```

## 📁 Project Structure

```
it-platform-portal/
├── docker-compose.yml      # All services
├── src/
│   ├── portal-shell/       # React host app
│   ├── api-gateway/        # YARP proxy
│   ├── bff-auth/           # Auth BFF
│   ├── bff-portal/         # Portal BFF
│   └── modules/            # .NET modules
├── infra/                   # Infrastructure configs
│   ├── ldap/
│   ├── keycloak/
│   └── postgres/
└── tests/
```

## 🔐 Security

- Zero Trust security model
- JWT authentication via Keycloak
- RBAC + ABAC permission engine
- Audit logging on every request
- SAST/DAST in CI/CD pipeline

## 🌏 Environments

| Env | URL | Branch | Deploy |
|-----|-----|--------|--------|
| UAT | uat.it-platform.internal | main | Auto |
| Staging | staging.it-platform.internal | staging | Auto |
| Production | it-platform.internal | release/v* | Manual |

## 👥 License

Internal use only — Louis Phạm