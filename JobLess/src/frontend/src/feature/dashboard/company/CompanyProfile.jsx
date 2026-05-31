import { useState, useEffect } from "react";
import { useAuth } from "../../../context/AuthContext";

export default function CompanyProfile() {
    const { user } = useAuth();
    const companyId = user?.id;

    const [form, setForm] = useState({
        naziv: "",
        email: "",
        telefon: "",
        website: "",
        pib: "",
        mb: "",
        grad: "",
        adresa: "",
        delatnost: "",
        opis: "",
    });

    const [loading, setLoading] = useState(true);
    const [saving, setSaving] = useState(false);
    const [saved, setSaved] = useState(false);
    const [error, setError] = useState(null);

    // ─── Učitaj profil kompanije pri mount-u ───────────────────
    useEffect(() => {
        if (!companyId) {
            setError("Niste prijavljeni kao kompanija.");
            setLoading(false);
            return;
        }

        fetch(`/api/Companies/One?id=${companyId}`)
            .then((res) => {
                if (!res.ok) throw new Error("Greška pri učitavanju profila.");
                return res.json();
            })
            .then((data) => {
                const c = data.company;
                setForm({
                    naziv: c.name ?? "",
                    email: c.email ?? "",
                    telefon: c.phoneNumber ?? "",
                    website: c.website ?? "",
                    pib: c.taxIdentificationNumber ?? "",
                    mb: c.registrationNumber ?? "",
                    grad: c.location ?? "",
                    adresa: c.address ?? "",
                    delatnost: c.industry ?? "",
                    opis: c.description ?? "",
                });
            })
            .catch((err) => setError(err.message))
            .finally(() => setLoading(false));
    }, [companyId]);

    // ─── Handlers ──────────────────────────────────────────────
    const handle = (e) => {
        setForm((f) => ({ ...f, [e.target.name]: e.target.value }));
        setSaved(false);
    };

    const handleSubmit = async (e) => {
        e.preventDefault();
        setSaving(true);
        setError(null);

        try {
            const response = await fetch("/api/Companies/Update", {
                method: "PUT",
                headers: { "Content-Type": "application/json" },
                body: JSON.stringify({
                    companyId: companyId,
                    name: form.naziv,
                    email: form.email,
                    phoneNumber: form.telefon,
                    website: form.website || null,
                    taxIdentificationNumber: form.pib,
                    registrationNumber: form.mb,
                    location: form.grad,
                    address: form.adresa || null,
                    industry: form.delatnost,
                    description: form.opis || null,
                }),
            });

            if (!response.ok) {
                const text = await response.text();
                throw new Error(text || "Greška pri čuvanju profila.");
            }

            setSaved(true);
            setTimeout(() => setSaved(false), 3000);
        } catch (err) {
            setError(err.message);
        } finally {
            setSaving(false);
        }
    };

    // ─── Render ────────────────────────────────────────────────
    if (loading) return <div>Učitavanje profila...</div>;

    return (
        <div>
            <h2>Profil kompanije</h2>

            {error && (
                <div style={{ color: "var(--danger, red)", marginBottom: "1rem", padding: "0.75rem", background: "#fff0f0", borderRadius: 6 }}>
                    {error}
                </div>
            )}

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
                        <button type="submit" className="btn-primary" disabled={saving}>
                            {saving ? "Čuvanje..." : saved ? "✓ Sačuvano" : "Sačuvaj izmene"}
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
