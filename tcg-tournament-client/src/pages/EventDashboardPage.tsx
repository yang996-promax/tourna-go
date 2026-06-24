import { useEffect, useState } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { tournamentApi } from '../api/client';
import { useEvent } from '../context/EventContext';
import StartTournamentModal from '../components/StartTournamentModal';
import { Calendar, GitBranch, Layers, MapPin, Swords, Trophy, Users } from 'lucide-react';

export default function EventDashboardPage() {
  const { tournament, refresh } = useEvent();
  const navigate = useNavigate();
  const [roundStats, setRoundStats] = useState({ completed: 0, total: 0, isLastRound: false, allComplete: false });
  const [endingSwiss, setEndingSwiss] = useState(false);
  const [swissError, setSwissError] = useState('');
  const [showStartModal, setShowStartModal] = useState(false);

  const loadRounds = async () => {
    if (!tournament) return;
    const rounds = await tournamentApi.getRounds(tournament.id, true);
    const lastRound = rounds.find((r) => r.roundNumber === tournament.totalSwissRounds);
    const current = rounds.find((r) => r.roundNumber === tournament.currentRound) ?? rounds[rounds.length - 1];

    if (current) {
      const completed = current.matches.filter((m) => m.isComplete).length;
      const total = current.matches.length;
      setRoundStats({
        completed,
        total,
        isLastRound: tournament.currentRound >= tournament.totalSwissRounds,
        allComplete: total > 0 && completed === total && (lastRound?.isComplete ?? false),
      });
    } else {
      setRoundStats({ completed: 0, total: 0, isLastRound: false, allComplete: false });
    }
  };

  useEffect(() => { loadRounds(); }, [tournament]);

  if (!tournament) return null;

  const handleStart = async () => {
    await tournamentApi.start(tournament.id);
    await refresh();
  };

  const handleGenerateRound = async () => {
    await tournamentApi.generateRound(tournament.id);
    await refresh();
    await loadRounds();
  };

  const handleEndSwissAndStartTopCut = async () => {
    setEndingSwiss(true);
    setSwissError('');
    try {
      await tournamentApi.completeSwiss(tournament.id);
      await refresh();
      navigate('topcut');
    } catch (err: unknown) {
      const msg = (err as { response?: { data?: { message?: string } } })?.response?.data?.message;
      setSwissError(msg ?? 'Could not end Swiss rounds.');
    } finally {
      setEndingSwiss(false);
    }
  };

  const hasTopCut = tournament.topCutSize > 0;
  const swissFinished = tournament.status === 'SwissComplete' || tournament.status === 'TopCutInProgress' || tournament.status === 'Completed';
  const canEndSwiss = hasTopCut
    && tournament.status === 'InProgress'
    && tournament.currentRound >= tournament.totalSwissRounds
    && roundStats.allComplete;
  const needsLastRoundResults = hasTopCut
    && tournament.status === 'InProgress'
    && tournament.currentRound >= tournament.totalSwissRounds
    && !roundStats.allComplete;

  const quickLinks = [
    { to: 'players', label: 'Players', desc: `${tournament.playerCount} registered`, icon: Users },
    { to: 'pairings', label: 'Pairings', desc: `Round ${tournament.currentRound || '—'}`, icon: Swords },
    { to: 'standings', label: 'Standings', desc: 'Live rankings', icon: Trophy },
    { to: 'topcut', label: 'Top Cut', desc: tournament.topCutSize ? `Top ${tournament.topCutSize}` : 'None', icon: Layers },
  ];

  return (
    <div>
      <h3 className="text-lg font-semibold mb-4">Event Overview</h3>

      <div className="grid grid-cols-2 md:grid-cols-4 gap-4 mb-8">
        <Stat icon={Calendar} label="Event Date" value={new Date(tournament.eventDate).toLocaleDateString()} />
        <Stat icon={MapPin} label="Venue" value={tournament.venue} />
        <Stat icon={Users} label="Players" value={String(tournament.playerCount)} />
        <Stat icon={Layers} label="Swiss Round" value={`${tournament.currentRound} / ${tournament.totalSwissRounds}`} />
      </div>

      {tournament.currentRound > 0 && (
        <p className="text-sm text-slate-400 mb-4">
          {roundStats.isLastRound ? 'Final Swiss round' : 'Current round'} matches completed: {roundStats.completed} / {roundStats.total}
        </p>
      )}

      {needsLastRoundResults && (
        <div className="bg-amber-500/10 border border-amber-500/30 rounded-lg px-4 py-3 mb-4 text-sm text-amber-200">
          Enter all match results in <Link to="pairings" className="underline font-medium">Pairings</Link> before starting top cut.
          ({roundStats.total - roundStats.completed} match{roundStats.total - roundStats.completed !== 1 ? 'es' : ''} remaining)
        </div>
      )}

      {swissError && (
        <div className="bg-red-900/20 border border-red-700/50 text-red-300 rounded-lg px-4 py-3 mb-4 text-sm">
          {swissError}
        </div>
      )}

      <div className="flex flex-wrap gap-3 mb-8">
        {tournament.status === 'Registration' && (
          <button onClick={() => setShowStartModal(true)} className="btn-primary">Start Tournament</button>
        )}
        {tournament.status === 'InProgress' && tournament.currentRound < tournament.totalSwissRounds && (
          <button onClick={handleGenerateRound} className="btn-primary">Generate Round {tournament.currentRound + 1}</button>
        )}
        {canEndSwiss && (
          <button onClick={handleEndSwissAndStartTopCut} disabled={endingSwiss} className="btn-primary flex items-center gap-2">
            <GitBranch size={16} />
            {endingSwiss ? 'Ending Swiss...' : `End Swiss & Start Top ${tournament.topCutSize}`}
          </button>
        )}
        {swissFinished && hasTopCut && tournament.status !== 'Completed' && (
          <Link to="topcut" className="btn-primary flex items-center gap-2">
            <GitBranch size={16} /> Go to Top Cut
          </Link>
        )}
      </div>

      <h4 className="text-sm font-medium text-slate-400 mb-3">Quick Access</h4>
      <div className="grid sm:grid-cols-2 lg:grid-cols-4 gap-4">
        {quickLinks.map(({ to, label, desc, icon: Icon }) => (
          <Link
            key={to}
            to={to}
            className="bg-slate-900 border border-slate-800 hover:border-amber-500/30 rounded-xl p-4 transition-colors"
          >
            <Icon className="text-amber-400 mb-2" size={22} />
            <p className="font-medium">{label}</p>
            <p className="text-sm text-slate-500">{desc}</p>
          </Link>
        ))}
      </div>

      {showStartModal && (
        <StartTournamentModal
          tournament={tournament}
          onClose={() => setShowStartModal(false)}
          onConfirm={handleStart}
        />
      )}
    </div>
  );
}

function Stat({ icon: Icon, label, value }: { icon: typeof Calendar; label: string; value: string }) {
  return (
    <div className="bg-slate-800/50 rounded-lg p-3">
      <div className="flex items-center gap-2 text-slate-400 text-xs mb-1">
        <Icon size={14} /> {label}
      </div>
      <p className="font-medium truncate">{value}</p>
    </div>
  );
}