import { genderLabel } from "../../../api/clientApi";

export default function ProfileView({ profile, email, onEdit }) {
  return (
    <div className="profile-view">
      <dl className="profile-details">
        <div className="profile-detail-row">
          <dt>Ime</dt>
          <dd>{profile.firstName}</dd>
        </div>
        <div className="profile-detail-row">
          <dt>Prezime</dt>
          <dd>{profile.lastName}</dd>
        </div>
        <div className="profile-detail-row">
          <dt>Email</dt>
          <dd>{email || "—"}</dd>
        </div>
        <div className="profile-detail-row">
          <dt>Telefon</dt>
          <dd>{profile.phoneNumber || "—"}</dd>
        </div>
        <div className="profile-detail-row">
          <dt>Pol</dt>
          <dd>{genderLabel(profile.gender)}</dd>
        </div>
      </dl>

      <button type="button" className="btn-edit-profile" onClick={onEdit}>
        Izmeni podatke
      </button>
    </div>
  );
}
