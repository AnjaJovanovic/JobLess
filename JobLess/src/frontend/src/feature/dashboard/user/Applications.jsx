import { useEffect, useState } from "react";
import {
  applicationStatusLabel,
  APPLICATION_STATUS,
  getClientApplications,
  getStoredClientId,
} from "../../../api/clientApi";

function statusClass(status) {
  const value = Number(status);
  if (value === APPLICATION_STATUS.REJECTED) return "status-rejected";
  if (value === APPLICATION_STATUS.ACCEPTED) return "status-accepted";
  return "status-pending";
}

async function fetchAdvertisement(advertisementId) {
  const response = await fetch(`/api/Advertisements/One?id=${advertisementId}`);
  if (!response.ok) return null;

  const data = await response.json();
  return data.advertisement ?? data.Advertisement ?? null;
}

export default function Applications() {
  const clientId = getStoredClientId();
  const [applications, setApplications] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  useEffect(() => {
    let cancelled = false;

    async function loadApplications() {
      if (!clientId) {
        setError("Profil klijenta nije pronađen. Popunite profil pa pokušajte ponovo.");
        setLoading(false);
        return;
      }

      try {
        setLoading(true);
        setError(null);

        const items = await getClientApplications(clientId);
        const enriched = await Promise.all(
          items.map(async (application) => {
            const ad = await fetchAdvertisement(application.advertisementId);
            return {
              ...application,
              title: ad?.title ?? `Oglas #${application.advertisementId}`,
              position: ad?.position ?? "—",
              city: ad?.city ?? "—",
            };
          }),
        );

        if (!cancelled) setApplications(enriched);
      } catch (err) {
        if (!cancelled) {
          setError(err.message || "Greška pri učitavanju prijava.");
        }
      } finally {
        if (!cancelled) setLoading(false);
      }
    }

    loadApplications();
    return () => { cancelled = true; };
  }, [clientId]);

  if (loading) {
    return (
      <div>
        <h2>Moje prijave</h2>
        <p className="job-empty">Učitavanje prijava...</p>
      </div>
    );
  }

  if (error) {
    return (
      <div>
        <h2>Moje prijave</h2>
        <p className="job-error">{error}</p>
      </div>
    );
  }

  return (
    <div>
      <h2>Moje prijave</h2>

      {applications.length === 0 && (
        <p className="job-empty">Još niste prijavljeni ni na jedan oglas.</p>
      )}

      {applications.map((app, i) => (
        <div key={app.applicationId} className="application-card" style={{ animationDelay: `${i * 0.06}s` }}>
          <div className="application-card-info">
            <h4>{app.title}</h4>
            <p>{app.position} · {app.city}</p>
            <p className="application-date">
              Prijavljeno: {new Date(app.appliedAt).toLocaleDateString("sr-RS")}
            </p>
          </div>
          <span className={statusClass(app.status)}>
            {applicationStatusLabel(app.status)}
          </span>
        </div>
      ))}
    </div>
  );
}
