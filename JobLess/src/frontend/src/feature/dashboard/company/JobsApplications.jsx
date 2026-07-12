import { useCallback, useEffect, useState } from "react";
import {
  APPLICATION_STATUS,
  applicationStatusLabel,
  getCompanyJobApplications,
  updateJobApplicationStatus,
} from "../../../api/clientApi";
import { useAuth } from "../../../context/AuthContext";
import CandidateProfileModal from "./CandidateProfileModal";

function statusBadgeClass(status) {
  const value = Number(status);
  if (value === APPLICATION_STATUS.ACCEPTED) return "status-accepted";
  if (value === APPLICATION_STATUS.REJECTED) return "status-rejected";
  return "status-pending";
}

export default function JobApplications() {
  const { user } = useAuth();
  const [applications, setApplications] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");
  const [actionError, setActionError] = useState("");
  const [actionLoadingId, setActionLoadingId] = useState(null);
  const [advertisementFilter, setAdvertisementFilter] = useState("");
  const [statusFilter, setStatusFilter] = useState("");
  const [selectedCandidate, setSelectedCandidate] = useState(null);

  const loadApplications = useCallback(async () => {
    if (!user?.accessToken) {
      setApplications([]);
      setLoading(false);
      return;
    }

    try {
      setLoading(true);
      setError("");
      const data = await getCompanyJobApplications(user.accessToken, {
        advertisementId: advertisementFilter ? Number(advertisementFilter) : undefined,
        status: statusFilter !== "" ? Number(statusFilter) : undefined,
      });
      setApplications(data);
    } catch (err) {
      setError(err.message || "Neuspešno učitavanje prijava.");
    } finally {
      setLoading(false);
    }
  }, [user?.accessToken, advertisementFilter, statusFilter]);

  useEffect(() => {
    loadApplications();
  }, [loadApplications]);

  const handleStatusChange = async (applicationId, status) => {
    if (!user?.accessToken) return;

    try {
      setActionLoadingId(applicationId);
      setActionError("");
      await updateJobApplicationStatus(applicationId, status, user.accessToken);
      await loadApplications();
    } catch (err) {
      setActionError(err.message || "Promena statusa nije uspela.");
    } finally {
      setActionLoadingId(null);
    }
  };

  return (
    <div>
      <h2>Prijave kandidata</h2>

      <div className="applications-filter">
        <input
          className="form-input"
          type="number"
          min="1"
          placeholder="ID oglasa"
          value={advertisementFilter}
          onChange={(e) => setAdvertisementFilter(e.target.value)}
          style={{ maxWidth: "160px" }}
        />
        <select
          className="form-input"
          value={statusFilter}
          onChange={(e) => setStatusFilter(e.target.value)}
          style={{ maxWidth: "220px" }}
        >
          <option value="">Svi statusi</option>
          <option value={APPLICATION_STATUS.PENDING}>U razmatranju</option>
          <option value={APPLICATION_STATUS.ACCEPTED}>Prihvaćen</option>
          <option value={APPLICATION_STATUS.REJECTED}>Odbijen</option>
        </select>
      </div>

      {loading && <p className="empty-state">Učitavanje prijava...</p>}
      {!loading && error && <div className="server-error" role="alert">{error}</div>}
      {!loading && actionError && <div className="server-error" role="alert">{actionError}</div>}

      {!loading && !error && applications.length === 0 && (
        <p className="empty-state">Nema prijava za izabrane filtere.</p>
      )}

      {!loading &&
        !error &&
        applications.map((item) => {
          const candidateName = `${item.candidateFirstName} ${item.candidateLastName}`;
          const isPending = Number(item.status) === APPLICATION_STATUS.PENDING;

          return (
            <div key={item.id} className="application-row">
              <div className="ar-info">
                <h4>{candidateName}</h4>
                <p>{item.candidateEmail}</p>
                <p>{item.advertisementTitle?.trim() || `Oglas #${item.advertisementId}`}</p>
                <p>
                  Prijavljeno: {item.createdAt ? new Date(item.createdAt).toLocaleString("sr-RS") : "—"}
                </p>
              </div>

              <div className="ar-actions">
                <span className={`application-status-badge ${statusBadgeClass(item.status)}`}>
                  {applicationStatusLabel(item.status)}
                </span>

                <button
                  type="button"
                  className="btn-view-profile"
                  onClick={() =>
                    setSelectedCandidate({
                      clientId: item.candidateId,
                      name: candidateName,
                    })
                  }
                >
                  Profil
                </button>

                {isPending && (
                  <>
                    <button
                      type="button"
                      className="btn-action btn-action-accept"
                      disabled={actionLoadingId === item.id}
                      onClick={() => handleStatusChange(item.id, APPLICATION_STATUS.ACCEPTED)}
                    >
                      Prihvati
                    </button>
                    <button
                      type="button"
                      className="btn-action btn-action-reject"
                      disabled={actionLoadingId === item.id}
                      onClick={() => handleStatusChange(item.id, APPLICATION_STATUS.REJECTED)}
                    >
                      Odbij
                    </button>
                  </>
                )}
              </div>
            </div>
          );
        })}

      {selectedCandidate && (
        <CandidateProfileModal
          clientId={selectedCandidate.clientId}
          candidateName={selectedCandidate.name}
          onClose={() => setSelectedCandidate(null)}
        />
      )}
    </div>
  );
}
