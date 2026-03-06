import { useState } from "react";
import Profile from "./Profile";
import Applications from "./Applications";
import JobList from "./JobList";
import "./User.css";

export default function UserDashboard() {
  const [activeTab, setActiveTab] = useState("profile");

  const tabs = [
    { key: "profile",       label: "Moj profil" },
    { key: "applications",  label: "Moje prijave" },
    { key: "jobs",          label: "Oglasi" },
  ];

  return (
    <div className="user-layout">
      <aside className="user-sidebar">
        {tabs.map(tab => (
          <button
            key={tab.key}
            className={activeTab === tab.key ? "active" : ""}
            onClick={() => setActiveTab(tab.key)}
          >
            {tab.label}
          </button>
        ))}
      </aside>

      <main className="user-content">
        {activeTab === "profile"       && <Profile />}
        {activeTab === "applications"  && <Applications />}
        {activeTab === "jobs"          && <JobList />}
      </main>
    </div>
  );
}