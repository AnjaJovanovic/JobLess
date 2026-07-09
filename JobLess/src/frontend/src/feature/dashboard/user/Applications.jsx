import { useEffect, useState } from "react";
import { applicationStatusLabel, getMyJobApplications } from "../../../api/clientApi";
import { useAuth } from "../../../context/AuthContext";

function statusClass(status) {
  const value = Number(status);
  if (value === 1) return "status-accepted";
  if (value === 2) return "status-rejected";
  return "status-pending";
}

export default function Applications() {
  const { user } = useAuth();
  const [applications, setApplications] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");

  useEffect(() => {
    let cancelled = false;

    async function loadApplications() {
      if (!user?.accessToken) {
        setApplications([]);
        setLoading(false);
        return;
      }

      try {
        setLoading(true);
        setError("");
        const data = await getMyJobApplications(user.accessToken);
        if (!cancelled) {
          setApplications(data);
        }
      } catch (err) {
        if (!cancelled) {
          setError(err.message || "Neuspešno učitavanje prijava.");
        }
      } finally {
        if (!cancelled) {
          setLoading(false);
        }
      }
    }

    loadApplications();

    return () => {
      cancelled = true;
    };
  }, [user?.accessToken]);

  return (
    <div>
      <h2>Moje prijave</h2>

      {loading && <p className="job-empty">Učitavanje prijava...</p>}
      {!loading && error && <p className="job-error">{error}</p>}

      {!loading && !error && applications.length === 0 && (
        <p className="job-empty">Trenutno nemate nijednu prijavu.</p>
      )}

      {!loading &&
        !error &&
        applications.map((item) => (
          <div key={item.id} className="application-card">
            <div className="application-card-info">
              <h4>Oglas #{item.advertisementId}</h4>
              <p>Kandidat: {item.candidateFirstName} {item.candidateLastName}</p>
              <p>Kompanija: {item.companyEmail}</p>
              <div className="application-date">
                Prijavljeno: {item.createdAt ? new Date(item.createdAt).toLocaleString("sr-RS") : "—"}
              </div>
            </div>
            <span className={`status-badge ${statusClass(item.status)}`}>
              {applicationStatusLabel(item.status)}
            </span>
          </div>
        ))}
    </div>
  );
}
