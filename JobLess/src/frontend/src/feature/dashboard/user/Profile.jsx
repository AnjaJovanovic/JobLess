import { useEffect, useState } from "react";
import {
  getClientProfile,
  getStoredClientId,
  storeClientId,
} from "../../../api/clientApi";
import { getStoredEmail } from "./profileUtils";
import ProfileEdit from "./ProfileEdit";
import ProfileSetup from "./ProfileSetup";
import ProfileView from "./ProfileView";

export default function Profile({ onProfileStatusChange }) {
  const [mode, setMode] = useState("loading");
  const [profile, setProfile] = useState(null);
  const [error, setError] = useState(null);
  const email = getStoredEmail();

  useEffect(() => {
    let cancelled = false;

    async function loadProfile() {
      setMode("loading");
      setError(null);

      const clientId = getStoredClientId();
      if (!clientId) {
        if (!cancelled) {
          setProfile(null);
          setMode("setup");
          onProfileStatusChange?.(false);
        }
        return;
      }

      try {
        const data = await getClientProfile(clientId);
        if (cancelled) return;

        if (!data) {
          localStorage.removeItem("clientProfileId");
          setProfile(null);
          setMode("setup");
          onProfileStatusChange?.(false);
          return;
        }

        setProfile(data);
        setMode("view");
        onProfileStatusChange?.(true);
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
  }, [onProfileStatusChange]);

  const handleCompleted = (result) => {
    setProfile(result);
    storeClientId(result.clientId);
    setMode("view");
    onProfileStatusChange?.(true);
  };

  const handleSaved = (result) => {
    setProfile(result);
    setMode("view");
    onProfileStatusChange?.(true);
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

  return (
    <div>
      <h2>Moj profil</h2>
      <ProfileView
        profile={profile}
        email={email}
        onEdit={() => setMode("edit")}
      />
    </div>
  );
}
