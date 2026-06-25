import { useEffect, useState } from "react";
import { getClientProfile } from "../../../api/clientApi";
import ProfileView from "../user/ProfileView";

export default function CandidateProfileModal({ clientId, candidateName, onClose }) {
  const [profile, setProfile] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  useEffect(() => {
    let cancelled = false;

    async function loadProfile() {
      setLoading(true);
      setError(null);

      try {
        const data = await getClientProfile(clientId);
        if (cancelled) return;

        if (!data) {
          setError("Profil kandidata nije pronađen.");
          setProfile(null);
          return;
        }

        setProfile(data);
      } catch (err) {
        if (!cancelled) {
          setError(err.message || "Greška pri učitavanju profila kandidata.");
          setProfile(null);
        }
      } finally {
        if (!cancelled) setLoading(false);
      }
    }

    loadProfile();
    return () => { cancelled = true; };
  }, [clientId]);

  return (
    <div
      className="modal-overlay"
      onClick={(e) => e.target === e.currentTarget && onClose()}
      role="presentation"
    >
      <div className="modal-box large candidate-profile-modal" role="dialog" aria-modal="true">
        <div className="modal-header">
          <h3>
            Profil kandidata
            {candidateName ? `: ${candidateName}` : ""}
          </h3>
          <button type="button" className="modal-close" onClick={onClose} aria-label="Zatvori">
            ✕
          </button>
        </div>

        <div className="candidate-profile-modal-body">
          {loading && (
            <p className="candidate-profile-loading">Učitavanje profila...</p>
          )}

          {error && (
            <div className="server-error" role="alert">
              {error}
            </div>
          )}

          {!loading && !error && profile && (
            <ProfileView profile={profile} email={profile.email} />
          )}
        </div>
      </div>
    </div>
  );
}
