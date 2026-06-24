import { useEffect, useState } from 'react';
import { tournamentApi } from '../api/client';
import TiebreakerInfoModal, { TiebreakerInfoButton, type TiebreakerMetric } from '../components/TiebreakerInfoModal';
import { useEvent } from '../context/EventContext';
import type { Standing, StandingPhase, StandingSortBy } from '../types';

type StandingView = 'beforeTopCut' | 'afterTopCut';

function viewToPhase(view: StandingView): StandingPhase {
  return view === 'beforeTopCut' ? 'Swiss' : 'Overall';
}

function defaultView(tournament: { topCutSize: number; status: string }): StandingView {
  if (tournament.topCutSize <= 0) return 'beforeTopCut';
  if (tournament.status === 'TopCutInProgress' || tournament.status === 'Completed') {
    return 'afterTopCut';
  }
  return 'beforeTopCut';
}

export default function StandingsPage() {
  const { tournament } = useEvent();
  const [standings, setStandings] = useState<Standing[]>([]);
  const [sortBy, setSortBy] = useState<StandingSortBy>('MatchPoints');
  const [view, setView] = useState<StandingView>('beforeTopCut');
  const [infoMetric, setInfoMetric] = useState<TiebreakerMetric | null>(null);

  const hasTopCut = (tournament?.topCutSize ?? 0) > 0;

  const load = async (
    sort: StandingSortBy = sortBy,
    standingView: StandingView = view,
  ) => {
    if (!tournament) return;
    setStandings(
      await tournamentApi.getStandings(tournament.id, sort, viewToPhase(standingView)),
    );
  };

  useEffect(() => {
    if (!tournament) return;
    setView(defaultView(tournament));
  }, [tournament?.id]);

  useEffect(() => {
    if (!tournament) return;
    load(sortBy, view);
  }, [tournament?.id, tournament?.status, view]);

  const handleSort = (sort: StandingSortBy) => {
    setSortBy(sort);
    load(sort, view);
  };

  const handleViewChange = (nextView: StandingView) => {
    setView(nextView);
    load(sortBy, nextView);
  };

  if (!tournament) return null;

  const highlightCount = tournament.topCutSize || 8;
  const showTopCutHighlight = view === 'beforeTopCut' && hasTopCut;

  return (
    <div>
      <div className="flex flex-wrap justify-between items-start gap-4 mb-6">
        <div>
          <h3 className="text-lg font-semibold">Standings</h3>
          {hasTopCut && (
            <p className="text-xs text-slate-500 mt-1">
              {view === 'beforeTopCut'
                ? `Swiss results — Top ${tournament.topCutSize} highlighted`
                : 'Swiss + top cut match results included'}
            </p>
          )}
        </div>

        <div className="flex flex-wrap items-center gap-2">
          {hasTopCut && (
            <div className="flex rounded-lg overflow-hidden border border-slate-700">
              <button
                type="button"
                onClick={() => handleViewChange('beforeTopCut')}
                className={`px-3 py-1.5 text-xs font-medium transition-colors ${
                  view === 'beforeTopCut'
                    ? 'bg-amber-500 text-slate-900'
                    : 'bg-slate-800 text-slate-300 hover:bg-slate-700'
                }`}
              >
                Before Top Cut
              </button>
              <button
                type="button"
                onClick={() => handleViewChange('afterTopCut')}
                className={`px-3 py-1.5 text-xs font-medium transition-colors ${
                  view === 'afterTopCut'
                    ? 'bg-amber-500 text-slate-900'
                    : 'bg-slate-800 text-slate-300 hover:bg-slate-700'
                }`}
              >
                After Top Cut
              </button>
            </div>
          )}

          {(['MatchPoints', 'OMWPercent', 'GWPercent', 'OGWPercent'] as StandingSortBy[]).map((s) => (
            <button
              key={s}
              onClick={() => handleSort(s)}
              className={`px-3 py-1 rounded text-xs ${
                sortBy === s ? 'bg-amber-500 text-slate-900' : 'bg-slate-800 text-slate-300'
              }`}
            >
              {s.replace('Percent', '%')}
            </button>
          ))}
        </div>
      </div>

      <div className="overflow-x-auto">
        <table className="w-full text-sm">
          <thead>
            <tr className="text-slate-400 border-b border-slate-700">
              <th className="text-left py-2 px-2">Rank</th>
              <th className="text-left py-2 px-2">Player</th>
              <th className="text-center py-2 px-2">MP</th>
              <th className="text-center py-2 px-2">W-L</th>
              <th className="text-center py-2 px-2">
                <TiebreakerInfoButton metric="OMWPercent" onOpen={setInfoMetric} />
              </th>
              <th className="text-center py-2 px-2">
                <TiebreakerInfoButton metric="GWPercent" onOpen={setInfoMetric} />
              </th>
              <th className="text-center py-2 px-2">
                <TiebreakerInfoButton metric="OGWPercent" onOpen={setInfoMetric} />
              </th>
            </tr>
          </thead>
          <tbody>
            {standings.map((s) => (
              <tr
                key={s.tournamentPlayerId}
                className={`border-b border-slate-800 ${
                  showTopCutHighlight && s.rank <= highlightCount ? 'bg-amber-500/5' : ''
                }`}
              >
                <td className="py-3 px-2 font-bold text-amber-400">{s.rank}</td>
                <td className="py-3 px-2">
                  <span className="text-slate-500 mr-2">#{s.playerNumber}</span>
                  {s.playerName}
                </td>
                <td className="py-3 px-2 text-center font-semibold">{s.matchPoints}</td>
                <td className="py-3 px-2 text-center text-slate-400">{s.matchesWon}-{s.matchesLost}</td>
                <td className="py-3 px-2 text-center">{s.omwPercent.toFixed(1)}%</td>
                <td className="py-3 px-2 text-center">{s.gwPercent.toFixed(1)}%</td>
                <td className="py-3 px-2 text-center">{s.ogwPercent.toFixed(1)}%</td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>

      {infoMetric && (
        <TiebreakerInfoModal metric={infoMetric} onClose={() => setInfoMetric(null)} />
      )}
    </div>
  );
}