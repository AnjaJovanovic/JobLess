import { useEffect, useMemo, useState } from "react";
import { useAuth } from "../../../context/AuthContext";
import {
  APPLICATION_STATUS,
  applicationStatusLabel,
  getApplicationsByAdvertisements,
  updateApplicationStatus,
} from "../../../api/clientApi";

const STATUS_OPTIONS = [
  { value: APPLICATION_STATUS.PENDING, label: "U razmatranju", cls: "s-review" },
  { value: APPLICATION_STATUS.ACCEPTED, label: "Prihvaćen", cls: "s-accepted" },
  { value: APPLICATION_STATUS.REJECTED, label: "Odbijen", cls: "s-rejected" },
];

const FILTERS = [
  { key: "all", label: "Sve", status: null },
  { key: "pending", label: "U razmatranju", status: APPLICATION_STATUS.PENDING },
  { key: "accepted", label: "Prihvaćen", status: APPLICATION_STATUS.ACCEPTED },
  { key: "rejected", label: "Odbijen", status: APPLICATION_STATUS.REJECTED },
];

async function fetchCompanyAdvertisements(companyId) {
  const response = await fetch(
    `/api/Advertisements/GetAdvertisementsForCompany?CompanyId=${companyId}&pageNumber=1&pageSize=100`
  );

  if (!response.ok) {
    throw new Error("Greška pri učitavanju oglasa kompanije.");
  }

  const data = await response.json();
  const ads = data.advertisements ?? data.Advertisements ?? [];
  return Array.isArray(ads) ? ads : [];
}

export default function JobApplications() {
  const { user } = useAuth();
  const companyId = user?.companyId ?? user?.id;

  const [applications, setApplications] = useState([]);
  const [adTitles, setAdTitles] = useState({});
  const [activeFilter, setActiveFilter] = useState("all");
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const [updatingId, setUpdatingId] = useState(null);

  useEffect(() => {
    let cancelled = false;

    async function loadApplications() {
      if (!companyId) {
        setLoading(false);
        setApplications([]);
        return;
      }

      setLoading(true);
      setError(null);

      try {
        const ads = await fetchCompanyAdvertisements(companyId);
        if (cancelled) return;

        const titles = {};
        const adIds = ads.map((ad) => {
          const id = ad.id ?? ad.Id;
          titles[id] = ad.title ?? ad.Title ?? `Oglas #${id}`;
          return id;
        });

        setAdTitles(titles);

        const items = await getApplicationsByAdvertisements(adIds);
        if (!cancelled) setApplications(items);
      } catch (err) {
        if (!cancelled) {
          setError(err.message || "Greška pri učitavanju prijava.");
          setApplications([]);
        }
      } finally {
        if (!cancelled) setLoading(false);
      }
    }

    loadApplications();
    return () => { cancelled = true; };
  }, [companyId]);

  const filtered = useMemo(() => {
    const filter = FILTERS.find((f) => f.key === activeFilter);
    if (!filter || filter.status === null) return applications;
    return applications.filter((app) => Number(app.status) === filter.status);
  }, [applications, activeFilter]);

  const countByStatus = (status) =>
    applications.filter((app) => Number(app.status) === status).length;

  const changeStatus = async (applicationId, newStatus) => {
    setUpdatingId(applicationId);
    setError(null);

    try {
      await updateApplicationStatus(applicationId, newStatus);
      setApplications((apps) =>
        apps.map((app) =>
          app.applicationId === applicationId
            ? { ...app, status: Number(newStatus) }
            : app
        )
      );
    } catch (err) {
      setError(err.message || "Greška pri promeni statusa.");
    } finally {
      setUpdatingId(null);
    }
  };

  const getStatusCls = (status) =>
    STATUS_OPTIONS.find((s) => s.value === Number(status))?.cls || "s-pending";

  if (!companyId) {
    return (
      <div>
        <h2>Prijave kandidata</h2>
        <p style={{ color: "var(--text-3)" }}>Nije moguće učitati prijave — nedostaje ID kompanije.</p>
      </div>
    );
  }

  return (
    <div>
      <h2>Prijave kandidata</h2>

      <div className="applications-filter">
        {FILTERS.map((filter) => (
          <button
            key={filter.key}
            type="button"
            className={`filter-btn ${activeFilter === filter.key ? "active" : ""}`}
            onClick={() => setActiveFilter(filter.key)}
          >
            {filter.label}
            <span style={{ marginLeft: 6, opacity: 0.6, fontSize: 11 }}>
              {filter.status === null
                ? applications.length
                : countByStatus(filter.status)}
            </span>
          </button>
        ))}
      </div>

      {loading && (
        <div style={{ textAlign: "center", padding: "40px 0", color: "var(--text-3)" }}>
          Učitavanje prijava...
        </div>
      )}

      {error && (
        <div className="server-error" role="alert" style={{ marginBottom: 16 }}>
          {error}
        </div>
      )}

      {!loading && filtered.map((app, i) => (
        <div
          className="application-row"
          key={app.applicationId}
          style={{ animationDelay: `${i * 0.05}s` }}
        >
          <div className="ar-info">
            <h4>{`${app.firstName} ${app.lastName}`}</h4>
            <p>
              {app.email}
              {" · "}
              {adTitles[app.advertisementId] ?? `Oglas #${app.advertisementId}`}
              {" · "}
              {app.appliedAt
                ? new Date(app.appliedAt).toLocaleDateString("sr-RS")
                : "—"}
            </p>
          </div>

          <div className="status-select-wrapper">
            <select
              className={`status-select ${getStatusCls(app.status)}`}
              value={Number(app.status)}
              disabled={updatingId === app.applicationId}
              onChange={(e) => changeStatus(app.applicationId, e.target.value)}
            >
              {STATUS_OPTIONS.map((opt) => (
                <option key={opt.value} value={opt.value}>
                  {opt.label}
                </option>
              ))}
            </select>
          </div>

          <span className={`badge badge-${Number(app.status) === APPLICATION_STATUS.ACCEPTED ? "accepted" : Number(app.status) === APPLICATION_STATUS.REJECTED ? "rejected" : "review"}`}>
            {applicationStatusLabel(app.status)}
          </span>
        </div>
      ))}

      {!loading && filtered.length === 0 && !error && (
        <div style={{ textAlign: "center", padding: "60px 0", color: "var(--text-3)", fontSize: 14 }}>
          Nema prijava za ovaj filter.
        </div>
      )}
    </div>
  );
}
