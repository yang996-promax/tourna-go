import { useEffect, useState } from 'react';
import { CheckCircle2, Circle } from 'lucide-react';
import { matchApi, tournamentApi } from '../api/client';
import MatchResultModal from '../components/MatchResultModal';
import { useEvent } from '../context/EventContext';
import type { Match, MatchResultType, Round } from '../types';

export default function PairingsPage() {
  const { tournament, refresh } = useEvent();
  const [rounds, setRounds] = useState<Round[]>([]);
  const [selectedRound, setSelectedRound] = useState<number>(0);
  const [selectedMatch, setSelectedMatch] = useState<Match | null>(null);

  const load = async () => {
    if (!tournament) return;
    const r = await tournamentApi.getRounds(tournament.id, true);
    setRounds(r);
    setSelectedRound((prev) => {
      if (r.length === 0) return 0;
      if (prev && r.some((round) => round.roundNumber === prev)) return prev;
      return r[r.length - 1].roundNumber;
    });
  };

  useEffect(() => { load(); }, [tournament?.id]);

  const handleResult = async (resultType: MatchResultType) => {
    if (!selectedMatch) return;
    await matchApi.enterResult(selectedMatch.id, resultType);
    await load();
    await refresh();
  };

  if (!tournament) return null;

  const currentRound = rounds.find((r) => r.roundNumber === selectedRound);
  const matchFormat = tournament.matchFormat ?? 'BO3';
  const playableMatches = currentRound?.matches.filter((m) => !m.isBye) ?? [];
  const completedCount = playableMatches.filter((m) => m.isComplete).length;
  const totalPlayable = playableMatches.length;

  return (
    <div>
      <h3 className="text-lg font-semibold mb-6">Pairings</h3>

      <div className="flex gap-2 mb-6 flex-wrap">
        {rounds.map((r) => (
          <button
            key={r.id}
            onClick={() => setSelectedRound(r.roundNumber)}
            className={`px-4 py-2 rounded-lg text-sm ${
              selectedRound === r.roundNumber ? 'bg-amber-500 text-slate-900' : 'bg-slate-800 text-slate-300'
            }`}
          >
            Round {r.roundNumber} {r.isComplete ? '✓' : ''}
          </button>
        ))}
      </div>

      {!currentRound ? (
        <p className="text-slate-400">
          No rounds generated yet. Start the tournament and generate Round 1 from the Overview tab.
        </p>
      ) : (
        <>
          <div className="flex flex-wrap items-center justify-between gap-3 mb-4">
            <p className="text-sm text-slate-400">
              Round {currentRound.roundNumber} overview
              {totalPlayable > 0 && (
                <span className="text-slate-300">
                  {' '}
                  — {completedCount}/{totalPlayable} matches complete
                </span>
              )}
            </p>
            {currentRound.isComplete && (
              <span className="text-xs text-green-400 flex items-center gap-1">
                <CheckCircle2 size={14} /> Round complete
              </span>
            )}
          </div>

          <div className="grid grid-cols-1 sm:grid-cols-2 md:grid-cols-3 lg:grid-cols-5 gap-3">
            {currentRound.matches.map((match) => (
              <MatchCard
                key={match.id}
                match={match}
                onClick={() => setSelectedMatch(match)}
              />
            ))}
          </div>

          {totalPlayable > completedCount && (
            <p className="text-xs text-slate-500 mt-4 text-center">
              Click a match to record the winner
            </p>
          )}
        </>
      )}

      {selectedMatch && currentRound && (
        <MatchResultModal
          match={selectedMatch}
          roundNumber={currentRound.roundNumber}
          matchFormat={matchFormat}
          onClose={() => setSelectedMatch(null)}
          onSubmit={handleResult}
        />
      )}
    </div>
  );
}

function MatchCard({
  match,
  onClick,
}: {
  match: Match;
  onClick: () => void;
}) {
  const isPending = !match.isBye && !match.isComplete;
  const isComplete = match.isComplete && !match.isBye;
  const winnerIsA = match.winnerId === match.playerAId;
  const winnerIsB = match.winnerId === match.playerBId;
  const playerAResult = getPlayerResult(match, 'A');
  const playerBResult = getPlayerResult(match, 'B');

  return (
    <button
      type="button"
      onClick={onClick}
      className={`text-left w-full rounded-xl border p-3.5 transition-colors ${
        match.isBye
          ? 'border-slate-700 bg-slate-900/60 hover:bg-slate-800/60'
          : isPending
            ? 'border-amber-500/40 bg-slate-900 hover:border-amber-500/60 hover:bg-amber-500/5'
            : isComplete
              ? 'border-green-700/40 bg-green-900/10 hover:bg-green-900/20'
              : 'border-slate-700 bg-slate-900 hover:bg-slate-800/80'
      }`}
    >
      <div className="flex items-center justify-between gap-2 mb-2.5">
        <span className="text-xs font-mono text-amber-400 font-semibold">
          Table {match.tableNumber}
        </span>
        <StatusBadge
          label={match.isBye ? 'Bye' : isComplete ? 'Complete' : 'Pending'}
          pending={isPending}
        />
      </div>

      <div className="space-y-1.5">
        <PlayerLine
          name={match.playerAName ?? 'TBD'}
          result={playerAResult}
          isWinner={winnerIsA}
          isLoser={isComplete && !winnerIsA && !match.isBye}
          isPending={isPending}
        />
        {!match.isBye && (
          <PlayerLine
            name={match.playerBName ?? 'TBD'}
            result={playerBResult}
            isWinner={winnerIsB}
            isLoser={isComplete && !winnerIsB}
            isPending={isPending}
          />
        )}
      </div>

      {isPending && (
        <p className="text-xs mt-2 text-amber-400/70 text-center">Tap result</p>
      )}
    </button>
  );
}

function getPlayerResult(match: Match, side: 'A' | 'B'): string {
  if (match.isBye && side === 'A') return 'BYE';
  if (match.isBye && side === 'B') return '';
  if (!match.isComplete || !match.result) return '—';

  const wins = side === 'A' ? match.result.playerAGameWins : match.result.playerBGameWins;
  return String(wins);
}

function PlayerLine({
  name,
  result,
  isWinner,
  isLoser,
  isPending,
}: {
  name: string;
  result: string;
  isWinner: boolean;
  isLoser: boolean;
  isPending: boolean;
}) {
  return (
    <div
      className={`flex items-center justify-between gap-2 text-sm font-medium px-2 py-1.5 rounded-lg ${
        isWinner
          ? 'bg-amber-500/25 text-amber-100'
          : isLoser
            ? 'text-slate-500'
            : 'text-slate-200 bg-slate-800/50'
      }`}
      title={name}
    >
      <span className={`truncate min-w-0 ${isLoser ? 'line-through' : ''}`}>{name}</span>
      <span
        className={`shrink-0 tabular-nums text-xs font-semibold ${
          isPending
            ? 'text-slate-600'
            : isWinner
              ? 'text-amber-300'
              : isLoser
                ? 'text-slate-600'
                : 'text-slate-400'
        }`}
      >
        {result}
      </span>
    </div>
  );
}

function StatusBadge({ label, pending }: { label: string; pending: boolean }) {
  if (label === 'Complete') {
    return (
      <span className="inline-flex items-center text-green-400" title="Complete">
        <CheckCircle2 size={12} />
      </span>
    );
  }
  if (label === 'Bye') {
    return <span className="text-xs text-slate-500">BYE</span>;
  }
  return (
    <span
      className={`inline-flex items-center ${pending ? 'text-amber-400' : 'text-slate-400'}`}
      title={label}
    >
      <Circle size={10} className={pending ? 'fill-amber-400/30' : ''} />
    </span>
  );
}