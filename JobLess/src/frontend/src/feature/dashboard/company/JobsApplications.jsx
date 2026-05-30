import { useState } from "react";

const STATUS_OPTIONS = [
  { value: "Primljeno",    label: "Primljeno",    cls: "s-pending"  },
  { value: "U razmatranju",label: "U razmatranju",cls: "s-review"   },
  { value: "Prihvaćen",   label: "Prihvaćen",    cls: "s-accepted" },
  { value: "Odbijen",     label: "Odbijen",       cls: "s-rejected" },
];

const MOCK_APPLICATIONS = [
  { id: 1, ime: "Marko Nikolić",    email: "marko@email.com", oglas: "Senior React Developer",   status: "U razmatranju", datum: "2025-01-15" },
  { id: 2, ime: "Jovana Petrović", email: "jovana@email.com", oglas: "Senior React Developer",   status: "Primljeno",     datum: "2025-01-14" },
  { id: 3, ime: "Stefan Ilić",     email: "stefan@email.com", oglas: "Backend .NET Developer",   status: "Prihvaćen",    datum: "2025-01-12" },
  { id: 4, ime: "Ana Đorđević",    email: "ana@email.com",    oglas: "Backend .NET Developer",   status: "Odbijen",      datum: "2025-01-10" },
];

const FILTERS = ["Sve", "Primljeno", "U razmatranju", "Prihvaćen", "Odbijen"];

export default function JobApplications() {
  const [applications, setApplications] = useState(MOCK_APPLICATIONS);
  const [activeFilter, setActiveFilter] = useState("Sve");

  const changeStatus = (id, newStatus) => {
    setApplications(apps =>
      apps.map(a => a.id === id ? { ...a, status: newStatus } : a)
    );
  };

  const filtered = activeFilter === "Sve"
    ? applications
    : applications.filter(a => a.status === activeFilter);

  const getStatusCls = (status) =>
    STATUS_OPTIONS.find(s => s.value === status)?.cls || "s-pending";

  return (
    <div>
      <h2>Prijave kandidata</h2>

      <div className="applications-filter">
        {FILTERS.map(f => (
          <button
            key={f}
            className={`filter-btn ${activeFilter === f ? "active" : ""}`}
            onClick={() => setActiveFilter(f)}
          >
            {f}
            {f !== "Sve" && (
              <span style={{ marginLeft: 6, opacity: 0.6, fontSize: 11 }}>
                {applications.filter(a => a.status === f).length}
              </span>
            )}
            {f === "Sve" && (
              <span style={{ marginLeft: 6, opacity: 0.6, fontSize: 11 }}>
                {applications.length}
              </span>
            )}
          </button>
        ))}
      </div>

      {filtered.map((app, i) => (
        <div
          className="application-row"
          key={app.id}
          style={{ animationDelay: `${i * 0.05}s` }}
        >
          <div className="ar-info">
            <h4>{app.ime}</h4>
            <p>{app.email} &nbsp;·&nbsp; {app.oglas} &nbsp;·&nbsp; {app.datum}</p>
          </div>

          <div className="status-select-wrapper">
            <select
              className={`status-select ${getStatusCls(app.status)}`}
              value={app.status}
              onChange={(e) => changeStatus(app.id, e.target.value)}
            >
              {STATUS_OPTIONS.map(opt => (
                <option key={opt.value} value={opt.value}>{opt.label}</option>
              ))}
            </select>
          </div>

          <button className="btn-icon" title="Pregledaj CV">
            📄
          </button>
        </div>
      ))}

      {filtered.length === 0 && (
        <div style={{ textAlign: "center", padding: "60px 0", color: "var(--text-3)", fontSize: 14 }}>
          Nema prijava za ovaj filter.
        </div>
      )}
    </div>
  );
}