import { useEffect } from 'react';
import { useRouter } from 'next/router';

export default function Home() {
  const router = useRouter();
  
  useEffect(() => {
    // Check if user is authenticated
    const token = localStorage.getItem('auth_token');
    if (!token) {
      router.push('/login');
    }
  }, [router]);
  
  return (
    <div style={{ padding: '2rem', textAlign: 'center' }}>
      <h1>IT Platform Portal</h1>
      <p>Welcome to the IT Platform Portal</p>
      <div style={{ marginTop: '2rem' }}>
        <a href="/dashboard" style={{ margin: '0 0.5rem', padding: '0.5rem 1rem', background: '#0066cc', color: 'white', borderRadius: '4px', textDecoration: 'none' }}>
          Dashboard
        </a>
        <a href="/users" style={{ margin: '0 0.5rem', padding: '0.5rem 1rem', background: '#0066cc', color: 'white', borderRadius: '4px', textDecoration: 'none' }}>
          Users
        </a>
      </div>
    </div>
  );
}
