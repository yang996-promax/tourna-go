export type TournamentStatus = 'Draft' | 'Registration' | 'InProgress' | 'SwissComplete' | 'TopCutInProgress' | 'Completed';
export type TopCutSize = 'None' | 'Top4' | 'Top8' | 'Top16';
export type FirstRoundPairingMode = 'Random' | 'Seeded';
export type MatchFormat = 'BO1' | 'BO3';
export type MatchResultType = 'Win2_0' | 'Win2_1' | 'Loss1_2' | 'Loss0_2' | 'Draw' | 'ByeWin';
export type StandingSortBy = 'MatchPoints' | 'OMWPercent' | 'GWPercent' | 'OGWPercent';
export type StandingPhase = 'Swiss' | 'Overall';
export type TopCutRound = 'RoundOf16' | 'QuarterFinal' | 'SemiFinal' | 'Final';

export interface Tournament {
  id: number;
  name: string;
  gameTitle: string;
  eventDate: string;
  organizer: string;
  venue: string;
  totalSwissRounds: number;
  topCutSize: number;
  firstRoundPairingMode: FirstRoundPairingMode;
  matchFormat: MatchFormat;
  hasElimination: boolean;
  eliminationLossCount?: number | null;
  status: TournamentStatus;
  currentRound: number;
  playerCount: number;
  createdAt: string;
}

export interface Dashboard {
  activeTournament: Tournament | null;
  totalPlayers: number;
  currentRound: number;
  completedMatches: number;
  totalMatches: number;
  status: string;
}

export interface Player {
  id: number;
  tournamentPlayerId: number;
  externalPlayerId: string;
  name: string;
  contactNumber?: string;
  deckName?: string;
  playerNumber: number;
  isDropped: boolean;
}

export interface Match {
  id: number;
  roundId: number;
  roundNumber: number;
  tableNumber: number;
  playerAId?: number;
  playerAName?: string;
  playerBId?: number;
  playerBName?: string;
  isBye: boolean;
  isComplete: boolean;
  winnerId?: number;
  result?: {
    resultType: MatchResultType;
    playerAGameWins: number;
    playerBGameWins: number;
    playerAMatchPoints: number;
    playerBMatchPoints: number;
  };
}

export type RoundType = 'Swiss' | 'TopCut';

export interface Round {
  id: number;
  roundNumber: number;
  roundType: RoundType;
  isComplete: boolean;
  matches: Match[];
}

export interface Standing {
  rank: number;
  tournamentPlayerId: number;
  playerName: string;
  playerNumber: number;
  matchPoints: number;
  gameWins: number;
  gameLosses: number;
  omwPercent: number;
  gwPercent: number;
  ogwPercent: number;
  matchesPlayed: number;
  matchesWon: number;
  matchesLost: number;
  matchesDrawn: number;
}

export interface TopCutBracket {
  id: number;
  round: TopCutRound;
  matchPosition: number;
  playerAId?: number;
  playerAName?: string;
  playerBId?: number;
  playerBName?: string;
  winnerId?: number;
  winnerName?: string;
  nextBracketId?: number;
  isComplete: boolean;
  isPlayable: boolean;
  matchId?: number;
}

export interface TopCutQualifiedPlayer {
  seed: number;
  tournamentPlayerId: number;
  playerName: string;
  playerNumber: number;
  matchPoints: number;
}

export interface TopCutTree {
  brackets: TopCutBracket[];
  qualifiedPlayers: TopCutQualifiedPlayer[];
  championName?: string;
  championPlayerId?: number;
  canGenerate: boolean;
  statusMessage?: string;
}

export interface LoginResponse {
  token: string;
  username: string;
  displayName: string;
  expiresAt: string;
}