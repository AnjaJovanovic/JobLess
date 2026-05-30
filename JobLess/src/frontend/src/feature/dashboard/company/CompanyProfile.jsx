import { useState } from "react";

export default function CompanyProfile() {
  const [form, setForm] = useState({
    naziv: "Tech Solutions d.o.o.",
    email: "kontakt@techsolutions.rs",
    telefon: "+381 11 123 4567",
    website: "https://techsolutions.rs",
    pib: "123456789",
    mb: "87654321",
    grad: "Beograd",
    adresa: "Knez Mihailova 10",
    delatnost: "Razvoj softvera",
    opis: "Vodeća kompanija u oblasti razvoja softvera i digitalnih rešenja na Balkanu.",
  });

  const [saved, setSaved] = useState(false);

  const handle = (e) => {
    setForm(f => ({ ...f, [e.target.name]: e.target.value }));
    setSaved(false);
  };

  const handleSubmit = (e) => {
    e.preventDefault();
    // TODO: API call
    setSaved(true);
    setTimeout(() => setSaved(false), 3000);
  };

  return (
    <div>
      <h2>Profil kompanije</h2>

      <form onSubmit={handleSubmit}>
        <div className="form-card">

          <h3>Osnovne informacije</h3>
          <div className="form-grid">
            <div className="form-group form-col-span-2">
              <label>Naziv kompanije</label>
              <input name="naziv" value={form.naziv} onChange={handle} placeholder="Naziv kompanije" />
            </div>

            <div className="form-group">
              <label>Email</label>
              <input type="email" name="email" value={form.email} onChange={handle} placeholder="kontakt@kompanija.rs" />
            </div>

            <div className="form-group">
              <label>Telefon</label>
              <input name="telefon" value={form.telefon} onChange={handle} placeholder="+381..." />
            </div>

            <div className="form-group">
              <label>Website</label>
              <input name="website" value={form.website} onChange={handle} placeholder="https://..." />
            </div>

            <div className="form-group">
              <label>Delatnost</label>
              <input name="delatnost" value={form.delatnost} onChange={handle} placeholder="Npr. IT, Finansije..." />
            </div>
          </div>

          <h3>Pravne informacije</h3>
          <div className="form-grid">
            <div className="form-group">
              <label>PIB</label>
              <input name="pib" value={form.pib} onChange={handle} placeholder="PIB" />
            </div>
            <div className="form-group">
              <label>Matični broj</label>
              <input name="mb" value={form.mb} onChange={handle} placeholder="Matični broj" />
            </div>
          </div>

          <h3>Lokacija</h3>
          <div className="form-grid">
            <div className="form-group">
              <label>Grad</label>
              <input name="grad" value={form.grad} onChange={handle} placeholder="Beograd" />
            </div>
            <div className="form-group">
              <label>Adresa</label>
              <input name="adresa" value={form.adresa} onChange={handle} placeholder="Ulica i broj" />
            </div>
          </div>

          <h3>O kompaniji</h3>
          <div className="form-grid cols-1">
            <div className="form-group">
              <label>Opis</label>
              <textarea
                name="opis"
                value={form.opis}
                onChange={handle}
                placeholder="Kratki opis vaše kompanije..."
                style={{ minHeight: 120 }}
              />
            </div>
          </div>

          <div className="form-actions">
            <button type="submit" className="btn-primary">
              {saved ? "✓ Sačuvano" : "Sačuvaj izmene"}
            </button>
            {saved && (
              <span style={{ fontSize: 13, color: "var(--success)" }}>
                Profil uspešno ažuriran
              </span>
            )}
          </div>
        </div>
      </form>
    </div>
  );
}