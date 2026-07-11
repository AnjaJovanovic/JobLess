import { useEffect, useState } from "react";
import { useAuth } from "../../../context/AuthContext";
import { getClientProfile } from "../../../api/clientApi";
import ProfileView from "../user/ProfileView";

export default function CandidateProfileModal({ clientId, candidateName, onClose }) {
  const { user } = useAuth();
  const [profile, setProfile] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  useEffect(() => {
    let cancelled = false;

    async function loadProfile() {
      setLoading(true);
      setError(null);

      if (!user?.accessToken) {
        setError("Niste prijavljeni. Prijavite se ponovo.");
        setProfile(null);
        setLoading(false);
        return;
      }

      try {
        const data = await getClientProfile(clientId, user.accessToken);
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
  }, [clientId, user?.accessToken]);

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
