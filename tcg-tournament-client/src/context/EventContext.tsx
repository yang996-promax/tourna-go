import { createContext, useContext, useEffect, useState, type ReactNode } from 'react';
import { useParams } from 'react-router-dom';
import { tournamentApi } from '../api/client';
import type { Tournament } from '../types';

interface EventContextType {
  tournament: Tournament | null;
  loading: boolean;
  refresh: () => Promise<void>;
}

const EventContext = createContext<EventContextType | null>(null);

export function EventProvider({ children }: { children: ReactNode }) {
  const { eventId } = useParams<{ eventId: string }>();
  const [tournament, setTournament] = useState<Tournament | null>(null);
  const [loading, setLoading] = useState(true);

  const refresh = async () => {
    if (!eventId) return;
    const t = await tournamentApi.getById(parseInt(eventId));
    setTournament(t);
  };

  useEffect(() => {
    if (!eventId) return;
    setLoading(true);
    tournamentApi.getById(parseInt(eventId))
      .then(setTournament)
      .finally(() => setLoading(false));
  }, [eventId]);

  return (
    <EventContext.Provider value={{ tournament, loading, refresh }}>
      {children}
    </EventContext.Provider>
  );
}

export function useEvent() {
  const ctx = useContext(EventContext);
  if (!ctx) throw new Error('useEvent must be used within EventProvider');
  return ctx;
}