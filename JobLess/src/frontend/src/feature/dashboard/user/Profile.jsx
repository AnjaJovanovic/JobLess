import { useEffect, useState } from "react";
import { useAuth } from "../../../context/AuthContext";
import {
  getClientProfileByEmail,
  storeClientId,
} from "../../../api/clientApi";
import ProfileEdit from "./ProfileEdit";
import ProfileSetup from "./ProfileSetup";
import ProfileView from "./ProfileView";
import { isProfileComplete } from "./profileUtils";

export default function Profile({ onProfileStatusChange }) {
  const { user } = useAuth();
  const [mode, setMode] = useState("loading");
  const [profile, setProfile] = useState(null);
  const [error, setError] = useState(null);
  const email = user?.email ?? "";

  useEffect(() => {
    let cancelled = false;

    async function loadProfile() {
      setMode("loading");
      setError(null);

      if (!email) {
        if (!cancelled) {
          setProfile(null);
          setMode("setup");
          onProfileStatusChange?.(false);
        }
        return;
      }

      try {
        const data = await getClientProfileByEmail(email);
        if (cancelled) return;

        if (!data) {
          setProfile(null);
          setMode("setup");
          onProfileStatusChange?.(false);
          return;
        }

        storeClientId(data.clientId);
        setProfile(data);
        setMode("view");
        onProfileStatusChange?.(isProfileComplete(data));
      } catch (err) {
        if (!cancelled) {
          setError(err.message || "Greška pri učitavanju profila.");
          setMode("setup");
          onProfileStatusChange?.(false);
        }
      }
    }

    loadProfile();
    return () => { cancelled = true; };
  }, [email, onProfileStatusChange]);

  const handleCompleted = (result) => {
    setProfile(result);
    storeClientId(result.clientId);
    setMode("view");
    onProfileStatusChange?.(isProfileComplete(result));
  };

  const handleSaved = (result) => {
    setProfile(result);
    setMode("view");
    onProfileStatusChange?.(isProfileComplete(result));
  };

  if (mode === "loading") {
    return (
      <div>
        <h2>Moj profil</h2>
        <p>Učitavanje profila...</p>
      </div>
    );
  }

  if (error && mode === "setup") {
    return (
      <div>
        <h2>Moj profil</h2>
        <div className="profile-message profile-message--error" role="alert">{error}</div>
        <ProfileSetup onCompleted={handleCompleted} />
      </div>
    );
  }

  if (mode === "setup") {
    return <ProfileSetup onCompleted={handleCompleted} />;
  }

  if (mode === "edit") {
    return (
      <ProfileEdit
        profile={profile}
        onSaved={handleSaved}
        onCancel={() => setMode("view")}
      />
    );
  }

  if (mode === "view") {
    return (
      <div>
        <h2>Moj profil</h2>
        {!isProfileComplete(profile) && (
          <div className="profile-message profile-setup-notice" role="status">
            Profil nije potpuno popunjen. Dopunite podatke pre prijave na posao.
          </div>
        )}
        <ProfileView
          profile={profile}
          email={email}
          onEdit={() => setMode("edit")}
        />
      </div>
    );
  }

  return null;
}
