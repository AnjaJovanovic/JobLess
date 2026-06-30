import { createContext, useState, useEffect, useContext } from "react";
import { clearStoredClientId, refreshAccessToken } from "../api/clientApi";

export const AuthContext = createContext();

export function AuthProvider({ children }) {
  const [user, setUser] = useState(null);

    useEffect(() => {
    const storedUser = localStorage.getItem("user");
    if (!storedUser) return;

    const parsed = JSON.parse(storedUser);
    const expiresAt = parsed.expiresAt ? new Date(parsed.expiresAt) : null;

    if (!expiresAt || expiresAt > new Date()) {
      setUser(parsed);
      return;
    }

    // istek access tokena - refresh token
    if (!parsed.refreshToken || !parsed.email) {
      localStorage.removeItem("user");
      return;
    }

    refreshAccessToken(parsed.email, parsed.refreshToken)
      .then((newAuth) => {
        if (newAuth) {
          localStorage.setItem("user", JSON.stringify(newAuth));
          setUser(newAuth);
        } else {
          localStorage.removeItem("user");
        }
      })
      .catch(() => localStorage.removeItem("user"));
  }, []);

  const login = (authData) => {
    localStorage.setItem("user", JSON.stringify(authData));
    setUser(authData);
  };

  const logout = () => {
    localStorage.removeItem("user");
    clearStoredClientId();
    setUser(null);
  };

  return (
    <AuthContext.Provider value={{ user, login, logout }}>
      {children}
    </AuthContext.Provider>
  );
};
export function useAuth() {
  return useContext(AuthContext);
}
