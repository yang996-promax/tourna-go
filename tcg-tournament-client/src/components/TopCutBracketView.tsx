import type { TopCutBracket, TopCutRound } from '../types';

const ROUND_ORDER: TopCutRound[] = ['RoundOf16', 'QuarterFinal', 'SemiFinal', 'Final'];

function isBracketPlayable(bracket: TopCutBracket): boolean {
  return bracket.isPlayable ?? (!bracket.isComplete && !!bracket.playerAId && !!bracket.playerBId);
}

const ROUND_LABELS: Record<TopCutRound, string> = {
  RoundOf16: 'Round of 16',
  QuarterFinal: 'Quarterfinals',
  SemiFinal: 'Semifinals',
  Final: 'Final',
};

interface Props {
  brackets: TopCutBracket[];
  onSelectWinner: (bracket: TopCutBracket, winnerId: number) => void;
  submittingId?: number | null;
}

export default function TopCutBracketView({ brackets, onSelectWinner, submittingId }: Props) {
  const rounds = ROUND_ORDER.filter((r) => brackets.some((b) => b.round === r));

  return (
    <div className="overflow-x-auto pb-4">
      <div className="flex gap-6 min-w-max items-stretch">
        {rounds.map((round, roundIdx) => {
          const roundBrackets = brackets
            .filter((b) => b.round === round)
            .sort((a, b) => a.matchPosition - b.matchPosition);

          return (
            <div key={round} className="flex flex-col min-w-[220px]">
              <h4 className="text-center text-sm font-semibold text-amber-400 mb-4 uppercase tracking-wide">
                {ROUND_LABELS[round]}
              </h4>
              <div
                className="flex flex-col justify-around flex-1 gap-4"
                style={{ minHeight: roundBrackets.length * 120 }}
              >
                {roundBrackets.map((bracket) => (
                  <BracketMatch
                    key={bracket.id}
                    bracket={bracket}
                    isLastRound={roundIdx === rounds.length - 1}
                    onSelectWinner={onSelectWinner}
                    submitting={submittingId === bracket.id}
                  />
                ))}
              </div>
            </div>
          );
        })}
      </div>
    </div>
  );
}

function BracketMatch({
  bracket,
  isLastRound,
  onSelectWinner,
  submitting,
}: {
  bracket: TopCutBracket;
  isLastRound: boolean;
  onSelectWinner: (bracket: TopCutBracket, winnerId: number) => void;
  submitting: boolean;
}) {
  const playable = isBracketPlayable(bracket);
  const canPlay = playable && !submitting;

  return (
    <div
      className={`relative border rounded-xl p-3 transition-colors ${
        bracket.isComplete
          ? 'border-green-700/50 bg-green-900/10'
          : playable
            ? 'border-amber-500/40 bg-slate-900 shadow-lg shadow-amber-500/5'
            : 'border-slate-700 bg-slate-900/60'
      }`}
    >
      {!isLastRound && (
        <div className="absolute -right-3 top-1/2 w-3 h-px bg-slate-600 hidden lg:block" />
      )}

      <p className="text-[10px] text-slate-500 mb-2">Match {bracket.matchPosition}</p>

      <PlayerSlot
        name={bracket.playerAName}
        isWinner={bracket.winnerId === bracket.playerAId}
        isLoser={bracket.isComplete && bracket.winnerId !== bracket.playerAId}
        canSelect={canPlay && !!bracket.playerAId}
        onSelect={() => bracket.playerAId && onSelectWinner(bracket, bracket.playerAId)}
      />

      <div className="text-center text-[10px] text-slate-600 my-1">vs</div>

      <PlayerSlot
        name={bracket.playerBName}
        isWinner={bracket.winnerId === bracket.playerBId}
        isLoser={bracket.isComplete && bracket.winnerId !== bracket.playerBId}
        canSelect={canPlay && !!bracket.playerBId}
        onSelect={() => bracket.playerBId && onSelectWinner(bracket, bracket.playerBId)}
      />

      {playable && (
        <p className="text-[10px] text-amber-400/80 text-center mt-2">Click winner to advance</p>
      )}
      {submitting && (
        <p className="text-[10px] text-slate-400 text-center mt-2">Saving...</p>
      )}
    </div>
  );
}

function PlayerSlot({
  name,
  isWinner,
  isLoser,
  canSelect,
  onSelect,
}: {
  name?: string;
  isWinner: boolean;
  isLoser: boolean;
  canSelect: boolean;
  onSelect: () => void;
}) {
  const displayName = name ?? 'TBD';

  return (
    <button
      type="button"
      disabled={!canSelect}
      onClick={onSelect}
      className={`w-full text-left px-2.5 py-2 rounded-lg text-sm transition-colors ${
        isWinner
          ? 'bg-amber-500/30 text-amber-100 font-semibold ring-1 ring-amber-500/50'
          : isLoser
            ? 'bg-slate-800/30 text-slate-500 line-through'
            : canSelect
              ? 'bg-slate-800 hover:bg-amber-500/20 hover:text-amber-200 cursor-pointer'
              : name
                ? 'bg-slate-800/50 text-slate-300'
                : 'bg-slate-800/30 text-slate-600 italic'
      }`}
    >
      {displayName}
    </button>
  );
}