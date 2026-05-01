# IT Platform Portal - API Documentation

## Overview

The IT Platform Portal uses a BFF (Backend for Frontend) architecture with YARP reverse proxy as the API Gateway.

```
Client → Portal Shell (Next.js) → API Gateway (YARP) → BFF Services → Backend Services
```

## Gateway Routing

| Route Pattern | Destination | Service | Port |
|---------------|-------------|---------|------|
| `/auth/*` | BFF Auth | Authentication BFF | 7001 |
| `/api/*` | BFF Portal | Main API BFF | 7002 |
| `/*` | Portal Shell | Next.js Frontend | 3000 |

## Base URLs

| Environment | URL |
|-------------|-----|
| Local | `http://localhost:7000` |
| Staging | `https://staging-api.itplatform.example.com` |
| Production | `https://api.itplatform.example.com` |

---

## Authentication

All API endpoints (except health checks and login) require JWT Bearer token authentication.

### Request Headers

```
Authorization: Bearer <access_token>
Content-Type: application/json
```

### Response Headers

| Header | Description |
|--------|-------------|
| `X-Gateway` | API Gateway identifier |
| `X-BFF` | BFF service identifier |

---

## Health Endpoints

### GET /health

Basic health check for all services.

**Response (200 OK):**
```json
{
  "status": "healthy",
  "service": "api-gateway",
  "timestamp": "2024-01-15T10:30:00Z",
  "version": "1.0.0"
}
```

### GET /health/live

Kubernetes liveness probe.

**Response (200 OK):**
```json
{
  "status": "alive"
}
```

### GET /health/ready

Kubernetes readiness probe. Checks all backend service dependencies.

**Response (200 OK):**
```json
{
  "status": "ready",
  "service": "api-gateway",
  "checks": {
    "database": "healthy",
    "keycloak": "healthy"
  },
  "timestamp": "2024-01-15T10:30:00Z"
}
```

**Response (503 Service Unavailable):**
```json
{
  "status": "not_ready",
  "service": "api-gateway",
  "checks": {
    "database": "healthy",
    "keycloak": "unhealthy: Connection refused"
  },
  "timestamp": "2024-01-15T10:30:00Z"
}
```

---

## BFF Auth Endpoints

Base path: `/auth`

**Note:** Auth endpoints are routed through `/auth/*` on the API Gateway.

### Internal Health (BFF Auth)

| Method | Path | Description |
|--------|------|-------------|
| GET | `/health` | Health check |
| GET | `/health/live` | Liveness probe |
| GET | `/health/ready` | Readiness probe |

---

## BFF Portal API Endpoints

Base path: `/api`

**Note:** All endpoints require JWT authentication.

### Users API

| Method | Path | Description |
|--------|------|-------------|
| GET | `/api/users/{userId}/profile` | Get user profile |
| PUT | `/api/users/{userId}/profile` | Update user profile |
| POST | `/api/users/{userId}/change-password` | Change user password |
| POST | `/api/users/{userId}/sessions` | Create new session |
| DELETE | `/api/users/sessions/{refreshToken}` | Revoke specific session |
| DELETE | `/api/users/{userId}/sessions` | Revoke all user sessions |

#### GET /api/users/{userId}/profile

**Response (200 OK):**
```json
{
  "id": "550e8400-e29b-41d4-a716-446655440000",
  "username": "john.doe",
  "email": "john.doe@example.com",
  "firstName": "John",
  "lastName": "Doe",
  "tenantId": "123e4567-e89b-12d3-a456-426614174000",
  "tenantName": "Alpha Corp",
  "roles": ["admin", "user"],
  "createdAt": "2024-01-01T00:00:00Z",
  "lastLogin": "2024-01-15T10:30:00Z"
}
```

#### PUT /api/users/{userId}/profile

**Request Body:**
```json
{
  "firstName": "John",
  "lastName": "Smith",
  "email": "john.smith@example.com"
}
```

#### POST /api/users/{userId}/change-password

**Request Body:**
```json
{
  "currentPassword": "oldPassword123",
  "newPassword": "newSecurePassword123"
}
```

**Response (200 OK):**
```json
{
  "message": "Password changed successfully"
}
```

---

### Tenants API

Base path: `/api/tenants`

| Method | Path | Description |
|--------|------|-------------|
| GET | `/api/tenants` | List all tenants |
| GET | `/api/tenants/{id}` | Get tenant by ID |
| GET | `/api/tenants/code/{code}` | Get tenant by code |
| POST | `/api/tenants` | Create new tenant |
| PUT | `/api/tenants/{id}` | Update tenant |
| POST | `/api/tenants/{id}/suspend` | Suspend tenant |
| POST | `/api/tenants/{id}/activate` | Activate tenant |
| GET | `/api/tenants/{id}/user-count` | Get tenant user count |

#### GET /api/tenants

**Response (200 OK):**
```json
[
  {
    "id": "123e4567-e89b-12d3-a456-426614174000",
    "code": "ALPHA",
    "name": "Alpha Corporation",
    "displayName": "Alpha Corp",
    "status": "active",
    "createdAt": "2024-01-01T00:00:00Z",
    "userCount": 150
  }
]
```

#### POST /api/tenants

**Request Body:**
```json
{
  "code": "BETA",
  "name": "Beta Corporation",
  "displayName": "Beta Corp",
  "contactEmail": "admin@beta.example.com",
  "contactPhone": "+1-555-0123"
}
```

---

### Permissions API

Base path: `/api/permissions`

| Method | Path | Description |
|--------|------|-------------|
| GET | `/api/permissions` | List all permissions |
| GET | `/api/permissions/role/{roleId}` | Get permissions by role |
| GET | `/api/permissions/user/{userId}` | Get user's permission codes |
| POST | `/api/permissions` | Create new permission |
| POST | `/api/permissions/assign` | Assign permissions to role |
| DELETE | `/api/permissions/role/{roleId}/permission/{permissionId}` | Remove permission from role |

#### GET /api/permissions

**Response (200 OK):**
```json
[
  {
    "id": "550e8400-e29b-41d4-a716-446655440001",
    "code": "user.read",
    "name": "Read Users",
    "description": "Permission to read user data",
    "module": "User",
    "createdAt": "2024-01-01T00:00:00Z"
  }
]
```

#### POST /api/permissions/assign

**Request Body:**
```json
{
  "roleId": "123e4567-e89b-12d3-a456-426614174000",
  "permissionIds": [
    "550e8400-e29b-41d4-a716-446655440001",
    "550e8400-e29b-41d4-a716-446655440002"
  ]
}
```

---

### Audit API

Base path: `/api/audit`

| Method | Path | Description |
|--------|------|-------------|
| POST | `/api/audit` | Create audit log entry |
| GET | `/api/audit` | Query audit logs |
| GET | `/api/audit/{id}` | Get audit log by ID |
| GET | `/api/audit/count` | Get total count of audit logs |

#### POST /api/audit

**Request Body:**
```json
{
  "action": "user.login",
  "userId": "550e8400-e29b-41d4-a716-446655440000",
  "tenantId": "123e4567-e89b-12d3-a456-426614174000",
  "resourceType": "Session",
  "resourceId": "session-123",
  "ipAddress": "192.168.1.100",
  "userAgent": "Mozilla/5.0...",
  "details": {
    "browser": "Chrome 120",
    "os": "Windows 11"
  }
}
```

**Response (202 Accepted)**

#### GET /api/audit

**Query Parameters:**
| Parameter | Type | Description |
|-----------|------|-------------|
| userId | GUID | Filter by user |
| tenantId | GUID | Filter by tenant |
| action | string | Filter by action |
| startDate | ISO 8601 | Start date filter |
| endDate | ISO 8601 | End date filter |
| page | int | Page number (default: 1) |
| pageSize | int | Page size (default: 20, max: 100) |

**Response (200 OK):**
```json
{
  "items": [
    {
      "id": "660e8400-e29b-41d4-a716-446655440002",
      "action": "user.login",
      "userId": "550e8400-e29b-41d4-a716-446655440000",
      "tenantId": "123e4567-e89b-12d3-a456-426614174000",
      "timestamp": "2024-01-15T10:30:00Z",
      "ipAddress": "192.168.1.100"
    }
  ],
  "totalCount": 150,
  "page": 1,
  "pageSize": 20
}
```

---

## Error Responses

### Standard Error Format

```json
{
  "message": "Error description",
  "code": "ERROR_CODE",
  "details": {}
}
```

### HTTP Status Codes

| Code | Description |
|------|-------------|
| 200 | Success |
| 201 | Created |
| 202 | Accepted |
| 400 | Bad Request - Invalid input |
| 401 | Unauthorized - Invalid or missing token |
| 403 | Forbidden - Insufficient permissions |
| 404 | Not Found |
| 409 | Conflict - Resource already exists |
| 500 | Internal Server Error |
| 503 | Service Unavailable |

### Common Error Codes

| Code | Description |
|------|-------------|
| `AUTH_INVALID_CREDENTIALS` | Invalid username or password |
| `AUTH_TOKEN_EXPIRED` | JWT token has expired |
| `AUTH_TOKEN_INVALID` | JWT token is invalid |
| `USER_NOT_FOUND` | User does not exist |
| `TENANT_NOT_FOUND` | Tenant does not exist |
| `PERMISSION_DENIED` | User lacks required permission |
| `VALIDATION_ERROR` | Request validation failed |

---

## Rate Limiting

Production API is rate-limited to prevent abuse:

| Endpoint | Limit |
|----------|-------|
| `/auth/*` | 20 requests/minute per IP |
| `/api/*` | 100 requests/minute per user |
| `/health/*` | No limit |

---

## Pagination

List endpoints support pagination:

```
GET /api/tenants?page=1&pageSize=20
```

**Pagination Response:**
```json
{
  "items": [...],
  "totalCount": 150,
  "page": 1,
  "pageSize": 20,
  "totalPages": 8
}
```

---

## Environment Variables

### Required for Production

| Variable | Description |
|----------|-------------|
| `POSTGRES_USER` | PostgreSQL username |
| `POSTGRES_PASSWORD` | PostgreSQL password |
| `POSTGRES_DB` | Main database name |
| `POSTGRES_AUDIT_DB` | Audit database name |
| `KEYCLOAK_ADMIN` | Keycloak admin username |
| `KEYCLOAK_ADMIN_PASSWORD` | Keycloak admin password |
| `LDAP_ADMIN_PASSWORD` | LDAP admin password |
| `JWT_SECRET` | JWT signing secret (min 32 chars) |
| `BFF_AUTH_CLIENT_SECRET` | BFF Auth Keycloak client secret |

### Service URLs

| Variable | Description |
|----------|-------------|
| `NEXT_PUBLIC_API_GATEWAY_URL` | Public API Gateway URL |
| `NEXT_PUBLIC_KEYCLOAK_URL` | Public Keycloak URL |
| `NEXT_PUBLIC_KEYCLOAK_REALM` | Keycloak realm name |
| `NEXT_PUBLIC_KEYCLOAK_CLIENT_ID` | Keycloak client ID |

---

## Keycloak Integration

The portal integrates with Keycloak for identity management:

- **Realm:** `it-platform`
- **Client ID:** `portal-shell`
- **Grant Type:** Authorization Code Flow
- **Token Endpoint:** `http://keycloak:8080/realms/it-platform/protocol/openid-connect/token`

### Keycloak Roles

| Role | Description |
|------|-------------|
| `platform-admin` | Full platform administration |
| `tenant-admin` | Tenant-level administration |
| `auditor` | Audit log access |
| `user` | Standard user access |
