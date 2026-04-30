"use client";
import AppLayout from '@/components/AppLayout';
import AuthGuard from '@/components/AuthGuard';
import Permissions from '@/components/Permissions';

export default function PermissionsPage() {
  return (
    <AuthGuard>
      <AppLayout>
        <Permissions />
      </AppLayout>
    </AuthGuard>
  );
}
