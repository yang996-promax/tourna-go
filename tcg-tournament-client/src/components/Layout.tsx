import { Link, Outlet, useLocation } from 'react-router-dom';
import { LayoutDashboard, LogOut } from 'lucide-react';
import { useAuth } from '../context/AuthContext';

export default function Layout() {
  const { pathname } = useLocation();
  const { displayName, orgCD, logout } = useAuth();
  const isHome = pathname === '/';

  return (
    <div className="min-h-screen flex">
      <aside className="w-64 bg-slate-900 border-r border-slate-800 p-4 flex flex-col">
        <div className="mb-8">
          <h1 className="text-xl font-bold text-amber-400">TCG Tournament</h1>
          <p className="text-xs text-slate-400 mt-1">Manager</p>
        </div>
        <nav className="space-y-1 flex-1">
          <Link
            to="/"
            className={`flex items-center gap-3 px-3 py-2 rounded-lg text-sm transition-colors ${
              isHome ? 'bg-amber-500/20 text-amber-300' : 'text-slate-300 hover:bg-slate-800'
            }`}
          >
            <LayoutDashboard size={18} />
            Events
          </Link>
        </nav>
        <div className="border-t border-slate-800 pt-4">
          <p className="text-sm text-slate-400 mb-1">{displayName}</p>
          {orgCD && <p className="text-xs text-slate-500 mb-2">Org: {orgCD}</p>}
          <button
            onClick={logout}
            className="flex items-center gap-2 text-sm text-slate-400 hover:text-red-400"
          >
            <LogOut size={16} /> Logout
          </button>
        </div>
      </aside>
      <main className="flex-1 p-8 overflow-auto">
        <Outlet />
      </main>
    </div>
  );
}