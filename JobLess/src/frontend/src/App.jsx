import { Routes, Route, Navigate } from "react-router-dom";
import Layout from "./feature/layout/Layout";
import Login from "./feature/login/Login";
import UserDashboard from "./feature/dashboard/user/UserDashboard";
import CompanyDashboard from "./feature/dashboard/company/CompanyDashboard";
import ProtectedRoute from "./ProtectedRoute";

function App() {
  return (
    <Routes>
      <Route path="/login" element={<Login />} />

      <Route path="/" element={<Layout />}>
        <Route index element={<Navigate to="/login" replace />} />

        <Route
          path="user"
          element={
            <ProtectedRoute>
              <UserDashboard />
            </ProtectedRoute>
          }
        />
        <Route
          path="company"
          element={
            <ProtectedRoute>
              <CompanyDashboard />
            </ProtectedRoute>
          }
        />
      </Route>
    </Routes>
  );
}

export default App;