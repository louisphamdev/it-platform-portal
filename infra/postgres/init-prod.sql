#!/bin/bash
# PostgreSQL initialization script for Production environment
# This script runs on first startup of postgres-prod container

set -e

psql -v ON_ERROR_STOP=1 --username "$POSTGRES_USER" --dbname "$POSTGRES_DB" <<-EOSQL
    -- Create application user
    CREATE USER app_user WITH PASSWORD 'app_user_pass';
    
    -- Grant privileges
    GRANT ALL PRIVILEGES ON DATABASE "$POSTGRES_DB" TO app_user;
    GRANT ALL PRIVILEGES ON DATABASE "itaudit_prod" TO app_user;
    
    -- Create extensions
    CREATE EXTENSION IF NOT EXISTS "uuid-ossp";
    CREATE EXTENSION IF NOT EXISTS "pg_trgm";
    
    -- Create audit table
    CREATE TABLE IF NOT EXISTS audit_log (
        id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
        action VARCHAR(100) NOT NULL,
        table_name VARCHAR(100),
        record_id VARCHAR(100),
        old_data JSONB,
        new_data JSONB,
        performed_by VARCHAR(100),
        performed_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
    );
    
    -- Create indexes for audit
    CREATE INDEX IF NOT EXISTS idx_audit_performed_at ON audit_log(performed_at);
    CREATE INDEX IF NOT EXISTS idx_audit_action ON audit_log(action);
    
    -- Production-specific: Enable row-level security
    ALTER TABLE audit_log ENABLE ROW LEVEL SECURITY;
EOSQL

echo "Production database initialized successfully"
