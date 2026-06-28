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
    { key: "profile", label: "Moj profil", requiresProfile: false },
    { key: "applications", label: "Moje prijave", requiresProfile: true },
    { key: "jobs", label: "Oglasi", requiresProfile: false },
    { key: "notifications", label: "Obaveštenja", requiresProfile: false },
  ];

  const handleTabClick = (tab) => {
    if (tab.requiresProfile && !profileComplete) return;
    setActiveTab(tab.key);
  };

  const handleLogout = () => {
    logout();
    navigate("/login");
  };

  return (
    <div className="user-layout">
      <aside className="user-sidebar">
        {tabs.map((tab) => {
          const disabled = tab.requiresProfile && !profileComplete;

          return (
            <button
              key={tab.key}
              className={`${activeTab === tab.key ? "active" : ""}${disabled ? " disabled" : ""}`}
              onClick={() => handleTabClick(tab)}
              disabled={disabled}
              title={disabled ? "Prvo popunite svoj profil" : undefined}
            >
              {tab.label}
            </button>
          );
        })}
        <button type="button" className="user-logout-btn" onClick={handleLogout}>
          Odjavi se
        </button>
      </aside>

      <main className="user-content">
        {activeTab === "profile" && (
          <Profile onProfileStatusChange={setProfileComplete} />
        )}
        {activeTab === "applications" && profileComplete && <Applications />}
        {activeTab === "jobs" && <JobList />}
        {activeTab === "notifications" && <Notifications />}
      </main>
    </div>
  );
}
