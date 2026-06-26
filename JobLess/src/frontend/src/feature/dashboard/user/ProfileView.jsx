import { genderLabel, educationLevelLabel } from "../../../api/clientApi";
import { formatProfileDate } from "./profileUtils";

function DetailRow({ label, value, multiline = false }) {
  return (
    <div className={`profile-detail-row${multiline ? " profile-detail-row--multiline" : ""}`}>
      <dt>{label}</dt>
      <dd>{value || "—"}</dd>
    </div>
  );
}

export default function ProfileView({ profile, email, onEdit }) {
  const educationPeriod =
    profile.educationStartYear && profile.educationEndYear
      ? `${profile.educationStartYear} – ${profile.educationEndYear}`
      : profile.educationStartYear || profile.educationEndYear || null;

  return (
    <div className="profile-view">
      <section className="profile-view-section">
        <h3 className="profile-view-section-title">Lični podaci</h3>
        <dl className="profile-details">
          <DetailRow label="Ime" value={profile.firstName} />
          <DetailRow label="Prezime" value={profile.lastName} />
          <DetailRow label="Email" value={email} />
          <DetailRow label="Datum rođenja" value={formatProfileDate(profile.dateOfBirth)} />
          <DetailRow label="Pol" value={genderLabel(profile.gender)} />
        </dl>
      </section>

      <section className="profile-view-section">
        <h3 className="profile-view-section-title">Kontakt i lokacija</h3>
        <dl className="profile-details">
          <DetailRow label="Telefon" value={profile.phoneNumber} />
          <DetailRow label="Grad" value={profile.city} />
          <DetailRow label="Adresa" value={profile.address} />
          <DetailRow label="LinkedIn" value={profile.linkedInUrl} />
        </dl>
      </section>

      <section className="profile-view-section">
        <h3 className="profile-view-section-title">Obrazovanje</h3>
        <dl className="profile-details">
          <DetailRow label="Stepen obrazovanja" value={educationLevelLabel(profile.educationLevel)} />
          <DetailRow label="Institucija" value={profile.institutionName} />
          <DetailRow label="Period studija / školovanja" value={educationPeriod} />
        </dl>
      </section>

      <section className="profile-view-section">
        <h3 className="profile-view-section-title">Radno iskustvo</h3>
        <dl className="profile-details">
          <DetailRow
            label="Godine iskustva"
            value={
              profile.yearsOfExperience !== null && profile.yearsOfExperience !== undefined
                ? String(profile.yearsOfExperience)
                : null
            }
          />
          <DetailRow label="Veštine" value={profile.skills} />
          <DetailRow label="Biografija" value={profile.professionalSummary} multiline />
        </dl>
      </section>

      {onEdit && (
        <button type="button" className="btn-edit-profile" onClick={onEdit}>
          Izmeni podatke
        </button>
      )}
    </div>
  );
}