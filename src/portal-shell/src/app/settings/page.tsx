"use client";
import AppLayout from '@/components/AppLayout';
import AuthGuard from '@/components/AuthGuard';
import Settings from '@/components/Settings';

export default function SettingsPage() {
  return (
    <AuthGuard>
      <AppLayout>
        <Settings />
      </AppLayout>
    </AuthGuard>
  );
}
