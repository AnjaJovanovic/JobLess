import { useState } from "react";
import { useNavigate } from "react-router-dom";
import { useAuth } from "../../../context/AuthContext";
import Profile from "./Profile";
import Applications from "./Applications";
import JobList from "./JobList";
import Notifications from "../../notifications/Notifications";
import "./User.css";

export default function UserDashboard() {
  const { logout } = useAuth();
  const navigate = useNavigate();
  const [activeTab, setActiveTab] = useState("profile");
  const [profileComplete, setProfileComplete] = useState(false);

  const tabs = [
    { key: "profile", label: "Moj profil" },
    { key: "applications", label: "Moje prijave" },
    { key: "jobs", label: "Oglasi" },
    { key: "notifications", label: "Obaveštenja" },
  ];

  const handleTabClick = (tab) => {
    setActiveTab(tab.key);
  };

  const handleLogout = () => {
    logout();
    navigate("/login");
  };

  return (
    <div className="user-layout">
      <aside className="user-sidebar">
        {tabs.map((tab) => (
          <button
            key={tab.key}
            className={activeTab === tab.key ? "active" : ""}
            onClick={() => handleTabClick(tab)}
          >
            {tab.label}
          </button>
        ))}
        <button type="button" className="user-logout-btn" onClick={handleLogout}>
          Odjavi se
        </button>
      </aside>

      <main className="user-content">
        {activeTab === "profile" && (
          <Profile onProfileStatusChange={setProfileComplete} />
        )}
        {activeTab === "applications" && <Applications />}
        {activeTab === "jobs" && <JobList profileComplete={profileComplete} />}
        {activeTab === "notifications" && <Notifications />}
      </main>
    </div>
  );
}
