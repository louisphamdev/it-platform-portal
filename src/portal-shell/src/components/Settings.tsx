"use client";

import { useState } from 'react';
import { useAuth } from '@/contexts/AuthContext';
import { userService } from '@/services';
import styles from './Settings.module.css';

export default function Settings() {
  const { user, logout } = useAuth();
  const [passwordData, setPasswordData] = useState({
    currentPassword: '',
    newPassword: '',
    confirmPassword: '',
  });
  const [message, setMessage] = useState('');
  const [error, setError] = useState('');
  const [loading, setLoading] = useState(false);

  const handleChangePassword = async () => {
    if (passwordData.newPassword !== passwordData.confirmPassword) {
      setError('Passwords do not match'); return;
    }
    if (passwordData.newPassword.length < 8) {
      setError('Password must be at least 8 characters'); return;
    }
    setLoading(true); setError(''); setMessage('');
    try {
      await userService.changePassword(user!.id, {
        currentPassword: passwordData.currentPassword,
        newPassword: passwordData.newPassword,
      });
      setMessage('Password changed successfully');
      setPasswordData({ currentPassword: '', newPassword: '', confirmPassword: '' });
    } catch (err: any) {
      setError(err.response?.data?.message || 'Failed to change password');
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className={styles.container}>
      <h1 className={styles.title}>Settings</h1>
      <div className={styles.section}>
        <h2>Account Information</h2>
        <div className={styles.info}>
          <p><strong>Username:</strong> {user?.username}</p>
          <p><strong>Email:</strong> {user?.email}</p>
          <p><strong>Tenant:</strong> {user?.tenantName}</p>
          <p><strong>Roles:</strong> {user?.roles?.join(', ')}</p>
        </div>
      </div>
      <div className={styles.section}>
        <h2>Change Password</h2>
        {message && <div className={styles.success}>{message}</div>}
        {error && <div className={styles.error}>{error}</div>}
        <div className={styles.form}>
          <div className={styles.field}>
            <label>Current Password</label>
            <input type="password" value={passwordData.currentPassword}
              onChange={(e) => setPasswordData({ ...passwordData, currentPassword: e.target.value })} />
          </div>
          <div className={styles.field}>
            <label>New Password</label>
            <input type="password" value={passwordData.newPassword}
              onChange={(e) => setPasswordData({ ...passwordData, newPassword: e.target.value })} />
          </div>
          <div className={styles.field}>
            <label>Confirm New Password</label>
            <input type="password" value={passwordData.confirmPassword}
              onChange={(e) => setPasswordData({ ...passwordData, confirmPassword: e.target.value })} />
          </div>
          <button className={styles.saveBtn} onClick={handleChangePassword} disabled={loading}>
            {loading ? 'Changing...' : 'Change Password'}
          </button>
        </div>
      </div>
      <div className={styles.section}>
        <h2>Security</h2>
        <button className={styles.dangerBtn} onClick={logout}>Sign Out</button>
      </div>
    </div>
  );
}
