import { useEffect, useState } from 'react';
import { playerApi } from '../api/client';
import { useEvent } from '../context/EventContext';
import type { Player } from '../types';
import { Plus, Search, Upload, Download, Pencil, Trash2 } from 'lucide-react';

export default function PlayersPage() {
  const { tournament, refresh } = useEvent();
  const [players, setPlayers] = useState<Player[]>([]);
  const [search, setSearch] = useState('');
  const [form, setForm] = useState({ externalPlayerId: '', name: '', contactNumber: '', deckName: '', playerNumber: '' });
  const [editing, setEditing] = useState<Player | null>(null);

  const load = async () => {
    if (!tournament) return;
    setPlayers(await playerApi.getAll(tournament.id));
  };

  useEffect(() => { load(); }, [tournament?.id]);

  const handleSearch = async () => {
    if (!tournament || !search.trim()) { load(); return; }
    setPlayers(await playerApi.search(tournament.id, search));
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!tournament) return;
    const data = {
      externalPlayerId: form.externalPlayerId,
      name: form.name,
      contactNumber: form.contactNumber || null,
      deckName: form.deckName || null,
      playerNumber: form.playerNumber ? parseInt(form.playerNumber) : null,
    };
    if (editing) {
      await playerApi.update(tournament.id, editing.tournamentPlayerId, { ...data, playerNumber: parseInt(form.playerNumber) || editing.playerNumber });
    } else {
      await playerApi.add(tournament.id, data);
    }
    setForm({ externalPlayerId: '', name: '', contactNumber: '', deckName: '', playerNumber: '' });
    setEditing(null);
    await load();
    await refresh();
  };

  const handleEdit = (p: Player) => {
    setEditing(p);
    setForm({ externalPlayerId: p.externalPlayerId, name: p.name, contactNumber: p.contactNumber || '', deckName: p.deckName || '', playerNumber: String(p.playerNumber) });
  };

  const handleDelete = async (p: Player) => {
    if (!tournament || !confirm(`Remove ${p.name}?`)) return;
    await playerApi.remove(tournament.id, p.tournamentPlayerId);
    await load();
    await refresh();
  };

  const handleImport = async (e: React.ChangeEvent<HTMLInputElement>) => {
    if (!tournament || !e.target.files?.[0]) return;
    await playerApi.importCsv(tournament.id, e.target.files[0]);
    await load();
    await refresh();
  };

  const handleExport = async () => {
    if (!tournament) return;
    const res = await playerApi.exportCsv(tournament.id);
    const url = URL.createObjectURL(res.data);
    const a = document.createElement('a');
    a.href = url;
    a.download = `players_${tournament.id}.csv`;
    a.click();
  };

  if (!tournament) return null;

  return (
    <div>
      <div className="flex justify-between items-center mb-6">
        <h3 className="text-lg font-semibold">Player Registration</h3>
        <div className="flex gap-2">
          <label className="btn-secondary cursor-pointer flex items-center gap-2">
            <Upload size={16} /> Import CSV
            <input type="file" accept=".csv" className="hidden" onChange={handleImport} />
          </label>
          <button onClick={handleExport} className="btn-secondary flex items-center gap-2"><Download size={16} /> Export</button>
        </div>
      </div>

      <form onSubmit={handleSubmit} className="bg-slate-900 border border-slate-700 rounded-xl p-4 mb-6 grid grid-cols-2 md:grid-cols-5 gap-3">
        <input placeholder="Player ID" className="input" value={form.externalPlayerId} onChange={(e) => setForm({ ...form, externalPlayerId: e.target.value })} required />
        <input placeholder="Name" className="input" value={form.name} onChange={(e) => setForm({ ...form, name: e.target.value })} required />
        <input placeholder="Contact" className="input" value={form.contactNumber} onChange={(e) => setForm({ ...form, contactNumber: e.target.value })} />
        <input placeholder="Deck Name" className="input" value={form.deckName} onChange={(e) => setForm({ ...form, deckName: e.target.value })} />
        <button type="submit" className="btn-primary flex items-center justify-center gap-2">
          <Plus size={16} /> {editing ? 'Update' : 'Add Player'}
        </button>
      </form>

      <div className="flex gap-2 mb-4">
        <input placeholder="Search players..." className="input flex-1" value={search} onChange={(e) => setSearch(e.target.value)} onKeyDown={(e) => e.key === 'Enter' && handleSearch()} />
        <button onClick={handleSearch} className="btn-secondary flex items-center gap-2"><Search size={16} /> Search</button>
      </div>

      <table className="w-full text-sm">
        <thead>
          <tr className="text-slate-400 border-b border-slate-700">
            <th className="text-left py-2">#</th>
            <th className="text-left py-2">ID</th>
            <th className="text-left py-2">Name</th>
            <th className="text-left py-2">Contact</th>
            <th className="text-left py-2">Deck</th>
            <th className="text-right py-2">Actions</th>
          </tr>
        </thead>
        <tbody>
          {players.map((p) => (
            <tr key={p.tournamentPlayerId} className="border-b border-slate-800 hover:bg-slate-900/50">
              <td className="py-3 text-amber-400">{p.playerNumber}</td>
              <td className="py-3">{p.externalPlayerId}</td>
              <td className="py-3 font-medium">{p.name}</td>
              <td className="py-3 text-slate-400">{p.contactNumber}</td>
              <td className="py-3 text-slate-400">{p.deckName}</td>
              <td className="py-3 text-right">
                <button onClick={() => handleEdit(p)} className="text-slate-400 hover:text-amber-400 mr-3"><Pencil size={16} /></button>
                <button onClick={() => handleDelete(p)} className="text-slate-400 hover:text-red-400"><Trash2 size={16} /></button>
              </td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  );
}