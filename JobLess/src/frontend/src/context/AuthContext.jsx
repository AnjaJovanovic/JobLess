import { createContext, useState, useEffect, useContext } from "react";
import { clearStoredClientId } from "../api/clientApi";

export const AuthContext = createContext();

export function AuthProvider({ children }) {
  const [user, setUser] = useState(null);

  useEffect(() => {
    const storedUser = localStorage.getItem("user");
    if (storedUser) {
      setUser(JSON.parse(storedUser));
    }
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
}

// 🔥 DODAJ OVO
export function useAuth() {
  return useContext(AuthContext);
}
