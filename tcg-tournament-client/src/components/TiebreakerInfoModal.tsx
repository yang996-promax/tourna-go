import type { ReactNode } from 'react';
import { Info, X } from 'lucide-react';

export type TiebreakerMetric = 'OMWPercent' | 'GWPercent' | 'OGWPercent';

const TIE_BREAKER_INFO: Record<TiebreakerMetric, { title: string; shortName: string; body: ReactNode }> = {
  OMWPercent: {
    title: 'Opponent Match Win % (OMW%)',
    shortName: 'OMW%',
    body: (
      <>
        <p>
          The average match win rate of everyone you played. It rewards players who faced stronger
          opponents — if your opponents kept winning, your OMW% goes up.
        </p>
        <div className="bg-slate-800/60 border border-slate-700 rounded-lg p-3 text-xs font-mono text-slate-300 space-y-1">
          <p>Opponent match win % = Match Points ÷ (Matches Played × 3)</p>
          <p>OMW% = average of all opponents&apos; match win %</p>
        </div>
        <p>
          Each opponent is counted at <strong className="text-amber-300">at least 33%</strong>. If you
          have no opponents yet, OMW% defaults to 33%.
        </p>
        <p className="text-slate-400 text-xs">
          Tiebreaker order: Match Points → OMW% → GW% → OGW%
        </p>
      </>
    ),
  },
  GWPercent: {
    title: 'Game Win % (GW%)',
    shortName: 'GW%',
    body: (
      <>
        <p>
          Your own game-level win rate across every match played. A 2–0 win counts more favorably than
          a narrow 2–1 win compared to total games played.
        </p>
        <div className="bg-slate-800/60 border border-slate-700 rounded-lg p-3 text-xs font-mono text-slate-300">
          <p>GW% = Game Wins ÷ (Game Wins + Game Losses)</p>
        </div>
        <p>
          If you have not played any games yet, GW% defaults to <strong className="text-amber-300">33%</strong>.
        </p>
        <p className="text-slate-400 text-xs">
          Tiebreaker order: Match Points → OMW% → GW% → OGW%
        </p>
      </>
    ),
  },
  OGWPercent: {
    title: 'Opponent Game Win % (OGW%)',
    shortName: 'OGW%',
    body: (
      <>
        <p>
          The average game win percentage of everyone you played. Like OMW%, it reflects how strong
          your opponents were — but measured at the individual game level instead of whole matches.
        </p>
        <div className="bg-slate-800/60 border border-slate-700 rounded-lg p-3 text-xs font-mono text-slate-300">
          <p>OGW% = average of all opponents&apos; GW%</p>
        </div>
        <p>
          If you have no opponents yet, OGW% defaults to <strong className="text-amber-300">33%</strong>.
        </p>
        <p className="text-slate-400 text-xs">
          Tiebreaker order: Match Points → OMW% → GW% → OGW%
        </p>
      </>
    ),
  },
};

interface Props {
  metric: TiebreakerMetric;
  onClose: () => void;
}

export function TiebreakerInfoButton({
  metric,
  onOpen,
}: {
  metric: TiebreakerMetric;
  onOpen: (metric: TiebreakerMetric) => void;
}) {
  const { shortName } = TIE_BREAKER_INFO[metric];

  return (
    <button
      type="button"
      onClick={() => onOpen(metric)}
      className="inline-flex items-center gap-1 text-slate-400 hover:text-amber-300 transition-colors"
      aria-label={`What is ${shortName}?`}
      title={`What is ${shortName}?`}
    >
      <span>{shortName}</span>
      <Info size={13} className="shrink-0 opacity-70" />
    </button>
  );
}

export default function TiebreakerInfoModal({ metric, onClose }: Props) {
  const info = TIE_BREAKER_INFO[metric];

  return (
    <div
      className="fixed inset-0 z-50 flex items-center justify-center p-4 bg-black/60 backdrop-blur-sm"
      onClick={onClose}
    >
      <div
        className="bg-slate-900 border border-slate-700 rounded-2xl w-full max-w-md shadow-2xl"
        onClick={(e) => e.stopPropagation()}
        role="dialog"
        aria-modal="true"
        aria-labelledby="tiebreaker-info-title"
      >
        <div className="flex items-center justify-between p-5 border-b border-slate-800">
          <div className="flex items-center gap-3">
            <div className="p-2 bg-amber-500/15 rounded-lg">
              <Info className="text-amber-400" size={20} />
            </div>
            <h3 id="tiebreaker-info-title" className="text-lg font-semibold text-slate-100">
              {info.title}
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

        <div className="p-5 text-sm text-slate-300 space-y-3 leading-relaxed">{info.body}</div>

        <div className="flex justify-end p-5 border-t border-slate-800">
          <button type="button" onClick={onClose} className="btn-secondary">
            Got it
          </button>
        </div>
      </div>
    </div>
  );
}