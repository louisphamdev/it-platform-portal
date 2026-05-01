import styles from './Dashboard.module.css';

export default function Dashboard() {
  const stats = [
    { label: 'Total Users', value: '24' },
    { label: 'Active Sessions', value: '18' },
    { label: 'Pending Approvals', value: '3' },
    { label: 'Audit Events', value: '156' },
  ];

  return (
    <div className={styles.container}>
      <h2>Dashboard</h2>
      <div className={styles.grid}>
        {stats.map((stat) => (
          <div key={stat.label} className={styles.card}>
            <span className={styles.value}>{stat.value}</span>
            <span className={styles.label}>{stat.label}</span>
          </div>
        ))}
      </div>
    </div>
  );
}
