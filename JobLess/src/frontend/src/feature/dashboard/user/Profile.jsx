export default function Profile() {
  return (
    <div>
      <h2>Moj profil</h2>

      <form className="profile-form" onSubmit={(e) => e.preventDefault()}>
        <div className="form-group">
          <label>Ime</label>
          <input type="text" placeholder="Unesite ime" />
        </div>
        <div className="form-group">
          <label>Prezime</label>
          <input type="text" placeholder="Unesite prezime" />
        </div>
        <div className="form-group">
          <label>Email</label>
          <input type="email" placeholder="vas@email.com" />
        </div>
        <button type="submit" className="btn-save">Sačuvaj izmene</button>
      </form>
    </div>
  );
}