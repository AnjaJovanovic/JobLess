import { GENDER, EDUCATION_LEVEL_OPTIONS } from "../../../api/clientApi";

function FormSection({ title, children }) {
  return (
    <section className="profile-form-section">
      <h3 className="profile-form-section-title">{title}</h3>
      {children}
    </section>
  );
}

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
      <FormSection title="Lični podaci">
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
          <span className="field-hint">Email se ne može menjati.</span>
        </div>

        <div className="form-group">
          <label htmlFor="dateOfBirth">Datum rođenja</label>
          <input
            id="dateOfBirth"
            name="dateOfBirth"
            type="date"
            value={form.dateOfBirth}
            onChange={onChange}
            className={errors.dateOfBirth ? "input-error" : ""}
          />
          {errors.dateOfBirth && <span className="field-error">{errors.dateOfBirth}</span>}
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
      </FormSection>

      <FormSection title="Kontakt i lokacija">
        <div className="form-group">
          <label htmlFor="phoneNumber">Telefon</label>
          <input
            id="phoneNumber"
            name="phoneNumber"
            type="tel"
            placeholder="+381 60 123 4567"
            value={form.phoneNumber}
            onChange={onChange}
            className={errors.phoneNumber ? "input-error" : ""}
          />
          {errors.phoneNumber && <span className="field-error">{errors.phoneNumber}</span>}
        </div>

        <div className="form-group">
          <label htmlFor="city">Grad</label>
          <input
            id="city"
            name="city"
            type="text"
            placeholder="npr. Beograd"
            value={form.city}
            onChange={onChange}
            className={errors.city ? "input-error" : ""}
          />
          {errors.city && <span className="field-error">{errors.city}</span>}
        </div>

        <div className="form-group">
          <label htmlFor="address">Adresa</label>
          <input
            id="address"
            name="address"
            type="text"
            placeholder="Ulica i broj (opciono)"
            value={form.address}
            onChange={onChange}
          />
        </div>

        <div className="form-group">
          <label htmlFor="linkedInUrl">LinkedIn profil</label>
          <input
            id="linkedInUrl"
            name="linkedInUrl"
            type="url"
            placeholder="https://linkedin.com/in/..."
            value={form.linkedInUrl}
            onChange={onChange}
            className={errors.linkedInUrl ? "input-error" : ""}
          />
          {errors.linkedInUrl && <span className="field-error">{errors.linkedInUrl}</span>}
        </div>
      </FormSection>

      <FormSection title="Obrazovanje">
        <div className="form-group">
          <label htmlFor="educationLevel">Stepen obrazovanja</label>
          <select
            id="educationLevel"
            name="educationLevel"
            value={form.educationLevel}
            onChange={onChange}
            className={errors.educationLevel ? "input-error" : ""}
          >
            <option value="">Izaberite stepen obrazovanja</option>
            {EDUCATION_LEVEL_OPTIONS.map((option) => (
              <option key={option.value} value={option.value}>
                {option.label}
              </option>
            ))}
          </select>
          {errors.educationLevel && <span className="field-error">{errors.educationLevel}</span>}
        </div>

        <div className="form-group">
          <label htmlFor="institutionName">Naziv institucije</label>
          <input
            id="institutionName"
            name="institutionName"
            type="text"
            placeholder="npr. Fakultet organizacionih nauka"
            value={form.institutionName}
            onChange={onChange}
            className={errors.institutionName ? "input-error" : ""}
          />
          {errors.institutionName && <span className="field-error">{errors.institutionName}</span>}
        </div>

        <div className="profile-form-row">
          <div className="form-group">
            <label htmlFor="educationStartYear">Godina početka</label>
            <input
              id="educationStartYear"
              name="educationStartYear"
              type="number"
              min="1950"
              max="2100"
              placeholder="2018"
              value={form.educationStartYear}
              onChange={onChange}
              className={errors.educationStartYear ? "input-error" : ""}
            />
            {errors.educationStartYear && (
              <span className="field-error">{errors.educationStartYear}</span>
            )}
          </div>

          <div className="form-group">
            <label htmlFor="educationEndYear">Godina završetka</label>
            <input
              id="educationEndYear"
              name="educationEndYear"
              type="number"
              min="1950"
              max="2100"
              placeholder="2022"
              value={form.educationEndYear}
              onChange={onChange}
              className={errors.educationEndYear ? "input-error" : ""}
            />
            {errors.educationEndYear && (
              <span className="field-error">{errors.educationEndYear}</span>
            )}
          </div>
        </div>
      </FormSection>

      <FormSection title="Radno iskustvo i veštine">
        <div className="form-group">
          <label htmlFor="yearsOfExperience">Godine radnog iskustva</label>
          <input
            id="yearsOfExperience"
            name="yearsOfExperience"
            type="number"
            min="0"
            max="60"
            placeholder="npr. 3"
            value={form.yearsOfExperience}
            onChange={onChange}
            className={errors.yearsOfExperience ? "input-error" : ""}
          />
          {errors.yearsOfExperience && (
            <span className="field-error">{errors.yearsOfExperience}</span>
          )}
        </div>

        <div className="form-group">
          <label htmlFor="skills">Veštine</label>
          <input
            id="skills"
            name="skills"
            type="text"
            placeholder="npr. JavaScript, React, SQL"
            value={form.skills}
            onChange={onChange}
          />
          <span className="field-hint">Odvojite veštine zarezom.</span>
        </div>

        <div className="form-group">
          <label htmlFor="professionalSummary">Kratak opis / biografija</label>
          <textarea
            id="professionalSummary"
            name="professionalSummary"
            rows={4}
            placeholder="Ukratko opišite svoje iskustvo, ciljeve i ključne kvalitete..."
            value={form.professionalSummary}
            onChange={onChange}
          />
        </div>
      </FormSection>

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
