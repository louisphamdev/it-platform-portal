"use client";
import AppLayout from '@/components/AppLayout';
import AuthGuard from '@/components/AuthGuard';
import AuditLogs from '@/components/AuditLogs';

export default function AuditPage() {
  return (
    <AuthGuard>
      <AppLayout>
        <AuditLogs />
      </AppLayout>
    </AuthGuard>
  );
}
