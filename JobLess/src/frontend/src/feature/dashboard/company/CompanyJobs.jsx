import { useState, useEffect, useCallback } from "react";

/* ─── Custom Toast ─────────────────────────────────────────────── */
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

/* ─── Confirm Dialog ────────────────────────────────────────────── */
function ConfirmDialog({ message, onConfirm, onCancel }) {
  return (
    <div className="dialog-overlay" onClick={(e) => e.target === e.currentTarget && onCancel()}>
      <div className="dialog-box">
        <div className="dialog-label">Potvrda</div>
        <div className="dialog-message">{message}</div>
        <div className="dialog-actions">
          <button className="btn-secondary" onClick={onCancel}>Otkaži</button>
          <button className="btn-danger" onClick={onConfirm}>Deaktiviraj</button>
        </div>
      </div>
    </div>
  );
}

/* ─── Main Component ───────────────────────────────────────────── */
export default function CompanyJobs({ onCreateNew }) {
  const [jobs, setJobs] = useState([]);
  const [editing, setEditing] = useState(null);
  const [loading, setLoading] = useState(false);
  const [confirmState, setConfirmState] = useState(null);
  const { toasts, show: showToast } = useToast();

  useEffect(() => {
    fetchJobs();
  }, []);

  const fetchJobs = async () => {
    try {
      setLoading(true);
      const response = await fetch(
        "/api/Advertisements/GetAdvertisementsForCompany?CompanyId=1"
      );
      if (!response.ok) throw new Error(await response.text());
      const data = await response.json();
      const mapped = (data.advertisements || []).map((job) => ({
        Id: job.id,
        Naslov: job.title,
        Pozicija: job.position,
        Grad: job.city,
        Drzava: job.country,
        Opis: job.description,
        DatumIsteka: job.expiresAt ? job.expiresAt.split("T")[0] : "",
        IskustvoMin: job.minExperience,
        IskustvoMax: job.maxExperience,
        PlataOd: job.salaryFrom,
        PlataDo: job.salaryTo,
        Valuta: job.currency,
        PlataVidljiva: Boolean(job.isSalaryVisible),
        Aktivan: job.isActive,
      }));
      setJobs(mapped);
    } catch (err) {
      showToast("Greška pri učitavanju oglasa: " + err.message, "error");
    } finally {
      setLoading(false);
    }
  };

  const handleToggleActive = async (id, trenutnoAktivan) => {
    try {
      if (trenutnoAktivan) {
        const response = await fetch("/api/Advertisements", {
          method: "DELETE",
          headers: { "Content-Type": "application/json" },
          body: JSON.stringify({ id }),
        });
        if (!response.ok) throw new Error(await response.text());
        showToast("Oglas je deaktiviran.", "success");
      } else {
        const response = await fetch(`/api/Advertisements/Activate?Id=${id}`, { method: "PUT" });
        if (!response.ok) throw new Error(await response.text());
        showToast("Oglas je aktiviran.", "success");
      }
      setJobs((j) =>
        j.map((x) => (x.Id === id ? { ...x, Aktivan: !x.Aktivan } : x))
      );
    } catch (err) {
      showToast("Greška pri promeni statusa: " + err.message, "error");
    }
  };

  const handleEditSave = (updated) => {
    setJobs((j) => j.map((x) => (x.Id === updated.Id ? updated : x)));
    setEditing(null);
  };

  if (loading) return <div>Učitavanje oglasa...</div>;

  return (
    <div>
      <h2>Moji oglasi</h2>

      <div className="jobs-toolbar">
        <span className="jobs-count">
          Ukupno: <span>{jobs.length}</span> · Aktivnih:{" "}
          <span>{jobs.filter((j) => j.Aktivan).length}</span>
        </span>
        <button className="btn-primary" onClick={onCreateNew}>
          + Novi oglas
        </button>
      </div>

      {jobs.map((job) => (
        <div className={`company-job-card${!job.Aktivan ? " job-card-inactive" : ""}`} key={job.Id}>
          <div className="cjc-main">
            <div className="cjc-title">{job.Naslov}</div>
            <div className="cjc-meta">
              <span className="cjc-meta-item">📍 {job.Grad}</span>
              {job.PlataVidljiva && (
                <span className="cjc-meta-item cjc-salary">
                  💰 {job.PlataOd?.toLocaleString()}–{job.PlataDo?.toLocaleString()} {job.Valuta}
                </span>
              )}
              <span className={`badge ${job.Aktivan ? "badge-active" : "badge-inactive"}`}>
                {job.Aktivan ? "● Aktivan" : "○ Neaktivan"}
              </span>
            </div>
          </div>

          <div className="cjc-actions">
            <button className="btn-icon" onClick={() => setEditing({ ...job })}>✎</button>
            <button
              className={job.Aktivan ? "btn-danger" : "btn-secondary"}
              onClick={() => {
                if (job.Aktivan) {
                  setConfirmState({ id: job.Id });
                } else {
                  handleToggleActive(job.Id, false);
                }
              }}
            >
              {job.Aktivan ? "Deaktiviraj" : "Aktiviraj"}
            </button>
          </div>
        </div>
      ))}

      {editing && (
        <EditJobModal
          job={editing}
          onSave={handleEditSave}
          onClose={() => setEditing(null)}
        />
      )}

      {confirmState && (
        <ConfirmDialog
          message="Da li sigurno želite da deaktivirate oglas? Oglas neće biti vidljiv kandidatima."
          onConfirm={() => {
            handleToggleActive(confirmState.id, true);
            setConfirmState(null);
          }}
          onCancel={() => setConfirmState(null)}
        />
      )}

      <ToastPortal toasts={toasts} />
    </div>
  );
}

/* ─── Edit Modal ───────────────────────────────────────────────── */
function EditJobModal({ job, onSave, onClose }) {
  const [form, setForm] = useState({ ...job });
  const [saving, setSaving] = useState(false);

  const handle = (e) => {
    const { name, value, type, checked } = e.target;
    setForm((f) => ({
      ...f,
      [name]: type === "checkbox" ? checked : type === "number" ? Number(value) : value,
    }));
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    setSaving(true);
    try {
      const command = {
        id: form.Id,
        title: form.Naslov,
        description: form.Opis,
        position: form.Pozicija,
        city: form.Grad,
        country: form.Drzava,
        expiresAt: form.DatumIsteka ? new Date(form.DatumIsteka).toISOString() : null,
        minExperience: form.IskustvoMin,
        maxExperience: form.IskustvoMax,
        salaryFrom: form.PlataOd,
        salaryTo: form.PlataDo,
        currency: form.Valuta,
        isSalaryVisible: form.PlataVidljiva,
        status: form.Aktivan ? 1 : 0,
      };
      const response = await fetch("/api/Advertisements/Update", {
        method: "PUT",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(command),
      });
      if (!response.ok) throw new Error(await response.text());
      onSave(form);
    } catch (err) {
      alert("Greška pri izmeni: " + err.message);
    } finally {
      setSaving(false);
    }
  };

  return (
    <div className="modal-overlay" onClick={(e) => e.target === e.currentTarget && onClose()}>
      <div className="modal-box">
        <div className="modal-header">
          <h3>Izmeni oglas</h3>
          <button className="modal-close" onClick={onClose}>✕</button>
        </div>

        <form onSubmit={handleSubmit} className="form-grid cols-2">
          <div className="form-group">
            <label>Naslov</label>
            <input name="Naslov" value={form.Naslov || ""} onChange={handle} />
          </div>
          <div className="form-group">
            <label>Pozicija</label>
            <input name="Pozicija" value={form.Pozicija || ""} onChange={handle} />
          </div>
          <div className="form-group form-col-span-2">
            <label>Opis</label>
            <textarea name="Opis" value={form.Opis || ""} onChange={handle} />
          </div>
          <div className="form-group">
            <label>Grad</label>
            <input name="Grad" value={form.Grad || ""} onChange={handle} />
          </div>
          <div className="form-group">
            <label>Država</label>
            <input name="Drzava" value={form.Drzava || ""} onChange={handle} />
          </div>
          <div className="form-group">
            <label>Datum isteka</label>
            <input type="date" name="DatumIsteka" value={form.DatumIsteka || ""} onChange={handle} />
          </div>
          <div className="form-group">
            <label>Min iskustvo</label>
            <input type="number" name="IskustvoMin" value={form.IskustvoMin || ""} onChange={handle} />
          </div>
          <div className="form-group">
            <label>Max iskustvo</label>
            <input type="number" name="IskustvoMax" value={form.IskustvoMax || ""} onChange={handle} />
          </div>
          <div className="form-group">
            <label>Plata od</label>
            <input type="number" name="PlataOd" value={form.PlataOd || ""} onChange={handle} />
          </div>
          <div className="form-group">
            <label>Plata do</label>
            <input type="number" name="PlataDo" value={form.PlataDo || ""} onChange={handle} />
          </div>
          <div className="form-group">
            <label>Valuta</label>
            <select name="Valuta" value={form.Valuta} onChange={handle}>
              <option value="RSD">RSD</option>
              <option value="EUR">EUR</option>
              <option value="USD">USD</option>
            </select>
          </div>
          <div className="form-group form-col-span-2">
            <label className="form-toggle">
              <input
                type="checkbox"
                name="PlataVidljiva"
                checked={Boolean(form.PlataVidljiva)}
                onChange={handle}
              />
              <span>Plata vidljiva</span>
            </label>
          </div>
          <div className="form-actions form-col-span-2">
            <button type="button" className="btn-secondary" onClick={onClose}>Otkaži</button>
            <button type="submit" className="btn-primary" disabled={saving}>
              {saving ? "Čuvanje..." : "Sačuvaj izmene"}
            </button>
          </div>
        </form>
      </div>
    </div>
  );
}