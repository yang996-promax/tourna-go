import { createContext, useContext, useState, type ReactNode } from 'react';
import { authApi } from '../api/client';

interface AuthContextType {
  token: string | null;
  username: string | null;
  displayName: string | null;
  orgCD: string | null;
  login: (username: string, password: string) => Promise<void>;
  logout: () => void;
  isAuthenticated: boolean;
}

const AuthContext = createContext<AuthContextType | null>(null);

export function AuthProvider({ children }: { children: ReactNode }) {
  const [token, setToken] = useState(localStorage.getItem('token'));
  const [username, setUsername] = useState(localStorage.getItem('username'));
  const [displayName, setDisplayName] = useState(localStorage.getItem('displayName'));
  const [orgCD, setOrgCD] = useState(localStorage.getItem('orgCD'));

  const login = async (user: string, password: string) => {
    const res = await authApi.login(user, password);
    localStorage.setItem('token', res.token);
    localStorage.setItem('username', res.username);
    localStorage.setItem('displayName', res.displayName);
    localStorage.setItem('orgCD', res.orgCD);
    setToken(res.token);
    setUsername(res.username);
    setDisplayName(res.displayName);
    setOrgCD(res.orgCD);
  };

  const logout = () => {
    localStorage.clear();
    setToken(null);
    setUsername(null);
    setDisplayName(null);
    setOrgCD(null);
  };

  return (
    <AuthContext.Provider value={{ token, username, displayName, orgCD, login, logout, isAuthenticated: !!token }}>
      {children}
    </AuthContext.Provider>
  );
}

export function useAuth() {
  const ctx = useContext(AuthContext);
  if (!ctx) throw new Error('useAuth must be used within AuthProvider');
  return ctx;
}