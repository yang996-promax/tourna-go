import { BrowserRouter, Navigate, Route, Routes } from 'react-router-dom';
import { AuthProvider, useAuth } from './context/AuthContext';
import Layout from './components/Layout';
import EventLayout from './components/EventLayout';
import LoginPage from './pages/LoginPage';
import DashboardPage from './pages/DashboardPage';
import EventDashboardPage from './pages/EventDashboardPage';
import PlayersPage from './pages/PlayersPage';
import PairingsPage from './pages/PairingsPage';
import StandingsPage from './pages/StandingsPage';
import TopCutPage from './pages/TopCutPage';

function ProtectedRoute({ children }: { children: React.ReactNode }) {
  const { isAuthenticated } = useAuth();
  return isAuthenticated ? <>{children}</> : <Navigate to="/login" replace />;
}

export default function App() {
  return (
    <AuthProvider>
      <BrowserRouter>
        <Routes>
          <Route path="/login" element={<LoginPage />} />
          <Route
            element={
              <ProtectedRoute>
                <Layout />
              </ProtectedRoute>
            }
          >
            <Route path="/" element={<DashboardPage />} />
            <Route path="/events/:eventId" element={<EventLayout />}>
              <Route index element={<EventDashboardPage />} />
              <Route path="players" element={<PlayersPage />} />
              <Route path="pairings" element={<PairingsPage />} />
              <Route path="standings" element={<StandingsPage />} />
              <Route path="topcut" element={<TopCutPage />} />
            </Route>
          </Route>
        </Routes>
      </BrowserRouter>
    </AuthProvider>
  );
}