import axios from 'axios';
import type {
  Dashboard,
  LoginResponse,
  Match,
  MatchResultType,
  Player,
  Round,
  Standing,
  StandingPhase,
  StandingSortBy,
  TopCutTree,
  Tournament,
} from '../types';

const api = axios.create({ baseURL: '/api' });

api.interceptors.request.use((config) => {
  const token = localStorage.getItem('token');
  if (token) config.headers.Authorization = `Bearer ${token}`;
  return config;
});

api.interceptors.response.use(
  (response) => response,
  (error) => {
    if (error.response?.status === 401 && !error.config?.url?.includes('/auth/login')) {
      localStorage.removeItem('token');
      localStorage.removeItem('username');
      localStorage.removeItem('displayName');
      if (window.location.pathname !== '/login') {
        window.location.href = '/login';
      }
    }
    return Promise.reject(error);
  },
);

export const authApi = {
  login: (username: string, password: string) =>
    api.post<LoginResponse>('/auth/login', { username, password }).then((r) => r.data),
};

export const tournamentApi = {
  getDashboard: () => api.get<Dashboard>('/tournaments/dashboard').then((r) => r.data),
  getAll: () => api.get<Tournament[]>('/tournaments').then((r) => r.data),
  getById: (id: number) => api.get<Tournament>(`/tournaments/${id}`).then((r) => r.data),
  create: (data: Record<string, unknown>) => api.post<Tournament>('/tournaments', data).then((r) => r.data),
  delete: (id: number) => api.delete(`/tournaments/${id}`),
  start: (id: number) => api.post(`/tournaments/${id}/start`),
  generateRound: (id: number, firstRoundPairingMode?: string) =>
    api.post<Round>(`/tournaments/${id}/generate-round`, { firstRoundPairingMode }).then((r) => r.data),
  getRounds: (id: number, swissOnly = false) =>
    api.get<Round[]>(`/tournaments/${id}/rounds`, { params: { swissOnly } }).then((r) => r.data),
  getStandings: (
    id: number,
    sortBy: StandingSortBy = 'MatchPoints',
    phase: StandingPhase = 'Overall',
  ) =>
    api.get<Standing[]>(`/tournaments/${id}/standings`, { params: { sortBy, phase } }).then((r) => r.data),
  completeSwiss: (id: number) => api.post(`/tournaments/${id}/complete-swiss`),
  generateTopCut: (id: number) => api.post<TopCutTree>(`/tournaments/${id}/generate-topcut`).then((r) => r.data),
  getTopCut: (id: number) => api.get<TopCutTree>(`/tournaments/${id}/topcut`).then((r) => r.data),
};

export const playerApi = {
  getAll: (tournamentId: number) =>
    api.get<Player[]>(`/tournaments/${tournamentId}/players`).then((r) => r.data),
  search: (tournamentId: number, q: string) =>
    api.get<Player[]>(`/tournaments/${tournamentId}/players/search`, { params: { q } }).then((r) => r.data),
  add: (tournamentId: number, data: Record<string, unknown>) =>
    api.post<Player>(`/tournaments/${tournamentId}/players`, data).then((r) => r.data),
  update: (tournamentId: number, tournamentPlayerId: number, data: Record<string, unknown>) =>
    api.put<Player>(`/tournaments/${tournamentId}/players/${tournamentPlayerId}`, data).then((r) => r.data),
  remove: (tournamentId: number, tournamentPlayerId: number) =>
    api.delete(`/tournaments/${tournamentId}/players/${tournamentPlayerId}`),
  importCsv: (tournamentId: number, file: File) => {
    const form = new FormData();
    form.append('file', file);
    return api.post(`/tournaments/${tournamentId}/players/import`, form);
  },
  exportCsv: (tournamentId: number) =>
    api.get(`/tournaments/${tournamentId}/players/export`, { responseType: 'blob' }),
};

export const matchApi = {
  enterResult: (matchId: number, resultType: MatchResultType) =>
    api.post<Match>(`/matches/${matchId}/result`, { resultType }).then((r) => r.data),
  enterTopCutResult: (bracketId: number, winnerId: number) =>
    api.post(`/matches/topcut/${bracketId}/result`, { winnerId }),
};