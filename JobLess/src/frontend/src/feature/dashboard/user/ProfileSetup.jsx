import { useState } from "react";
import { useAuth } from "../../../context/AuthContext";
import { createClientProfile, storeClientId } from "../../../api/clientApi";
import ProfileForm from "./ProfileForm";
import {
  emptyProfileForm,
  formToPayload,
  validateProfileForm,
} from "./profileUtils";

export default function ProfileSetup({ onCompleted }) {
  const { user } = useAuth();
  const [form, setForm] = useState(emptyProfileForm());
  const [errors, setErrors] = useState({});
  const [saving, setSaving] = useState(false);
  const [error, setError] = useState(null);
  const email = user?.email ?? "";

  const handleChange = (e) => {
    const { name, value } = e.target;
    setForm((prev) => ({ ...prev, [name]: value }));
    setErrors((prev) => ({ ...prev, [name]: undefined }));
    setError(null);
  };

  const handleSubmit = async (e) => {
    e.preventDefault();

    if (!email) {
      setError("Niste prijavljeni. Prijavite se ponovo.");
      return;
    }

    const validationErrors = validateProfileForm(form);
    if (Object.keys(validationErrors).length > 0) {
      setErrors(validationErrors);
      return;
    }

    setSaving(true);
    setError(null);

    try {
      const result = await createClientProfile(formToPayload(form, email));
      storeClientId(result.clientId);
      onCompleted(result);
    } catch (err) {
      setError(err.message || "Greška pri kreiranju profila.");
    } finally {
      setSaving(false);
    }
  };

  return (
    <div>
      <h2>Moj profil</h2>
      <p className="profile-setup-notice">
        Pre nego što nastavite, popunite profil sa podacima potrebnim za prijavu na posao.
        Email se ne može menjati nakon registracije.
      </p>

      {error && <div className="profile-message profile-message--error" role="alert">{error}</div>}

      <ProfileForm
        form={form}
        email={email}
        errors={errors}
        saving={saving}
        submitLabel="Sačuvaj profil"
        onChange={handleChange}
        onSubmit={handleSubmit}
      />
    </div>
  );
}
