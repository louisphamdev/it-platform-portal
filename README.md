# IT Internal Platform Portal

A modern internal platform portal for enterprise IT organizations, built with a modular architecture combining React micro-frontends and a .NET 10 modular monolith.

## Overview

This project provides a unified internal platform for IT service management, featuring authentication via OpenLDAP and Keycloak, multi-tenant support, comprehensive audit logging, and fine-grained permission management.

## Tech Stack

### Frontend
- **React 18** - UI library with hooks and concurrent features
- **Next.js 14** - React framework with App Router
- **Module Federation** - Micro-frontend architecture for distributed deployment

### Backend
- **.NET 10** - Modular monolith architecture
- **YARP** - Yet Another Reverse Proxy for API Gateway
- **BFF Pattern** - Backend for Frontend architecture

### Authentication & Authorization
- **OpenLDAP** - Directory service for user management
- **Keycloak 24** - Identity provider and OAuth2/OIDC server

### Database
- **PostgreSQL 16** - Primary data store
  - `postgres-main:5432` - Main application database
  - `postgres-audit:5433` - Audit and logging database

## Repository Structure

```
it-platform-portal/
├── .github/workflows/           # CI/CD workflows
├── src/
│   ├── portal-shell/            # React host application
│   ├── api-gateway/             # YARP reverse proxy
│   ├── bff-auth/                # Authentication BFF
│   ├── bff-portal/               # Portal BFF
│   └── modules/                 # .NET domain modules
│       ├── Auth/                # Authentication module
│       ├── User/                # User management module
│       ├── Tenant/              # Multi-tenancy module
│       ├── Audit/               # Audit logging module
│       └── Permission/           # Permission management module
├── infra/
│   ├── ldap/                    # OpenLDAP configuration
│   ├── keycloak/                # Keycloak configuration
│   └── postgres/                # PostgreSQL migrations
├── docker-compose.yml
└── README.md
```

## Getting Started

### Prerequisites
- Docker and Docker Compose
- .NET 10 SDK
- Node.js 20+
- Git

### Quick Start

1. **Clone the repository**
   ```bash
   git clone <repository-url>
   cd it-platform-portal
   ```

2. **Start infrastructure services**
   ```bash
   docker-compose up -d
   ```

3. **Access services**
   - Portal: http://localhost:3000
   - Keycloak Admin: http://localhost:8080 (admin/admin)
   - LDAP: localhost:389

## Services

| Service | Port | Description |
|---------|------|-------------|
| portal-shell | 3000 | React host application |
| api-gateway | 5000 | YARP API gateway |
| bff-auth | 5001 | Authentication BFF |
| bff-portal | 5002 | Portal BFF |
| keycloak | 8080 | Identity provider |
| openldap | 389 | Directory service |
| postgres-main | 5432 | Main database |
| postgres-audit | 5433 | Audit database |

## Modules

### Auth Module
Handles authentication flows, session management, and LDAP integration.

### User Module
Manages user profiles, preferences, and user-related operations.

### Tenant Module
Provides multi-tenancy support with tenant isolation.

### Audit Module
Comprehensive audit logging for all platform operations.

### Permission Module
Role-based access control (RBAC) and permission management.

## License

Internal use only - IT Platform Corp
