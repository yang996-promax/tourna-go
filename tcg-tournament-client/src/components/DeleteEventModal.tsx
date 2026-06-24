import { useState } from 'react';
import { AlertTriangle, Loader2, Trash2, X } from 'lucide-react';
import type { Tournament } from '../types';

interface Props {
  events: Tournament[];
  onClose: () => void;
  onConfirm: () => Promise<void>;
}

export default function DeleteEventModal({ events, onClose, onConfirm }: Props) {
  const [deleting, setDeleting] = useState(false);
  const [error, setError] = useState('');

  const handleConfirm = async () => {
    setDeleting(true);
    setError('');
    try {
      await onConfirm();
      onClose();
    } catch {
      setError('Failed to delete event(s). Please try again.');
    } finally {
      setDeleting(false);
    }
  };

  const isSingle = events.length === 1;

  return (
    <div className="fixed inset-0 z-50 flex items-center justify-center p-4 bg-black/60 backdrop-blur-sm" onClick={onClose}>
      <div
        className="bg-slate-900 border border-slate-700 rounded-2xl w-full max-w-md shadow-2xl"
        onClick={(e) => e.stopPropagation()}
      >
        <div className="flex items-center justify-between p-5 border-b border-slate-800">
          <div className="flex items-center gap-3">
            <div className="p-2 bg-red-900/30 rounded-lg">
              <AlertTriangle className="text-red-400" size={20} />
            </div>
            <h3 className="text-lg font-semibold text-red-300">
              Delete {isSingle ? 'Event' : `${events.length} Events`}
            </h3>
          </div>
          <button onClick={onClose} className="text-slate-400 hover:text-slate-200 p-1">
            <X size={20} />
          </button>
        </div>

        <div className="p-5">
          <p className="text-sm text-slate-300 mb-4">
            {isSingle
              ? 'This will permanently delete the event and all related data (players, pairings, standings, top cut).'
              : 'This will permanently delete the selected events and all related data. This cannot be undone.'}
          </p>

          <ul className="max-h-48 overflow-y-auto border border-slate-800 rounded-lg divide-y divide-slate-800">
            {events.map((e) => (
              <li key={e.id} className="px-4 py-3 text-sm">
                <p className="font-medium text-slate-200">{e.name}</p>
                <p className="text-slate-500 text-xs mt-0.5">
                  {e.gameTitle} · {e.playerCount} players · {e.status}
                </p>
              </li>
            ))}
          </ul>

          {error && <p className="text-sm text-red-400 mt-3">{error}</p>}
        </div>

        <div className="flex justify-end gap-3 p-5 border-t border-slate-800">
          <button onClick={onClose} className="btn-secondary" disabled={deleting}>Cancel</button>
          <button
            onClick={handleConfirm}
            disabled={deleting}
            className="flex items-center gap-2 px-4 py-2 rounded-lg bg-red-600 hover:bg-red-500 text-white font-semibold text-sm transition-colors disabled:opacity-50"
          >
            {deleting ? <Loader2 className="animate-spin" size={16} /> : <Trash2 size={16} />}
            {deleting ? 'Deleting...' : `Delete ${isSingle ? 'Event' : 'Selected'}`}
          </button>
        </div>
      </div>
    </div>
  );
}