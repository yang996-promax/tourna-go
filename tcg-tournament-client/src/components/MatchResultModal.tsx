import { useState } from 'react';
import { Loader2, Trophy, X } from 'lucide-react';
import type { Match, MatchFormat, MatchResultType } from '../types';

const RESULTS_BO3: { value: MatchResultType; label: string; side: 'A' | 'B' | 'draw' }[] = [
  { value: 'Win2_0', label: '2-0', side: 'A' },
  { value: 'Win2_1', label: '2-1', side: 'A' },
  { value: 'Loss1_2', label: '1-2', side: 'B' },
  { value: 'Loss0_2', label: '0-2', side: 'B' },
  { value: 'Draw', label: 'Draw', side: 'draw' },
];

const RESULTS_BO1: { value: MatchResultType; label: string; side: 'A' | 'B' | 'draw' }[] = [
  { value: 'Win2_0', label: '1-0', side: 'A' },
  { value: 'Loss0_2', label: '0-1', side: 'B' },
  { value: 'Draw', label: 'Draw', side: 'draw' },
];

function getResultOptions(format: MatchFormat) {
  return format === 'BO1' ? RESULTS_BO1 : RESULTS_BO3;
}

function formatResultScore(match: Match, format: MatchFormat): string {
  if (match.isBye) return 'BYE';
  if (!match.result) return '—';
  const { playerAGameWins, playerBGameWins } = match.result;
  if (format === 'BO1') return `${playerAGameWins}-${playerBGameWins}`;
  return `${playerAGameWins}-${playerBGameWins}`;
}

interface Props {
  match: Match;
  roundNumber: number;
  matchFormat: MatchFormat;
  onClose: () => void;
  onSubmit: (resultType: MatchResultType) => Promise<void>;
}

export default function MatchResultModal({ match, roundNumber, matchFormat, onClose, onSubmit }: Props) {
  const [submitting, setSubmitting] = useState(false);
  const [error, setError] = useState('');
  const options = getResultOptions(matchFormat);
  const playerAOptions = options.filter((o) => o.side === 'A');
  const playerBOptions = options.filter((o) => o.side === 'B');
  const drawOption = options.find((o) => o.side === 'draw');
  const canEnter = !match.isBye && !match.isComplete && !submitting;

  const handleSelect = async (resultType: MatchResultType) => {
    if (!canEnter) return;
    setSubmitting(true);
    setError('');
    try {
      await onSubmit(resultType);
      onClose();
    } catch (err: unknown) {
      const msg = (err as { response?: { data?: { message?: string } } })?.response?.data?.message;
      setError(msg ?? 'Failed to save result.');
    } finally {
      setSubmitting(false);
    }
  };

  const winnerName =
    match.winnerId === match.playerAId
      ? match.playerAName
      : match.winnerId === match.playerBId
        ? match.playerBName
        : match.result?.resultType === 'Draw'
          ? 'Draw'
          : null;

  return (
    <div
      className="fixed inset-0 z-50 flex items-center justify-center p-4 bg-black/60 backdrop-blur-sm"
      onClick={onClose}
    >
      <div
        className="bg-slate-900 border border-slate-700 rounded-2xl w-full max-w-lg shadow-2xl"
        onClick={(e) => e.stopPropagation()}
        role="dialog"
        aria-modal="true"
        aria-labelledby="match-result-title"
      >
        <div className="flex items-center justify-between p-5 border-b border-slate-800">
          <div>
            <p className="text-xs text-amber-400 uppercase tracking-wide font-medium">
              Round {roundNumber} · Table {match.tableNumber}
            </p>
            <h3 id="match-result-title" className="text-lg font-semibold text-slate-100 mt-0.5">
              {match.isComplete ? 'Match Result' : 'Record Winner'}
            </h3>
          </div>
          <button
            type="button"
            onClick={onClose}
            className="text-slate-400 hover:text-slate-200 p-1"
            aria-label="Close"
          >
            <X size={20} />
          </button>
        </div>

        <div className="p-5">
          {match.isBye ? (
            <p className="text-slate-300 text-center py-4">
              <span className="font-semibold text-amber-300">{match.playerAName}</span> receives a bye.
            </p>
          ) : match.isComplete ? (
            <div className="text-center py-4 space-y-3">
              <div className="flex items-center justify-center gap-6">
                <p className="font-semibold text-lg">{match.playerAName}</p>
                <span className="text-amber-500 font-bold">{formatResultScore(match, matchFormat)}</span>
                <p className="font-semibold text-lg">{match.playerBName}</p>
              </div>
              {winnerName && winnerName !== 'Draw' && (
                <p className="text-sm text-green-400 flex items-center justify-center gap-2">
                  <Trophy size={16} />
                  Winner: {winnerName}
                </p>
              )}
              {winnerName === 'Draw' && (
                <p className="text-sm text-slate-400">Match drawn</p>
              )}
            </div>
          ) : (
            <>
              <p className="text-sm text-slate-400 text-center mb-5">Select the winner and score</p>

              <div className="grid grid-cols-2 gap-4">
                <PlayerWinnerCard
                  name={match.playerAName ?? 'TBD'}
                  options={playerAOptions}
                  disabled={!canEnter}
                  onSelect={handleSelect}
                />
                <PlayerWinnerCard
                  name={match.playerBName ?? 'TBD'}
                  options={playerBOptions}
                  disabled={!canEnter}
                  onSelect={handleSelect}
                />
              </div>

              {drawOption && (
                <button
                  type="button"
                  disabled={!canEnter}
                  onClick={() => handleSelect(drawOption.value)}
                  className="w-full mt-4 py-3 rounded-xl border border-slate-600 bg-slate-800 hover:bg-slate-700 text-slate-200 text-sm font-medium transition-colors disabled:opacity-50 disabled:cursor-not-allowed"
                >
                  {drawOption.label}
                </button>
              )}

              {error && (
                <p className="text-sm text-red-400 text-center mt-4">{error}</p>
              )}
            </>
          )}
        </div>

        <div className="flex justify-end p-5 border-t border-slate-800">
          {submitting ? (
            <span className="flex items-center gap-2 text-sm text-slate-400">
              <Loader2 className="animate-spin" size={16} /> Saving...
            </span>
          ) : (
            <button type="button" onClick={onClose} className="btn-secondary">
              {match.isComplete || match.isBye ? 'Close' : 'Cancel'}
            </button>
          )}
        </div>
      </div>
    </div>
  );
}

function PlayerWinnerCard({
  name,
  options,
  disabled,
  onSelect,
}: {
  name: string;
  options: { value: MatchResultType; label: string }[];
  disabled: boolean;
  onSelect: (resultType: MatchResultType) => void;
}) {
  return (
    <div className="bg-slate-800/50 border border-slate-700 rounded-xl p-4 text-center">
      <p className="font-semibold text-slate-100 mb-3 truncate" title={name}>
        {name}
      </p>
      <div className="flex flex-col gap-2">
        {options.map((opt) => (
          <button
            key={opt.value}
            type="button"
            disabled={disabled}
            onClick={() => onSelect(opt.value)}
            className="py-2.5 rounded-lg bg-amber-500/15 hover:bg-amber-500/30 border border-amber-500/30 text-amber-200 font-semibold text-sm transition-colors disabled:opacity-50 disabled:cursor-not-allowed"
          >
            {opt.label}
          </button>
        ))}
      </div>
    </div>
  );
}

export function formatMatchResultLabel(match: Match, format: MatchFormat): string {
  if (match.isBye) return 'BYE';
  if (!match.isComplete || !match.result) return 'Pending';
  if (match.result.resultType === 'Draw') return 'Draw';
  const score = formatResultScore(match, format);
  const winner =
    match.winnerId === match.playerAId
      ? match.playerAName
      : match.winnerId === match.playerBId
        ? match.playerBName
        : null;
  return winner ? `${winner} (${score})` : score;
}