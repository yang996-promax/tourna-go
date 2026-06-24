import { useEffect, useState } from 'react';
import { Link } from 'react-router-dom';
import { playerApi } from '../api/client';
import type { Player, Tournament } from '../types';
import { AlertTriangle, Loader2, Play, Users, X } from 'lucide-react';

interface Props {
  tournament: Tournament;
  onClose: () => void;
  onConfirm: () => Promise<void>;
}

export default function StartTournamentModal({ tournament, onClose, onConfirm }: Props) {
  const [players, setPlayers] = useState<Player[]>([]);
  const [loading, setLoading] = useState(true);
  const [starting, setStarting] = useState(false);
  const [error, setError] = useState('');

  const minRequired = Math.max(2, tournament.topCutSize || 0);
  const activePlayers = players.filter((p) => !p.isDropped);
  const hasEnough = activePlayers.length >= minRequired;

  useEffect(() => {
    playerApi.getAll(tournament.id)
      .then(setPlayers)
      .catch(() => setError('Failed to load player list.'))
      .finally(() => setLoading(false));
  }, [tournament.id]);

  const handleConfirm = async () => {
    if (!hasEnough) return;
    setStarting(true);
    setError('');
    try {
      await onConfirm();
      onClose();
    } catch (err: unknown) {
      const msg = (err as { response?: { data?: { message?: string } } })?.response?.data?.message;
      setError(msg ?? 'Failed to start tournament.');
    } finally {
      setStarting(false);
    }
  };

  return (
    <div className="fixed inset-0 z-50 flex items-center justify-center p-4 bg-black/60 backdrop-blur-sm" onClick={onClose}>
      <div
        className="bg-slate-900 border border-slate-700 rounded-2xl w-full max-w-lg shadow-2xl"
        onClick={(e) => e.stopPropagation()}
      >
        <div className="flex items-center justify-between p-5 border-b border-slate-800">
          <div>
            <h3 className="text-lg font-semibold text-amber-300">Start Tournament</h3>
            <p className="text-sm text-slate-400 mt-0.5">{tournament.name}</p>
          </div>
          <button onClick={onClose} className="text-slate-400 hover:text-slate-200 p-1">
            <X size={20} />
          </button>
        </div>

        <div className="p-5">
          {loading ? (
            <div className="flex items-center justify-center gap-2 py-8 text-slate-400">
              <Loader2 className="animate-spin" size={20} /> Loading players...
            </div>
          ) : (
            <>
              {!hasEnough && (
                <div className="flex gap-3 bg-red-900/30 border border-red-700/50 rounded-xl p-4 mb-4">
                  <AlertTriangle className="text-red-400 shrink-0 mt-0.5" size={20} />
                  <div>
                    <p className="font-medium text-red-300">Not enough players</p>
                    <p className="text-sm text-red-200/80 mt-1">
                      This event requires at least <strong>{minRequired}</strong> players
                      {tournament.topCutSize > 0 && ` (Top ${tournament.topCutSize} cut)`}.
                      Currently registered: <strong>{activePlayers.length}</strong>.
                    </p>
                    <Link to="players" onClick={onClose} className="text-sm text-amber-400 hover:underline mt-2 inline-block">
                      Add more players →
                    </Link>
                  </div>
                </div>
              )}

              {hasEnough && (
                <div className="flex items-center gap-2 text-sm text-slate-400 mb-4">
                  <Users size={16} className="text-amber-400" />
                  <span>
                    <strong className="text-slate-200">{activePlayers.length}</strong> players ready
                    {tournament.topCutSize > 0 && ` · Top ${tournament.topCutSize} cut`}
                    {' · '}{tournament.totalSwissRounds} Swiss rounds
                  </span>
                </div>
              )}

              <p className="text-xs text-slate-500 mb-2 uppercase tracking-wide">Registered players</p>
              <div className="max-h-64 overflow-y-auto border border-slate-800 rounded-lg">
                {activePlayers.length === 0 ? (
                  <p className="text-slate-500 text-sm p-4 text-center">No players registered yet.</p>
                ) : (
                  <table className="w-full text-sm">
                    <thead className="bg-slate-800/80 sticky top-0">
                      <tr className="text-slate-400 text-left">
                        <th className="py-2 px-3 font-medium">#</th>
                        <th className="py-2 px-3 font-medium">Name</th>
                        <th className="py-2 px-3 font-medium hidden sm:table-cell">Deck</th>
                      </tr>
                    </thead>
                    <tbody>
                      {activePlayers.map((p) => (
                        <tr key={p.tournamentPlayerId} className="border-t border-slate-800">
                          <td className="py-2 px-3 text-amber-400">{p.playerNumber}</td>
                          <td className="py-2 px-3 font-medium">{p.name}</td>
                          <td className="py-2 px-3 text-slate-500 hidden sm:table-cell truncate max-w-[140px]">
                            {p.deckName ?? '—'}
                          </td>
                        </tr>
                      ))}
                    </tbody>
                  </table>
                )}
              </div>

              {hasEnough && (
                <p className="text-sm text-slate-400 mt-4">
                  Confirm to start the tournament. Player registration will be locked once Swiss begins.
                </p>
              )}

              {error && (
                <p className="text-sm text-red-400 mt-3">{error}</p>
              )}
            </>
          )}
        </div>

        <div className="flex justify-end gap-3 p-5 border-t border-slate-800">
          <button onClick={onClose} className="btn-secondary" disabled={starting}>
            {hasEnough ? 'Cancel' : 'Close'}
          </button>
          {hasEnough && !loading && (
            <button onClick={handleConfirm} disabled={starting} className="btn-primary flex items-center gap-2">
              {starting ? <Loader2 className="animate-spin" size={16} /> : <Play size={16} />}
              {starting ? 'Starting...' : 'Confirm & Start'}
            </button>
          )}
        </div>
      </div>
    </div>
  );
}