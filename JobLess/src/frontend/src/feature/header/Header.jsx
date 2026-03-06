import { Link } from "react-router-dom";
import { useAuth } from "../../context/AuthContext/";

export default function Header() {
  const { user, logout } = useAuth();

  return (
    <header className="header">
      <h2>Mini Infostud</h2>
      <nav>
        {!user && (
          <>
            <Link to="/login">Login</Link>
            
          </>
        )}

        {user && (
          <>
            <Link to="/user">Home</Link>
            <button onClick={logout}>Logout</button>
          </>
        )}
      </nav>
    </header>
  );
}
