import { useState,useCallback } from "react";
function useToast() {
  const [toasts, setToasts] = useState([]);

  const show = useCallback((message, type = "info") => {
    const id = Date.now();
    setToasts((t) => [...t, { id, message, type }]);
    setTimeout(() => {
      setToasts((t) => t.filter((x) => x.id !== id));
    }, 3000);
  }, []);

  return { toasts, show };
}

function ToastPortal({ toasts }) {
  if (!toasts.length) return null;
  return (
    <div className="toast-portal">
      {toasts.map((t) => (
        <div key={t.id} className={`toast toast-${t.type}`}>
          {t.message}
        </div>
      ))}
    </div>
  );
}
const INITIAL = {
  Naslov: "",
  Pozicija: "",
  Opis: "",
  TipZaposlenja: 0,
  RadnoVreme: 0,
  Senioritet: 0,
  IskustvoMin: "",
  IskustvoMax: "",
  TipRada: 0,
  Grad: "Beograd",
  Drzava: "Srbija",
  PlataOd: "",
  PlataDo: "",
  Valuta: "RSD",
  PlataVidljiva: false,
  DatumIsteka: "",
};

export default function CreateJob({ onSuccess }) {
  const [form, setForm] = useState(INITIAL);
  const [submitting, setSubmitting] = useState(false);
  const { toasts, show: showToast } = useToast();

  const handle = (e) => {
    const { name, value, type, checked } = e.target;

    setForm((prev) => ({
      ...prev,
      [name]:
        type === "checkbox"
          ? checked
          : ["TipZaposlenja", "RadnoVreme", "Senioritet", "TipRada"].includes(name)
          ? Number(value)
          : value,
    }));
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    setSubmitting(true);

    try {
      const payload = {
        CompanyId: 1,
        Title: form.Naslov,
        Description: form.Opis || null,
        Position: form.Pozicija,

        ExpiresAt: form.DatumIsteka
          ? new Date(form.DatumIsteka).toISOString()
          : null,

        EmploymentType: Number(form.TipZaposlenja),
        WorkSchedule: Number(form.RadnoVreme),
        SeniorityLevel: Number(form.Senioritet),
        WorkType: Number(form.TipRada),

        MinExperience: form.IskustvoMin === "" ? null : Number(form.IskustvoMin),
        MaxExperience: form.IskustvoMax === "" ? null : Number(form.IskustvoMax),

        City: form.Grad,
        Country: form.Drzava || null,

        SalaryFrom: form.PlataOd === "" ? null : Number(form.PlataOd),
        SalaryTo: form.PlataDo === "" ? null : Number(form.PlataDo),

        Currency: form.Valuta || null,
        IsSalaryVisible: form.PlataVidljiva,
      };

      const response = await fetch("/api/Advertisements", {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(payload),
      });

      if (!response.ok) {
        const errorText = await response.text();
        throw new Error(errorText);
      }
      setForm(INITIAL);
      showToast("Oglas je kreiran.", "success");
      onSuccess?.();
    } catch (err) {
      console.error(err);
      alert(err.message);
    } finally {
      setSubmitting(false);
    }
  };

  return (
    <div style={{ maxWidth: 680, margin: "0 auto", padding: "2rem 1rem", fontFamily: "system-ui, sans-serif" }}>
      <h2 style={{ marginBottom: "1.5rem" }}>Kreiraj oglas</h2>

      <form onSubmit={handleSubmit}>
        <div className="form-card">

          {/* ── Osnovno ── */}
          <fieldset style={fs}>
            <legend style={leg}>Osnovno</legend>

            <div className="form-group">
              <label>Naslov *</label>
              <input name="Naslov" value={form.Naslov} onChange={handle} required />
            </div>

            <div className="form-group">
              <label>Pozicija *</label>
              <input name="Pozicija" value={form.Pozicija} onChange={handle} required />
            </div>

            <div className="form-group">
              <label>Opis</label>
              <textarea name="Opis" value={form.Opis} onChange={handle} rows={4} />
            </div>

            <div className="form-group">
              <label>Datum isteka oglasa</label>
              <input
                type="date"
                name="DatumIsteka"
                value={form.DatumIsteka}
                onChange={handle}
                min={new Date().toISOString().split("T")[0]}
              />
            </div>
          </fieldset>

          {/* ── Vrsta posla ── */}
          <fieldset style={fs}>
            <legend style={leg}>Vrsta posla</legend>

            <div className="form-group">
              <label>Tip zaposlenja</label>
              <select name="TipZaposlenja" value={form.TipZaposlenja} onChange={handle}>
                <option value={0}>Stalni</option>
                <option value={1}>Honorarni</option>
                <option value={2}>Praksa</option>
              </select>
            </div>

            <div className="form-group">
              <label>Radno vreme</label>
              <select name="RadnoVreme" value={form.RadnoVreme} onChange={handle}>
                <option value={0}>Puno</option>
                <option value={1}>Nepuno</option>
              </select>
            </div>

            <div className="form-group">
              <label>Tip rada</label>
              <select name="TipRada" value={form.TipRada} onChange={handle}>
                <option value={0}>OnSite</option>
                <option value={1}>Remote</option>
                <option value={2}>Hybrid</option>
              </select>
            </div>

            <div className="form-group">
              <label>Senioritet</label>
              <select name="Senioritet" value={form.Senioritet} onChange={handle}>
                <option value={0}>Početnik</option>
                <option value={1}>Junior</option>
                <option value={2}>Medior</option>
                <option value={3}>Senior</option>
              </select>
            </div>
          </fieldset>

          {/* ── Iskustvo ── */}
          <fieldset style={fs}>
            <legend style={leg}>Iskustvo (godine)</legend>

            <div style={{ display: "grid", gridTemplateColumns: "1fr 1fr", gap: "1rem" }}>
              <div className="form-group">
                <label>Minimum</label>
                <input
                  type="number"
                  name="IskustvoMin"
                  value={form.IskustvoMin}
                  onChange={handle}
                  min={0}
                  placeholder="npr. 1"
                />
              </div>

              <div className="form-group">
                <label>Maksimum</label>
                <input
                  type="number"
                  name="IskustvoMax"
                  value={form.IskustvoMax}
                  onChange={handle}
                  min={0}
                  placeholder="npr. 5"
                />
              </div>
            </div>
          </fieldset>

          {/* ── Lokacija ── */}
          <fieldset style={fs}>
            <legend style={leg}>Lokacija</legend>

            <div style={{ display: "grid", gridTemplateColumns: "1fr 1fr", gap: "1rem" }}>
              <div className="form-group">
                <label>Grad *</label>
                <input name="Grad" value={form.Grad} onChange={handle} required />
              </div>

              <div className="form-group">
                <label>Država</label>
                <input name="Drzava" value={form.Drzava} onChange={handle} />
              </div>
            </div>
          </fieldset>

          {/* ── Plata ── */}
          <fieldset style={fs}>
            <legend style={leg}>Plata</legend>

            <div style={{ display: "grid", gridTemplateColumns: "1fr 1fr 1fr", gap: "1rem" }}>
              <div className="form-group">
                <label>Od</label>
                <input
                  type="number"
                  name="PlataOd"
                  value={form.PlataOd}
                  onChange={handle}
                  min={0}
                  placeholder="npr. 50000"
                />
              </div>

              <div className="form-group">
                <label>Do</label>
                <input
                  type="number"
                  name="PlataDo"
                  value={form.PlataDo}
                  onChange={handle}
                  min={0}
                  placeholder="npr. 100000"
                />
              </div>

              <div className="form-group">
                <label>Valuta</label>
                <select name="Valuta" value={form.Valuta} onChange={handle}>
                  <option value="RSD">RSD</option>
                  <option value="EUR">EUR</option>
                  <option value="USD">USD</option>
                </select>
              </div>
            </div>

            <div className="form-group" style={{ marginTop: "0.75rem" }}>
  <label className="form-toggle">
    <input
      type="checkbox"
      name="PlataVidljiva"
      checked={form.PlataVidljiva}
      onChange={handle}
    />
    <span>Prikaži platu kandidatima</span>
  </label>
</div>
          </fieldset>

          <button type="submit" className="btn-primary" disabled={submitting} style={{ marginTop: "1rem" }}>
            {submitting ? "Objavljivanje..." : "Objavi oglas"}
          </button>

        </div>
      </form>
    </div>
  );
}

// inline styles za fieldset / legend
const fs = {
  border: "1px solid #e2e8f0",
  borderRadius: 8,
  padding: "1rem 1.25rem",
  marginBottom: "1.25rem",
};
const leg = {
  fontWeight: 600,
  fontSize: "0.9rem",
  padding: "0 0.4rem",
  color: "#334155",
};