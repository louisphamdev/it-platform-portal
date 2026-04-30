"use client";

import { useState, useEffect } from 'react';
import { auditService, AuditLog } from '@/services';
import styles from './AuditLogs.module.css';

export default function AuditLogs() {
  const [logs, setLogs] = useState<AuditLog[]>([]);
  const [loading, setLoading] = useState(true);
  const [page, setPage] = useState(1);
  const [total, setTotal] = useState(0);
  const pageSize = 20;

  const loadLogs = async () => {
    setLoading(true);
    try {
      const [data, count] = await Promise.all([
        auditService.query({ page, pageSize }),
        auditService.getCount({ page, pageSize }),
      ]);
      setLogs(data);
      setTotal(count);
    } catch (err) {
      console.error('Failed to load audit logs', err);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => { loadLogs(); }, [page]);

  return (
    <div className={styles.container}>
      <h1 className={styles.title}>Audit Logs</h1>
      {loading ? <p>Loading...</p> : (
        <>
          <table className={styles.table}>
            <thead><tr><th>Timestamp</th><th>User</th><th>Action</th><th>Entity</th><th>Status</th><th>Duration</th></tr></thead>
            <tbody>
              {logs.map((log) => (
                <tr key={log.id}>
                  <td>{new Date(log.createdAt).toLocaleString()}</td>
                  <td>{log.userName}</td>
                  <td>{log.action}</td>
                  <td>{log.entityType}:{log.entityId}</td>
                  <td>
                    <span className={`${styles.badge} ${log.statusCode >= 400 ? styles.error : styles.success}`}>
                      {log.statusCode}
                    </span>
                  </td>
                  <td>{log.durationMs}ms</td>
                </tr>
              ))}
            </tbody>
          </table>
          <div className={styles.pagination}>
            <button disabled={page <= 1} onClick={() => setPage(page - 1)}>Previous</button>
            <span>Page {page} of {Math.max(1, Math.ceil(total / pageSize))}</span>
            <button disabled={page >= Math.ceil(total / pageSize)} onClick={() => setPage(page + 1)}>Next</button>
          </div>
        </>
      )}
    </div>
  );
}
