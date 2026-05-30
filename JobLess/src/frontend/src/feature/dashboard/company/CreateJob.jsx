import { useState, useCallback } from "react";


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


function validate(form) {
  const errors = {};

  if (!form.Naslov?.trim())
    errors.Naslov = "Naslov je obavezan.";
  else if (form.Naslov.trim().length < 3)
    errors.Naslov = "Naslov mora imati najmanje 3 karaktera.";

  if (!form.Pozicija?.trim())
    errors.Pozicija = "Pozicija je obavezna.";

  if (!form.Grad?.trim())
    errors.Grad = "Grad je obavezan.";

  if (form.DatumIsteka) {
    const expiry = new Date(form.DatumIsteka);
    const today = new Date();
    today.setHours(0, 0, 0, 0);
    if (expiry < today)
      errors.DatumIsteka = "Datum isteka ne može biti u prošlosti.";
  }

  const min = form.IskustvoMin !== "" && form.IskustvoMin != null ? Number(form.IskustvoMin) : null;
  const max = form.IskustvoMax !== "" && form.IskustvoMax != null ? Number(form.IskustvoMax) : null;

  if (min !== null && min < 0)
    errors.IskustvoMin = "Iskustvo ne može biti negativno.";
  if (max !== null && max < 0)
    errors.IskustvoMax = "Iskustvo ne može biti negativno.";
  if (min !== null && max !== null && min > max)
    errors.IskustvoMin = "Minimalno iskustvo ne može biti veće od maksimalnog.";

  const od = form.PlataOd !== "" && form.PlataOd != null ? Number(form.PlataOd) : null;
  const do_ = form.PlataDo !== "" && form.PlataDo != null ? Number(form.PlataDo) : null;

  if (od !== null && od < 0)
    errors.PlataOd = "Plata ne može biti negativna.";
  if (do_ !== null && do_ < 0)
    errors.PlataDo = "Plata ne može biti negativna.";
  if (od !== null && do_ !== null && od > do_)
    errors.PlataOd = "Plata od ne može biti veća od plate do.";

  if (form.PlataVidljiva && !form.Valuta)
    errors.Valuta = "Valuta je obavezna kada je plata vidljiva.";

  return errors;
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
  const [errors, setErrors] = useState({});
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

    if (errors[name]) {
      setErrors((prev) => ({ ...prev, [name]: undefined }));
    }
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    const errs = validate(form);
    if (Object.keys(errs).length > 0) {
      setErrors(errs);
      return;
    }
    setErrors({});
    setSubmitting(true);

    try {
      const payload = {
        CompanyId: 1,
        Title: form.Naslov,
        Description: form.Opis || null,
        Position: form.Pozicija,
        ExpiresAt: form.DatumIsteka ? new Date(form.DatumIsteka).toISOString() : null,
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

  
          <fieldset style={fs}>
            <legend style={leg}>Osnovno</legend>

            <div className="form-group">
              <label>Naslov *</label>
              <input
                name="Naslov"
                value={form.Naslov}
                onChange={handle}
                className={errors.Naslov ? "input-error" : ""}
              />
              {errors.Naslov && <span className="field-error">{errors.Naslov}</span>}
            </div>

            <div className="form-group">
              <label>Pozicija *</label>
              <input
                name="Pozicija"
                value={form.Pozicija}
                onChange={handle}
                className={errors.Pozicija ? "input-error" : ""}
              />
              {errors.Pozicija && <span className="field-error">{errors.Pozicija}</span>}
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
                className={errors.DatumIsteka ? "input-error" : ""}
              />
              {errors.DatumIsteka && <span className="field-error">{errors.DatumIsteka}</span>}
            </div>
          </fieldset>

  
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
                  className={errors.IskustvoMin ? "input-error" : ""}
                />
                {errors.IskustvoMin && <span className="field-error">{errors.IskustvoMin}</span>}
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
                  className={errors.IskustvoMax ? "input-error" : ""}
                />
                {errors.IskustvoMax && <span className="field-error">{errors.IskustvoMax}</span>}
              </div>
            </div>
          </fieldset>

          <fieldset style={fs}>
            <legend style={leg}>Lokacija</legend>

            <div style={{ display: "grid", gridTemplateColumns: "1fr 1fr", gap: "1rem" }}>
              <div className="form-group">
                <label>Grad *</label>
                <input
                  name="Grad"
                  value={form.Grad}
                  onChange={handle}
                  className={errors.Grad ? "input-error" : ""}
                />
                {errors.Grad && <span className="field-error">{errors.Grad}</span>}
              </div>

              <div className="form-group">
                <label>Država</label>
                <input name="Drzava" value={form.Drzava} onChange={handle} />
              </div>
            </div>
          </fieldset>

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
                  className={errors.PlataOd ? "input-error" : ""}
                />
                {errors.PlataOd && <span className="field-error">{errors.PlataOd}</span>}
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
                  className={errors.PlataDo ? "input-error" : ""}
                />
                {errors.PlataDo && <span className="field-error">{errors.PlataDo}</span>}
              </div>

              <div className="form-group">
                <label>Valuta</label>
                <select
                  name="Valuta"
                  value={form.Valuta}
                  onChange={handle}
                  className={errors.Valuta ? "input-error" : ""}
                >
                  <option value="">— Izaberi —</option>
                  <option value="RSD">RSD</option>
                  <option value="EUR">EUR</option>
                  <option value="USD">USD</option>
                </select>
                {errors.Valuta && <span className="field-error">{errors.Valuta}</span>}
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

      <ToastPortal toasts={toasts} />
    </div>
  );
}

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