import { GENDER } from "../../../api/clientApi";

export default function ProfileForm({
  form,
  email,
  errors = {},
  saving = false,
  submitLabel,
  onChange,
  onSubmit,
  onCancel,
}) {
  return (
    <form className="profile-form" onSubmit={onSubmit} noValidate>
      <div className="form-group">
        <label htmlFor="firstName">Ime</label>
        <input
          id="firstName"
          name="firstName"
          type="text"
          placeholder="Unesite ime"
          value={form.firstName}
          onChange={onChange}
          className={errors.firstName ? "input-error" : ""}
        />
        {errors.firstName && <span className="field-error">{errors.firstName}</span>}
      </div>

      <div className="form-group">
        <label htmlFor="lastName">Prezime</label>
        <input
          id="lastName"
          name="lastName"
          type="text"
          placeholder="Unesite prezime"
          value={form.lastName}
          onChange={onChange}
          className={errors.lastName ? "input-error" : ""}
        />
        {errors.lastName && <span className="field-error">{errors.lastName}</span>}
      </div>

      <div className="form-group">
        <label htmlFor="email">Email</label>
        <input id="email" type="email" value={email} readOnly disabled />
      </div>

      <div className="form-group">
        <label htmlFor="phoneNumber">Telefon</label>
        <input
          id="phoneNumber"
          name="phoneNumber"
          type="tel"
          placeholder="+381 60 123 4567"
          value={form.phoneNumber}
          onChange={onChange}
        />
      </div>

      <fieldset className="form-group profile-gender-group">
        <legend>Pol</legend>
        <div className="profile-gender-options">
          <label className="profile-gender-option">
            <input
              type="radio"
              name="gender"
              value="0"
              checked={form.gender !== null && form.gender !== "" && Number(form.gender) === GENDER.MALE}
              onChange={onChange}
            />
            <span>Muški</span>
          </label>
          <label className="profile-gender-option">
            <input
              type="radio"
              name="gender"
              value="1"
              checked={form.gender !== null && form.gender !== "" && Number(form.gender) === GENDER.FEMALE}
              onChange={onChange}
            />
            <span>Ženski</span>
          </label>
        </div>
        {errors.gender && <span className="field-error">{errors.gender}</span>}
      </fieldset>

      <div className="profile-form-actions">
        {onCancel && (
          <button type="button" className="btn-secondary" onClick={onCancel} disabled={saving}>
            Otkaži
          </button>
        )}
        <button type="submit" className="btn-save" disabled={saving}>
          {saving ? "Čuvanje..." : submitLabel}
        </button>
      </div>
    </form>
  );
}
