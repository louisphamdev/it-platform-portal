"use client";

import { useState, useEffect } from 'react';
import { permissionService, Permission } from '@/services';
import styles from './Permissions.module.css';

export default function Permissions() {
  const [permissions, setPermissions] = useState<Permission[]>([]);
  const [loading, setLoading] = useState(true);

  const loadPermissions = async () => {
    setLoading(true);
    try {
      const data = await permissionService.getAll();
      setPermissions(data);
    } catch (err) {
      console.error('Failed to load permissions', err);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => { loadPermissions(); }, []);

  return (
    <div className={styles.container}>
      <h1 className={styles.title}>Permissions</h1>
      {loading ? <p>Loading...</p> : (
        <table className={styles.table}>
          <thead><tr><th>Code</th><th>Name</th><th>Module</th><th>Description</th><th>Status</th></tr></thead>
          <tbody>
            {permissions.map((perm) => (
              <tr key={perm.id}>
                <td><code>{perm.code}</code></td>
                <td>{perm.name}</td>
                <td>{perm.module}</td>
                <td>{perm.description}</td>
                <td>
                  <span className={`${styles.badge} ${perm.isActive ? styles.active : styles.inactive}`}>
                    {perm.isActive ? 'Active' : 'Inactive'}
                  </span>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      )}
    </div>
  );
}
