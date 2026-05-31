import { useState } from "react";
import { createClientProfile, storeClientId } from "../../../api/clientApi";
import ProfileForm from "./ProfileForm";
import {
  emptyProfileForm,
  formToPayload,
  getStoredEmail,
  validateProfileForm,
} from "./profileUtils";

export default function ProfileSetup({ onCompleted }) {
  const [form, setForm] = useState(emptyProfileForm());
  const [errors, setErrors] = useState({});
  const [saving, setSaving] = useState(false);
  const [error, setError] = useState(null);
  const email = getStoredEmail();

  const handleChange = (e) => {
    const { name, value } = e.target;
    setForm((prev) => ({ ...prev, [name]: value }));
    setErrors((prev) => ({ ...prev, [name]: undefined }));
    setError(null);
  };

  const handleSubmit = async (e) => {
    e.preventDefault();

    const validationErrors = validateProfileForm(form);
    if (Object.keys(validationErrors).length > 0) {
      setErrors(validationErrors);
      return;
    }

    setSaving(true);
    setError(null);

    try {
      const result = await createClientProfile(formToPayload(form));
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
        Pre nego što nastavite, popunite osnovne informacije o sebi. Ovi podaci su obavezni.
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
