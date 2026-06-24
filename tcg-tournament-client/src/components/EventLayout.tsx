import { useState } from 'react';
import { Link, Outlet, useLocation, useNavigate, useParams } from 'react-router-dom';
import { ArrowLeft, LayoutDashboard, Users, Swords, Trophy, GitBranch, Trash2 } from 'lucide-react';
import { EventProvider, useEvent } from '../context/EventContext';
import { tournamentApi } from '../api/client';
import DeleteEventModal from './DeleteEventModal';

const tabs = [
  { to: '', label: 'Overview', icon: LayoutDashboard },
  { to: 'players', label: 'Players', icon: Users },
  { to: 'pairings', label: 'Pairings', icon: Swords },
  { to: 'standings', label: 'Standings', icon: Trophy },
  { to: 'topcut', label: 'Top Cut', icon: GitBranch },
];

function EventLayoutInner() {
  const { eventId } = useParams<{ eventId: string }>();
  const { pathname } = useLocation();
  const navigate = useNavigate();
  const { tournament, loading } = useEvent();
  const [showDelete, setShowDelete] = useState(false);
  const base = `/events/${eventId}`;

  const handleDelete = async () => {
    if (!tournament) return;
    await tournamentApi.delete(tournament.id);
    navigate('/');
  };

  if (loading) return <p className="text-slate-400">Loading event...</p>;
  if (!tournament) return <p className="text-red-400">Event not found.</p>;

  return (
    <div>
      <Link to="/" className="inline-flex items-center gap-2 text-sm text-slate-400 hover:text-amber-400 mb-6">
        <ArrowLeft size={16} /> Back to Events
      </Link>

      <div className="bg-slate-900 border border-slate-700 rounded-xl p-5 mb-6">
        <div className="flex flex-wrap justify-between items-start gap-4">
          <div>
            <h2 className="text-2xl font-bold text-amber-300">{tournament.name}</h2>
            <p className="text-slate-400">{tournament.gameTitle} · {tournament.organizer}</p>
            <p className="text-sm text-slate-500 mt-1">
              {new Date(tournament.eventDate).toLocaleDateString()} · {tournament.venue}
            </p>
          </div>
          <div className="flex items-center gap-3">
            <span className="px-3 py-1 bg-amber-500/20 text-amber-300 rounded-full text-sm">
              {tournament.status}
            </span>
            <button
              type="button"
              onClick={() => setShowDelete(true)}
              className="p-2 rounded-lg text-slate-500 hover:text-red-400 hover:bg-red-600/10 transition-colors"
              title="Delete event"
            >
              <Trash2 size={18} />
            </button>
          </div>
        </div>
      </div>

      {showDelete && (
        <DeleteEventModal
          events={[tournament]}
          onClose={() => setShowDelete(false)}
          onConfirm={handleDelete}
        />
      )}

      <nav className="flex gap-1 mb-8 border-b border-slate-800 overflow-x-auto">
        {tabs.map(({ to, label, icon: Icon }) => {
          const path = to ? `${base}/${to}` : base;
          const active = to
            ? pathname.startsWith(path)
            : pathname === base || pathname === `${base}/`;

          return (
            <Link
              key={to || 'overview'}
              to={path}
              className={`flex items-center gap-2 px-4 py-3 text-sm border-b-2 -mb-px whitespace-nowrap transition-colors ${
                active
                  ? 'border-amber-500 text-amber-300'
                  : 'border-transparent text-slate-400 hover:text-slate-200'
              }`}
            >
              <Icon size={16} />
              {label}
            </Link>
          );
        })}
      </nav>

      <Outlet />
    </div>
  );
}

export default function EventLayout() {
  return (
    <EventProvider>
      <EventLayoutInner />
    </EventProvider>
  );
}