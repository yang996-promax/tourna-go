import { useEffect, useMemo, useState } from 'react';
import { Link } from 'react-router-dom';
import { tournamentApi } from '../api/client';
import type { Tournament } from '../types';
import { Calendar, ChevronRight, MapPin, Trash2, Users } from 'lucide-react';
import Checkbox from '../components/Checkbox';
import DeleteEventModal from '../components/DeleteEventModal';

export default function DashboardPage() {
  const [tournaments, setTournaments] = useState<Tournament[]>([]);
  const [loading, setLoading] = useState(true);
  const [showCreate, setShowCreate] = useState(false);
  const [selectedIds, setSelectedIds] = useState<Set<number>>(new Set());
  const [deleteTargets, setDeleteTargets] = useState<Tournament[] | null>(null);
  const defaultForm = {
    name: '',
    gameTitle: '',
    eventDate: '',
    organizer: '',
    venue: '',
    totalSwissRounds: 5,
    topCutSize: 8,
    firstRoundPairingMode: 'Random' as const,
    matchFormat: 'BO3' as const,
    hasElimination: false,
    eliminationLossCount: 2,
  };

  const [form, setForm] = useState(defaultForm);

  const load = () => {
    tournamentApi.getAll()
      .then(setTournaments)
      .finally(() => setLoading(false));
  };

  useEffect(() => { load(); }, []);

  const allSelected = tournaments.length > 0 && selectedIds.size === tournaments.length;
  const someSelected = selectedIds.size > 0 && !allSelected;

  const selectedEvents = useMemo(
    () => tournaments.filter((t) => selectedIds.has(t.id)),
    [tournaments, selectedIds],
  );

  const toggleSelect = (id: number) => {
    setSelectedIds((prev) => {
      const next = new Set(prev);
      if (next.has(id)) next.delete(id);
      else next.add(id);
      return next;
    });
  };

  const toggleSelectAll = () => {
    if (allSelected) {
      setSelectedIds(new Set());
    } else {
      setSelectedIds(new Set(tournaments.map((t) => t.id)));
    }
  };

  const handleDeleteConfirm = async () => {
    if (!deleteTargets) return;
    await Promise.all(deleteTargets.map((e) => tournamentApi.delete(e.id)));
    setSelectedIds((prev) => {
      const deleted = new Set(deleteTargets.map((e) => e.id));
      const next = new Set([...prev].filter((id) => !deleted.has(id)));
      return next;
    });
    setDeleteTargets(null);
    load();
  };

  const handleCreate = async (e: React.FormEvent) => {
    e.preventDefault();
    await tournamentApi.create({
      name: form.name,
      gameTitle: form.gameTitle,
      eventDate: new Date(form.eventDate).toISOString(),
      organizer: form.organizer,
      venue: form.venue,
      totalSwissRounds: form.totalSwissRounds,
      topCutSize: form.topCutSize === 0 ? 'None' : `Top${form.topCutSize}`,
      firstRoundPairingMode: form.firstRoundPairingMode,
      matchFormat: form.matchFormat,
      hasElimination: form.hasElimination,
      eliminationLossCount: form.hasElimination ? form.eliminationLossCount : null,
    });
    setShowCreate(false);
    setForm(defaultForm);
    load();
  };

  if (loading) return <p className="text-slate-400">Loading...</p>;

  return (
    <div>
      <div className="flex justify-between items-center mb-6">
        <div>
          <h2 className="text-2xl font-bold">Events</h2>
          <p className="text-slate-400 text-sm mt-1">Select an event to manage players, pairings, and standings</p>
        </div>
        <div className="flex items-center gap-3">
          {selectedIds.size > 0 && (
            <button
              onClick={() => setDeleteTargets(selectedEvents)}
              className="flex items-center gap-2 px-4 py-2 rounded-lg bg-red-600/20 hover:bg-red-600/30 text-red-400 border border-red-800 text-sm font-medium transition-colors"
            >
              <Trash2 size={16} />
              Delete Selected ({selectedIds.size})
            </button>
          )}
          <button onClick={() => setShowCreate(!showCreate)} className="btn-primary">
            Create Event
          </button>
        </div>
      </div>

      {showCreate && (
        <form onSubmit={handleCreate} className="bg-slate-900 border border-slate-700 rounded-xl p-4 mb-6 grid grid-cols-2 gap-3">
          <input className="input" placeholder="Tournament Name" value={form.name} onChange={(e) => setForm({ ...form, name: e.target.value })} required />
          <input className="input" placeholder="Game Title" value={form.gameTitle} onChange={(e) => setForm({ ...form, gameTitle: e.target.value })} required />
          <input className="input" type="date" value={form.eventDate} onChange={(e) => setForm({ ...form, eventDate: e.target.value })} required />
          <input className="input" placeholder="Organizer" value={form.organizer} onChange={(e) => setForm({ ...form, organizer: e.target.value })} required />
          <input className="input" placeholder="Venue" value={form.venue} onChange={(e) => setForm({ ...form, venue: e.target.value })} required />
          <input className="input" type="number" min={1} max={15} placeholder="Swiss Rounds" value={form.totalSwissRounds} onChange={(e) => setForm({ ...form, totalSwissRounds: parseInt(e.target.value) })} />
          <select className="input" value={form.topCutSize} onChange={(e) => setForm({ ...form, topCutSize: parseInt(e.target.value) })}>
            <option value={0}>No Top Cut</option>
            <option value={4}>Top 4</option>
            <option value={8}>Top 8</option>
            <option value={16}>Top 16</option>
          </select>
          <select className="input" value={form.firstRoundPairingMode} onChange={(e) => setForm({ ...form, firstRoundPairingMode: e.target.value as typeof form.firstRoundPairingMode })}>
            <option value="Random">Round 1: Random</option>
            <option value="Seeded">Round 1: Seeded</option>
          </select>
          <select className="input" value={form.matchFormat} onChange={(e) => setForm({ ...form, matchFormat: e.target.value as typeof form.matchFormat })}>
            <option value="BO1">Best of 1 (BO1)</option>
            <option value="BO3">Best of 3 (BO3)</option>
          </select>
          <div className="flex items-center gap-3">
            <label className="flex items-center gap-2 text-sm text-slate-300 cursor-pointer">
              <input
                type="checkbox"
                checked={form.hasElimination}
                onChange={(e) => setForm({ ...form, hasElimination: e.target.checked })}
                className="rounded border-slate-600 bg-slate-800 text-amber-500 focus:ring-amber-500"
              />
              Elimination
            </label>
            {form.hasElimination && (
              <input
                className="input flex-1"
                type="number"
                min={1}
                max={10}
                value={form.eliminationLossCount}
                onChange={(e) => setForm({ ...form, eliminationLossCount: parseInt(e.target.value) || 1 })}
                placeholder="Losses to eliminate"
                title="Number of match losses before a player is eliminated"
              />
            )}
          </div>
          {form.hasElimination && (
            <p className="col-span-2 text-xs text-slate-500">
              Players are dropped after {form.eliminationLossCount} match loss{form.eliminationLossCount === 1 ? '' : 'es'}.
            </p>
          )}
          <button type="submit" className="btn-primary col-span-2">Create Event</button>
        </form>
      )}

      {tournaments.length === 0 ? (
        <div className="bg-slate-900 border border-slate-800 rounded-xl p-12 text-center">
          <p className="text-slate-400">No events yet. Create your first tournament to get started.</p>
        </div>
      ) : (
        <div className="space-y-3">
          <div
            role="button"
            tabIndex={0}
            onClick={toggleSelectAll}
            onKeyDown={(e) => { if (e.key === 'Enter' || e.key === ' ') { e.preventDefault(); toggleSelectAll(); } }}
            className={`flex items-center gap-3 w-full sm:w-auto px-4 py-2.5 rounded-xl border transition-all duration-200 select-none cursor-pointer ${
              someSelected || allSelected
                ? 'bg-amber-500/10 border-amber-500/30 hover:border-amber-500/50'
                : 'bg-slate-900/60 border-slate-800 hover:border-slate-700 hover:bg-slate-900'
            }`}
          >
            <Checkbox checked={allSelected} indeterminate={someSelected} onChange={toggleSelectAll} />
            <span className={`text-sm font-medium transition-colors ${
              allSelected ? 'text-amber-300' : someSelected ? 'text-amber-300/90' : 'text-slate-400'
            }`}>
              {allSelected
                ? `All ${tournaments.length} events selected`
                : someSelected
                  ? `${selectedIds.size} of ${tournaments.length} selected`
                  : 'Select all events'}
            </span>
          </div>

          <div className="grid gap-4">
            {tournaments.map((t) => {
              const isSelected = selectedIds.has(t.id);
              return (
                <div
                  key={t.id}
                  className={`border rounded-xl p-5 flex items-center gap-4 transition-all duration-200 ${
                    isSelected
                      ? 'bg-amber-500/5 border-amber-500/40 shadow-sm shadow-amber-500/5'
                      : 'bg-slate-900 border-slate-800 hover:border-amber-500/30'
                  }`}
                >
                  <Checkbox checked={isSelected} onChange={() => toggleSelect(t.id)} />
                  <Link
                    to={`/events/${t.id}`}
                    className="flex-1 flex justify-between items-center group min-w-0"
                  >
                    <div className="flex-1 min-w-0">
                      <div className="flex items-center gap-3 mb-2">
                        <h3 className="font-semibold text-lg group-hover:text-amber-300 transition-colors truncate">{t.name}</h3>
                        <span className="px-2 py-0.5 bg-slate-800 text-slate-400 rounded text-xs shrink-0">{t.status}</span>
                      </div>
                      <p className="text-slate-400 text-sm mb-3">{t.gameTitle}</p>
                      <div className="flex flex-wrap gap-4 text-xs text-slate-500">
                        <span className="flex items-center gap-1"><Calendar size={12} /> {new Date(t.eventDate).toLocaleDateString()}</span>
                        <span className="flex items-center gap-1"><MapPin size={12} /> {t.venue}</span>
                        <span className="flex items-center gap-1"><Users size={12} /> {t.playerCount} players</span>
                        <span>Round {t.currentRound} / {t.totalSwissRounds}</span>
                      </div>
                    </div>
                    <ChevronRight className="text-slate-600 group-hover:text-amber-400 transition-colors shrink-0 ml-4" size={24} />
                  </Link>
                  <button
                    type="button"
                    onClick={() => setDeleteTargets([t])}
                    className="shrink-0 p-2 rounded-lg text-slate-500 hover:text-red-400 hover:bg-red-600/10 transition-colors"
                    title="Delete event"
                  >
                    <Trash2 size={18} />
                  </button>
                </div>
              );
            })}
          </div>
        </div>
      )}

      {deleteTargets && (
        <DeleteEventModal
          events={deleteTargets}
          onClose={() => setDeleteTargets(null)}
          onConfirm={handleDeleteConfirm}
        />
      )}
    </div>
  );
}