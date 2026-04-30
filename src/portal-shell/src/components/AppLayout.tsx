'use client';

import { useState } from 'react';
import styles from './AppLayout.module.css';

interface AppLayoutProps {
  children: React.ReactNode;
}

export default function AppLayout({ children }: AppLayoutProps) {
  const [sidebarCollapsed, setSidebarCollapsed] = useState(false);

  return (
    <div className={styles.layout}>
      <header className={styles.header}>
        <button 
          className={styles.menuBtn}
          onClick={() => setSidebarCollapsed(!sidebarCollapsed)}
        >
          ☰
        </button>
        <div className={styles.logo}>IT Platform Portal</div>
        <div className={styles.headerRight}>
          <span className={styles.badge}>v1.0.0</span>
        </div>
      </header>

      <div className={styles.body}>
        <aside className={`${styles.sidebar} ${sidebarCollapsed ? styles.collapsed : ''}`}>
          <nav className={styles.nav}>
            <a href="/dashboard" className={styles.navItem}>📊 Dashboard</a>
            <a href="/users" className={styles.navItem}>👥 Users</a>
            <a href="/tenants" className={styles.navItem}>🏢 Tenants</a>
            <a href="/audit" className={styles.navItem}>📋 Audit Logs</a>
            <a href="/permissions" className={styles.navItem}>🔐 Permissions</a>
            <a href="/settings" className={styles.navItem}>⚙️ Settings</a>
          </nav>
        </aside>

        <main className={styles.main}>
          {children}
        </main>
      </div>
    </div>
  );
}
