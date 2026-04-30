"use client";

import { useState } from 'react';
import { useAuth } from '@/contexts/AuthContext';
import { userService } from '@/services';
import styles from './UserManagement.module.css';

export default function UserManagement() {
  const { user } = useAuth();
  const [profile, setProfile] = useState<any>(null);
  const [loading, setLoading] = useState(false);
  const [editMode, setEditMode] = useState(false);
  const [formData, setFormData] = useState({
    department: '',
    jobTitle: '',
    address: '',
    city: '',
    country: '',
  });

  const loadProfile = async () => {
    if (!user) return;
    setLoading(true);
    try {
      const data = await userService.getProfile(user.id);
      setProfile(data);
      setFormData({
        department: data.department || '',
        jobTitle: data.jobTitle || '',
        address: data.address || '',
        city: data.city || '',
        country: data.country || '',
      });
    } catch (err) {
      console.error('Failed to load profile', err);
    } finally {
      setLoading(false);
    }
  };

  const handleSave = async () => {
    if (!user) return;
    try {
      await userService.updateProfile(user.id, formData);
      setEditMode(false);
      loadProfile();
    } catch (err) {
      console.error('Failed to update profile', err);
    }
  };

  return (
    <div className={styles.container}>
      <h1 className={styles.title}>User Management</h1>
      <div className={styles.card}>
        <div className={styles.cardHeader}>
          <h2>My Profile</h2>
          {!editMode && (
            <button className={styles.editBtn} onClick={() => { setEditMode(true); loadProfile(); }}>
              Edit Profile
            </button>
          )}
        </div>
        {loading && <p>Loading...</p>}
        {editMode ? (
          <div className={styles.form}>
            <div className={styles.field}>
              <label>Department</label>
              <input type="text" value={formData.department}
                onChange={(e) => setFormData({ ...formData, department: e.target.value })} />
            </div>
            <div className={styles.field}>
              <label>Job Title</label>
              <input type="text" value={formData.jobTitle}
                onChange={(e) => setFormData({ ...formData, jobTitle: e.target.value })} />
            </div>
            <div className={styles.field}>
              <label>Address</label>
              <input type="text" value={formData.address}
                onChange={(e) => setFormData({ ...formData, address: e.target.value })} />
            </div>
            <div className={styles.field}>
              <label>City</label>
              <input type="text" value={formData.city}
                onChange={(e) => setFormData({ ...formData, city: e.target.value })} />
            </div>
            <div className={styles.field}>
              <label>Country</label>
              <input type="text" value={formData.country}
                onChange={(e) => setFormData({ ...formData, country: e.target.value })} />
            </div>
            <div className={styles.actions}>
              <button className={styles.saveBtn} onClick={handleSave}>Save</button>
              <button className={styles.cancelBtn} onClick={() => setEditMode(false)}>Cancel</button>
            </div>
          </div>
        ) : (
          <div className={styles.profile}>
            <p><strong>Username:</strong> {user?.username}</p>
            <p><strong>Email:</strong> {user?.email}</p>
            <p><strong>Tenant:</strong> {user?.tenantName}</p>
            <p><strong>Roles:</strong> {user?.roles?.join(', ')}</p>
            {profile && (
              <>
                <p><strong>Department:</strong> {profile.department || 'N/A'}</p>
                <p><strong>Job Title:</strong> {profile.jobTitle || 'N/A'}</p>
                <p><strong>City:</strong> {profile.city || 'N/A'}</p>
              </>
            )}
          </div>
        )}
      </div>
    </div>
  );
}
