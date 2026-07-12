import { useState } from "react";
import { useNavigate } from "react-router-dom";
import { useAuth } from "../../../context/AuthContext";
import { useNotifications } from "../../../context/NotificationContext";
import CompanyProfile from "./CompanyProfile";
import CreateJob from "./CreateJob";
import CompanyJobs from "./CompanyJobs";
import JobApplications from "./JobsApplications";
import Notifications from "../../notifications/Notifications";
import "./Company.css";

const tabs = [
  { key: "profile",       label: "Profil kompanije" },
  { key: "create-job",    label: "Kreiraj oglas" },
  { key: "jobs",          label: "Moji oglasi" },
  { key: "applications",  label: "Prijave" },
  { key: "notifications", label: "Obaveštenja" },
];

export default function CompanyDashboard() {
  const { logout } = useAuth();
  const { unreadCount } = useNotifications();
  const navigate = useNavigate();
  const [activeTab, setActiveTab] = useState("profile");

  const handleLogout = () => {
    logout();
    navigate("/login");
  };

  return (
    <div className="company-layout">
      <aside className="company-sidebar">
        {tabs.map(tab => (
          <button
            key={tab.key}
            className={activeTab === tab.key ? "active" : ""}
            onClick={() => setActiveTab(tab.key)}
          >
            {tab.label}
             {tab.key === "notifications" && unreadCount > 0 && (
              <span className="tab-unread-badge">{unreadCount}</span>
            )}
          </button>
        ))}
        <button type="button" className="company-logout-btn" onClick={handleLogout}>
          Odjavi se
        </button>
      </aside>

      <main className="company-content">
        {activeTab === "profile"       && <CompanyProfile />}
        {activeTab === "create-job"    && <CreateJob onSuccess={() => setActiveTab("jobs")} />}
        {activeTab === "jobs"          && <CompanyJobs onCreateNew={() => setActiveTab("create-job")} />}
        {activeTab === "applications"  && <JobApplications />}
        {activeTab === "notifications" && <Notifications />}
      </main>
    </div>
  );
}