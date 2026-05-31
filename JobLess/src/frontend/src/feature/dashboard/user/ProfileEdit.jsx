import { useEffect, useState } from "react";
import { updateClientProfile } from "../../../api/clientApi";
import ProfileForm from "./ProfileForm";
import {
  formToPayload,
  getStoredEmail,
  profileToForm,
  validateProfileForm,
} from "./profileUtils";

export default function ProfileEdit({ profile, onSaved, onCancel }) {
  const [form, setForm] = useState(() => profileToForm(profile));
  const [errors, setErrors] = useState({});
  const [saving, setSaving] = useState(false);
  const [error, setError] = useState(null);
  const email = getStoredEmail();

  useEffect(() => {
    setForm(profileToForm(profile));
  }, [profile]);

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
      const result = await updateClientProfile(profile.clientId, formToPayload(form));
      onSaved(result);
    } catch (err) {
      setError(err.message || "Greška pri čuvanju profila.");
    } finally {
      setSaving(false);
    }
  };

  return (
    <div>
      <h2>Izmena profila</h2>
      <p className="profile-edit-intro">Ažurirajte svoje lične podatke i sačuvajte izmene.</p>

      {error && <div className="profile-message profile-message--error" role="alert">{error}</div>}

      <ProfileForm
        form={form}
        email={email}
        errors={errors}
        saving={saving}
        submitLabel="Sačuvaj izmene"
        onChange={handleChange}
        onSubmit={handleSubmit}
        onCancel={onCancel}
      />
    </div>
  );
}
