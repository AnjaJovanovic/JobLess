import { Routes, Route, Navigate } from "react-router-dom";
import Layout from "./feature/layout/Layout";
import Login from "./feature/login/Login";
import UserDashboard from "./feature/dashboard/user/UserDashboard";
import CompanyDashboard from "./feature/dashboard/company/CompanyDashboard";

function App() {
  return (
    <Routes>
      <Route path="/login" element={<Login />} />

      <Route path="/" element={<Layout />}>
        <Route index element={<Navigate to="/login" replace />} />

        <Route path="user" element={<UserDashboard />} />
        <Route path="company" element={<CompanyDashboard />} />
      </Route>
    </Routes>
  );
}

export default App;