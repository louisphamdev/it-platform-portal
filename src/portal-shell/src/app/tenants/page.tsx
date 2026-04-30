"use client";
import AppLayout from '@/components/AppLayout';
import AuthGuard from '@/components/AuthGuard';
import TenantManagement from '@/components/TenantManagement';

export default function TenantsPage() {
  return (
    <AuthGuard>
      <AppLayout>
        <TenantManagement />
      </AppLayout>
    </AuthGuard>
  );
}
