'use client';

import { useAuth } from '@/contexts/AuthContext';
import styles from './Dashboard.module.css';

export default function Dashboard() {
  const { user, logout } = useAuth();

  return (
    <div className={styles.layout}>
      <header className={styles.header}>
        <div className={styles.logo}>IT Platform Portal</div>
        <div className={styles.userMenu}>
          <span className={styles.userName}>{user?.username}</span>
          <span className={styles.userRole}>{user?.roles?.[0] || 'User'}</span>
          <button onClick={logout} className={styles.logoutBtn}>Logout</button>
        </div>
      </header>

      <div className={styles.container}>
        <aside className={styles.sidebar}>
          <nav className={styles.nav}>
            <a href="/dashboard" className={styles.navItem}>Dashboard</a>
            <a href="/users" className={styles.navItem}>User Management</a>
            <a href="/tenants" className={styles.navItem}>Tenant Management</a>
            <a href="/audit" className={styles.navItem}>Audit Logs</a>
            <a href="/permissions" className={styles.navItem}>Permissions</a>
            <a href="/settings" className={styles.navItem}>Settings</a>
          </nav>
        </aside>

        <main className={styles.main}>
          <h1 className={styles.welcome}>Welcome, {user?.username}!</h1>
          <p className={styles.subtitle}>Tenant: {user?.tenantName}</p>

          <div className={styles.grid}>
            <div className={styles.card}>
              <h3>Profile</h3>
              <p>Manage your account settings</p>
            </div>
            <div className={styles.card}>
              <h3>Security</h3>
              <p>Change password and 2FA</p>
            </div>
            <div className={styles.card}>
              <h3>Notifications</h3>
              <p>Configure notification preferences</p>
            </div>
            <div className={styles.card}>
              <h3>Activity</h3>
              <p>View your recent activity</p>
            </div>
          </div>
        </main>
      </div>
    </div>
  );
}
