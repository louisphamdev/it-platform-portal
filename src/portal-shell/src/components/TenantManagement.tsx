"use client";

import { useState, useEffect } from 'react';
import { tenantService, Tenant } from '@/services';
import styles from './TenantManagement.module.css';

export default function TenantManagement() {
  const [tenants, setTenants] = useState<Tenant[]>([]);
  const [loading, setLoading] = useState(true);
  const [showCreate, setShowCreate] = useState(false);
  const [formData, setFormData] = useState({ name: '', code: '' });

  const loadTenants = async () => {
    setLoading(true);
    try {
      const data = await tenantService.getAll();
      setTenants(data);
    } catch (err) {
      console.error('Failed to load tenants', err);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => { loadTenants(); }, []);

  const handleCreate = async () => {
    try {
      await tenantService.create(formData);
      setShowCreate(false);
      setFormData({ name: '', code: '' });
      loadTenants();
    } catch (err) {
      console.error('Failed to create tenant', err);
    }
  };

  const handleSuspend = async (id: string) => {
    try { await tenantService.suspend(id); loadTenants(); } catch (err) { console.error(err); }
  };

  const handleActivate = async (id: string) => {
    try { await tenantService.activate(id); loadTenants(); } catch (err) { console.error(err); }
  };

  return (
    <div className={styles.container}>
      <h1 className={styles.title}>Tenant Management</h1>
      <div className={styles.toolbar}>
        <button className={styles.createBtn} onClick={() => setShowCreate(true)}>+ Create Tenant</button>
      </div>
      {showCreate && (
        <div className={styles.modal}>
          <div className={styles.modalContent}>
            <h3>Create New Tenant</h3>
            <div className={styles.field}>
              <label>Name</label>
              <input type="text" value={formData.name}
                onChange={(e) => setFormData({ ...formData, name: e.target.value })} />
            </div>
            <div className={styles.field}>
              <label>Code</label>
              <input type="text" value={formData.code}
                onChange={(e) => setFormData({ ...formData, code: e.target.value })} />
            </div>
            <div className={styles.actions}>
              <button className={styles.saveBtn} onClick={handleCreate}>Create</button>
              <button className={styles.cancelBtn} onClick={() => setShowCreate(false)}>Cancel</button>
            </div>
          </div>
        </div>
      )}
      {loading ? <p>Loading...</p> : (
        <table className={styles.table}>
          <thead><tr><th>Name</th><th>Code</th><th>Status</th><th>Created</th><th>Actions</th></tr></thead>
          <tbody>
            {tenants.map((t) => (
              <tr key={t.id}>
                <td>{t.name}</td><td>{t.code}</td>
                <td><span className={`${styles.badge} ${styles[t.status.toLowerCase()]}`}>{t.status}</span></td>
                <td>{new Date(t.createdAt).toLocaleDateString()}</td>
                <td>
                  {t.status === 'Active'
                    ? <button className={styles.suspendBtn} onClick={() => handleSuspend(t.id)}>Suspend</button>
                    : <button className={styles.activateBtn} onClick={() => handleActivate(t.id)}>Activate</button>}
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      )}
    </div>
  );
}
