"use client";
import AppLayout from '@/components/AppLayout';
import AuthGuard from '@/components/AuthGuard';
import UserManagement from '@/components/UserManagement';

export default function UsersPage() {
  return (
    <AuthGuard>
      <AppLayout>
        <UserManagement />
      </AppLayout>
    </AuthGuard>
  );
}
