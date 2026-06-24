import { useEffect, useState } from 'react';
import { matchApi, tournamentApi } from '../api/client';
import { useEvent } from '../context/EventContext';
import TopCutBracketView from '../components/TopCutBracketView';
import type { TopCutBracket, TopCutTree } from '../types';
import { Crown, GitBranch, Loader2, Users } from 'lucide-react';

export default function TopCutPage() {
  const { tournament, refresh } = useEvent();
  const [tree, setTree] = useState<TopCutTree | null>(null);
  const [loading, setLoading] = useState(true);
  const [generating, setGenerating] = useState(false);
  const [submittingId, setSubmittingId] = useState<number | null>(null);
  const [error, setError] = useState('');

  const load = async (options?: { silent?: boolean }) => {
    if (!tournament) return;
    if (!options?.silent) setLoading(true);
    setError('');
    try {
      setTree(await tournamentApi.getTopCut(tournament.id));
    } catch {
      if (!options?.silent) setTree(null);
      setError('Failed to load top cut data.');
    } finally {
      if (!options?.silent) setLoading(false);
    }
  };

  useEffect(() => { load(); }, [tournament?.id]);

  const handleGenerate = async () => {
    if (!tournament) return;
    setGenerating(true);
    setError('');
    try {
      setTree(await tournamentApi.generateTopCut(tournament.id));
      await refresh();
    } catch (err: unknown) {
      const msg = (err as { response?: { data?: { message?: string } } })?.response?.data?.message;
      setError(msg ?? 'Failed to generate bracket.');
    } finally {
      setGenerating(false);
    }
  };

  const handleWinner = async (bracket: TopCutBracket, winnerId: number) => {
    if (!confirm(`Record ${winnerId === bracket.playerAId ? bracket.playerAName : bracket.playerBName} as the winner?`)) return;

    setSubmittingId(bracket.id);
    setError('');
    try {
      await matchApi.enterTopCutResult(bracket.id, winnerId);
      await load({ silent: true });
      await refresh();
    } catch (err: unknown) {
      const msg = (err as { response?: { data?: { message?: string } } })?.response?.data?.message;
      setError(msg ?? 'Failed to record result.');
      await load({ silent: true });
    } finally {
      setSubmittingId(null);
    }
  };

  if (!tournament) return null;

  if (loading) {
    return (
      <div className="flex items-center gap-2 text-slate-400">
        <Loader2 className="animate-spin" size={18} /> Loading top cut...
      </div>
    );
  }

  const hasBracket = (tree?.brackets.length ?? 0) > 0;
  const canGenerate = tree?.canGenerate ?? false;

  return (
    <div>
      <div className="flex flex-wrap justify-between items-start gap-4 mb-6">
        <div>
          <h3 className="text-lg font-semibold flex items-center gap-2">
            <GitBranch size={20} className="text-amber-400" />
            Top Cut Bracket
            {tournament.topCutSize > 0 && (
              <span className="text-sm font-normal text-slate-400">— Top {tournament.topCutSize}</span>
            )}
          </h3>
          {tree?.statusMessage && (
            <p className="text-sm text-slate-400 mt-1">{tree.statusMessage}</p>
          )}
        </div>
        {canGenerate && (
          <button onClick={handleGenerate} disabled={generating} className="btn-primary flex items-center gap-2">
            {generating ? <Loader2 className="animate-spin" size={16} /> : <GitBranch size={16} />}
            Generate Top {tournament.topCutSize}
          </button>
        )}
      </div>

      {error && (
        <div className="bg-red-900/20 border border-red-700/50 text-red-300 rounded-lg px-4 py-3 mb-6 text-sm">
          {error}
        </div>
      )}

      {tree?.championName && (
        <div className="bg-gradient-to-r from-amber-500/20 to-amber-600/10 border border-amber-500/30 rounded-xl p-6 mb-8 flex items-center gap-4">
          <Crown className="text-amber-400 shrink-0" size={36} />
          <div>
            <p className="text-sm text-amber-300 uppercase tracking-wide">Champion</p>
            <p className="text-3xl font-bold">{tree.championName}</p>
          </div>
        </div>
      )}

      {!hasBracket && tree?.qualifiedPlayers && tree.qualifiedPlayers.length > 0 && (
        <div className="bg-slate-900 border border-slate-700 rounded-xl p-5 mb-8">
          <h4 className="text-sm font-semibold text-slate-300 mb-4 flex items-center gap-2">
            <Users size={16} /> Qualified Players (by Swiss standings)
          </h4>
          <div className="grid sm:grid-cols-2 lg:grid-cols-4 gap-2">
            {tree.qualifiedPlayers.map((p) => (
              <div
                key={p.tournamentPlayerId}
                className="flex items-center gap-3 bg-slate-800/50 rounded-lg px-3 py-2 text-sm"
              >
                <span className="w-6 h-6 rounded-full bg-amber-500/20 text-amber-300 flex items-center justify-center text-xs font-bold">
                  {p.seed}
                </span>
                <div className="min-w-0">
                  <p className="font-medium truncate">{p.playerName}</p>
                  <p className="text-xs text-slate-500">{p.matchPoints} MP</p>
                </div>
              </div>
            ))}
          </div>
        </div>
      )}

      {hasBracket ? (
        <TopCutBracketView
          brackets={tree!.brackets}
          onSelectWinner={handleWinner}
          submittingId={submittingId}
        />
      ) : tournament.topCutSize === 0 ? (
        <p className="text-slate-400">This event has no top cut configured.</p>
      ) : !canGenerate ? (
        <p className="text-slate-400">Complete all Swiss rounds and match results, then generate the bracket here.</p>
      ) : null}

      {hasBracket && !tree?.championName && (
        <p className="text-xs text-slate-500 mt-6 text-center">
          Winners automatically advance to the next round. The tournament completes when the final is decided.
        </p>
      )}
    </div>
  );
}