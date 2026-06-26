import { useState, useCallback } from "react";
import { useAuth } from "../../../context/AuthContext";

function useToast() {
    const [toasts, setToasts] = useState([]);

    const show = useCallback((message, type = "info") => {
        const id = Date.now();
        setToasts((t) => [...t, { id, message, type }]);
        setTimeout(() => {
            setToasts((t) => t.filter((x) => x.id !== id));
        }, 4000);
    }, []);

    return { toasts, show };
}

function ToastPortal({ toasts }) {
    if (!toasts.length) return null;
    return (
        <div style={{
            position: "fixed",
            bottom: "2rem",
            left: "50%",
            transform: "translateX(-50%)",
            zIndex: 9999,
            display: "flex",
            flexDirection: "column",
            gap: "0.5rem",
            maxWidth: 380,
        }}>
            {toasts.map((t) => (
                <div
                    key={t.id}
                    style={{
                        padding: "0.85rem 1.1rem",
                        borderRadius: 10,
                        fontSize: "0.875rem",
                        fontWeight: 500,
                        lineHeight: 1.5,
                        boxShadow: "0 4px 20px rgba(0,0,0,0.18)",
                        animation: "slideIn 0.25s ease",
                        background:
                            t.type === "success"
                                ? "#16a34a"
                                : t.type === "error"
                                    ? "#dc2626"
                                    : "#2563eb",
                        color: "#fff",
                        display: "flex",
                        alignItems: "flex-start",
                        gap: "0.6rem",
                    }}
                >
                    <span style={{ fontSize: "1rem", flexShrink: 0 }}>
                        {t.type === "success" ? "✓" : t.type === "error" ? "✕" : "ℹ"}
                    </span>
                    <span>{t.message}</span>
                </div>
            ))}
            <style>{`
        @keyframes slideIn {
          from { opacity: 0; transform: translateY(20px); }
          to   { opacity: 1; transform: translateY(0); }
        }
      `}</style>
        </div>
    );
}

// ── Konstante ─────────────────────────────────────────────────
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

// ── Inline stilovi ────────────────────────────────────────────
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

// ── Komponenta ────────────────────────────────────────────────
export default function CreateJob({ onSuccess }) {
    const { user } = useAuth();
    const companyId = user?.companyId ?? user?.id;
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
                companyId: companyId,
                title: form.Naslov,
                description: form.Opis || null,
                position: form.Pozicija,
                expiresAt: form.DatumIsteka ? new Date(form.DatumIsteka).toISOString() : null,
                employmentType: Number(form.TipZaposlenja),
                workSchedule: Number(form.RadnoVreme),
                seniorityLevel: Number(form.Senioritet),
                workType: Number(form.TipRada),
                minExperience: form.IskustvoMin === "" ? null : Number(form.IskustvoMin),
                maxExperience: form.IskustvoMax === "" ? null : Number(form.IskustvoMax),
                city: form.Grad,
                country: form.Drzava || null,
                salaryFrom: form.PlataOd === "" ? null : Number(form.PlataOd),
                salaryTo: form.PlataDo === "" ? null : Number(form.PlataDo),
                currency: form.Valuta || null,
                isSalaryVisible: form.PlataVidljiva,
            };

            if (!companyId) {
                throw new Error("Nije pronađen ID kompanije. Odjavite se i prijavite ponovo.");
            }

            const response = await fetch("/api/Advertisements", {
                method: "POST",
                headers: { "Content-Type": "application/json" },
                body: JSON.stringify(payload),
            });

            if (!response.ok) {
                const errorText = await response.text();

                let friendlyMessage = "Došlo je do greške.";
                try {
                    const parsed = JSON.parse(errorText);
                    if (parsed?.errors) {
                        const messages = Object.values(parsed.errors).flat();
                        friendlyMessage = messages.join("\n");
                    } else if (parsed?.title) {
                        friendlyMessage = parsed.title;
                    }
                } catch {
                    friendlyMessage = errorText.split("\n")[0].replace(/^FluentValidation\.\w+:\s*/i, "").trim();
                    if (!friendlyMessage) friendlyMessage = "Nepoznata greška.";
                }

                throw new Error(friendlyMessage);
            }

            setForm(INITIAL);
            showToast("Oglas je uspešno kreiran!", "success");
            onSuccess?.();
        } catch (err) {
            console.error(err);
            showToast(err.message || "Došlo je do greške.", "error");
        } finally {
            setSubmitting(false);
        }
    };

    return (
        <div style={{ maxWidth: 680, margin: "0 auto", padding: "2rem 1rem", fontFamily: "system-ui, sans-serif" }}>

            <ToastPortal toasts={toasts} />

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