import { useEffect, useState } from "react";
import { getClientProfile, getAdvertisementById } from "../../api/clientApi";
import ProfileView from "../dashboard/user/ProfileView";

export default function NewApplicationModal({ candidateId, advertisementId, onClose }) {
  const [candidate, setCandidate] = useState(null);
  const [advertisement, setAdvertisement] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  useEffect(() => {
    let cancelled = false;

    async function load() {
      setLoading(true);
      setError(null);

      try {
        const [candidateProfile, ad] = await Promise.all([
          candidateId ? getClientProfile(candidateId) : Promise.resolve(null),
          advertisementId ? getAdvertisementById(advertisementId) : Promise.resolve(null),
        ]);

        if (cancelled) return;

        if (!candidateProfile && !ad) {
          setError("Podaci o prijavi nisu pronađeni.");
        }

        setCandidate(candidateProfile);
        setAdvertisement(ad);
      } catch (err) {
        if (!cancelled) {
          setError(err.message || "Greška pri učitavanju podataka o prijavi.");
        }
      } finally {
        if (!cancelled) setLoading(false);
      }
    }

    load();
    return () => { cancelled = true; };
  }, [candidateId, advertisementId]);

  return (
    <div
      className="modal-overlay"
      onClick={(e) => e.target === e.currentTarget && onClose()}
      role="presentation"
    >
      <div className="modal-box large notification-detail-modal" role="dialog" aria-modal="true">
        <div className="modal-header">
          <h3>Prijava na oglas</h3>
          <button type="button" className="modal-close" onClick={onClose} aria-label="Zatvori">
            ✕
          </button>
        </div>

        <div className="notification-detail-body">
          {loading && <p className="notification-detail-loading">Učitavanje...</p>}
          {error && <div className="notification-detail-error" role="alert">{error}</div>}

          {!loading && advertisement && (
            <section className="notification-detail-section">
              <h4>Oglas</h4>
              <p className="notification-detail-highlight">{advertisement.title}</p>
              <p className="notification-detail-sub">
                {advertisement.position}
                {advertisement.city ? ` · ${advertisement.city}` : ""}
                {advertisement.country ? `, ${advertisement.country}` : ""}
              </p>
            </section>
          )}

          {!loading && candidate && (
            <section className="notification-detail-section">
              <h4>Kandidat</h4>
              <ProfileView profile={candidate} email={candidate.email} />
            </section>
          )}
        </div>
      </div>
    </div>
  );
}
