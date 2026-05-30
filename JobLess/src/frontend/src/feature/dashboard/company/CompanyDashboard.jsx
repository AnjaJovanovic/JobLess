import { useState } from "react";
import CompanyProfile from "./CompanyProfile";
import CreateJob from "./CreateJob";
import CompanyJobs from "./CompanyJobs";
import JobApplications from "./JobsApplications";
import "./Company.css";

const tabs = [
  { key: "profile",      label: "Profil kompanije" },
  { key: "create-job",   label: "Kreiraj oglas" },
  { key: "jobs",         label: "Moji oglasi" },
  { key: "applications", label: "Prijave" },
];

export default function CompanyDashboard() {
  const [activeTab, setActiveTab] = useState("profile");

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
          </button>
        ))}
      </aside>

      <main className="company-content">
        {activeTab === "profile"      && <CompanyProfile />}
        {activeTab === "create-job"   && <CreateJob onSuccess={() => setActiveTab("jobs")} />}
        {activeTab === "jobs"         && <CompanyJobs onCreateNew={() => setActiveTab("create-job")} />}
        {activeTab === "applications" && <JobApplications />}
      </main>
    </div>
  );
}