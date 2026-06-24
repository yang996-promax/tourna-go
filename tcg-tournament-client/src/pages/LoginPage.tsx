import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { useAuth } from '../context/AuthContext';

export default function LoginPage() {
  const [username, setUsername] = useState('admin');
  const [password, setPassword] = useState('admin123');
  const [error, setError] = useState('');
  const [loading, setLoading] = useState(false);
  const { login } = useAuth();
  const navigate = useNavigate();

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setLoading(true);
    setError('');
    try {
      await login(username, password);
      navigate('/');
    } catch {
      setError('Invalid credentials');
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="min-h-screen flex items-center justify-center bg-gradient-to-br from-slate-950 via-slate-900 to-amber-950">
      <form onSubmit={handleSubmit} className="bg-slate-900 border border-slate-700 rounded-2xl p-8 w-full max-w-md shadow-2xl">
        <h1 className="text-2xl font-bold text-amber-400 mb-2">TCG Tournament Manager</h1>
        <p className="text-slate-400 text-sm mb-6">Local organizer login</p>
        {error && <p className="text-red-400 text-sm mb-4">{error}</p>}
        <label className="block text-sm text-slate-300 mb-1">Username</label>
        <input
          className="w-full bg-slate-800 border border-slate-600 rounded-lg px-3 py-2 mb-4 focus:outline-none focus:border-amber-500"
          value={username}
          onChange={(e) => setUsername(e.target.value)}
        />
        <label className="block text-sm text-slate-300 mb-1">Password</label>
        <input
          type="password"
          className="w-full bg-slate-800 border border-slate-600 rounded-lg px-3 py-2 mb-6 focus:outline-none focus:border-amber-500"
          value={password}
          onChange={(e) => setPassword(e.target.value)}
        />
        <button
          type="submit"
          disabled={loading}
          className="w-full bg-amber-500 hover:bg-amber-400 text-slate-900 font-semibold py-2 rounded-lg transition-colors disabled:opacity-50"
        >
          {loading ? 'Signing in...' : 'Sign In'}
        </button>
        <p className="text-xs text-slate-500 mt-4 text-center">Default: admin / admin123</p>
      </form>
    </div>
  );
}