import { useState } from "react";
import Profile from "./Profile";
import Applications from "./Applications";
import JobList from "./JobList";
import "./User.css";

export default function UserDashboard() {
  const [activeTab, setActiveTab] = useState("profile");
  const [profileComplete, setProfileComplete] = useState(false);

  const tabs = [
    { key: "profile", label: "Moj profil", requiresProfile: false },
    { key: "applications", label: "Moje prijave", requiresProfile: true },
    { key: "jobs", label: "Oglasi", requiresProfile: true },
  ];

  const handleTabClick = (tab) => {
    if (tab.requiresProfile && !profileComplete) return;
    setActiveTab(tab.key);
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
      </aside>

      <main className="user-content">
        {activeTab === "profile" && (
          <Profile onProfileStatusChange={setProfileComplete} />
        )}
        {activeTab === "applications" && profileComplete && <Applications />}
        {activeTab === "jobs" && profileComplete && <JobList />}
      </main>
    </div>
  );
}
