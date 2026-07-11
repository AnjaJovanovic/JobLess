import { useState, useEffect } from "react";
import { useAuth } from "../../../context/AuthContext";

// Mapiranje string labele -> broj koji backend koristi za GetCompanySize()
const companySizeEmployeeMap = {
    "1-10": 10,
    "11-50": 50,
    "51-200": 200,
    "201-500": 500,
    "500+": 501,
};

// Mapiranje enum vrednosti iz baze -> string za <select>
const companySizeReverseMap = {
    1: "1-10",
    2: "11-50",
    3: "51-200",
    4: "201-500",
    5: "500+",
};

const industryReverseMap = {
    1: "Informacione tehnologije",
    2: "Finansije i bankarstvo",
    3: "Maloprodaja i usluge",
    4: "Industrija i proizvodnja",
    5: "Zdravstvo",
    6: "Građevinarstvo",
    7: "Mediji i marketing",
    8: "Ostalo",
};

export default function CompanyProfile() {
    const { user } = useAuth();
    const companyId = user?.id;
    const token = user?.accessToken;

    const [readonly, setReadonly] = useState({
        email: "",
        pib: "",
        mb: "",
        industry: "",
    });

    const [form, setForm] = useState({
        naziv: "",
        opis: "",
        website: "",
        grad: "",
        adresa: "",
        ownerName: "",
        telefon: "",
        companySize: "",
        contactFirstName: "",
        contactLastName: "",
        contactPosition: "",
        contactPhone: "",
    });

    const [loading, setLoading] = useState(true);
    const [saving, setSaving] = useState(false);
    const [saved, setSaved] = useState(false);
    const [error, setError] = useState(null);

    useEffect(() => {
        if (!companyId) {
            setError("Niste prijavljeni kao kompanija.");
            setLoading(false);
            return;
        }

        fetch(`/api/Companies/One?id=${companyId}`, {
            headers: { Authorization: `Bearer ${token}` },
        })
            .then((res) => {
                if (!res.ok) throw new Error("Greška pri učitavanju profila.");
                return res.json();
            })
            .then((data) => {
                const c = data.company;
                setReadonly({
                    email: c.email ?? "",
                    pib: c.taxIdentificationNumber ?? "",
                    mb: c.registrationNumber ?? "",
                    industry: industryReverseMap[c.industry] ?? c.industry ?? "",
                });
                setForm({
                    naziv: c.name ?? "",
                    opis: c.description ?? "",
                    website: c.website ?? "",
                    grad: c.location ?? "",
                    adresa: c.address ?? "",
                    ownerName: c.ownerName ?? "",
                    telefon: c.phoneNumber ?? "",
                    companySize: companySizeReverseMap[c.companySize] ?? "",
                    contactFirstName: c.contactPersonFirstName ?? "",
                    contactLastName: c.contactPersonLastName ?? "",
                    contactPosition: c.contactPersonPosition ?? "",
                    contactPhone: c.contactPersonPhoneNumber ?? "",
                });
            })
            .catch((err) => setError(err.message))
            .finally(() => setLoading(false));
    }, [companyId]);

    const handle = (e) => {
        setForm((f) => ({ ...f, [e.target.name]: e.target.value }));
        setSaved(false);
    };

    const handleSubmit = async (e) => {
        e.preventDefault();
        setSaving(true);
        setError(null);

        const phonePattern = /^\+[0-9][0-9\s\-()/]{5,18}$/;
        if (form.telefon?.trim() && !phonePattern.test(form.telefon.trim())) {
            setError("Broj telefona mora biti u formatu +381 60 123 4567.");
            setSaving(false);
            return;
        }
        if (form.contactPhone?.trim() && !phonePattern.test(form.contactPhone.trim())) {
            setError("Telefon kontakt osobe mora biti u formatu +381 60 123 4567.");
            setSaving(false);
            return;
        }

        try {
            const response = await fetch("/api/Companies/Update", {
                method: "PUT",
                headers: { 
                    "Content-Type": "application/json" ,
                    Authorization: `Bearer ${token}`,
                },
                body: JSON.stringify({
                    companyId: companyId,
                    name: form.naziv,
                    description: form.opis || null,
                    website: form.website || null,
                    location: form.grad,
                    address: form.adresa || null,
                    ownerName: form.ownerName || null,
                    phoneNumber: form.telefon || null,
                    employeeCount: companySizeEmployeeMap[form.companySize] ?? null,
                    contactPersonFirstName: form.contactFirstName || null,
                    contactPersonLastName: form.contactLastName || null,
                    contactPersonPosition: form.contactPosition || null,
                    contactPersonPhoneNumber: form.contactPhone || null,
                }),
            });

            if (!response.ok) {
                const text = await response.text();
                try {
                    const json = JSON.parse(text);
                    const firstError = Object.values(json.errors || {})[0]?.[0];
                    throw new Error(firstError || json.message || "Greška pri čuvanju profila.");
                } catch {
                    throw new Error(text || "Greška pri čuvanju profila.");
                }
            }

            setSaved(true);
            setTimeout(() => setSaved(false), 3000);
        } catch (err) {
            setError(err.message);
        } finally {
            setSaving(false);
        }
    };

    if (loading) return <div>Učitavanje profila...</div>;

    const readonlyStyle = { background: "#f5f5f5", cursor: "not-allowed", color: "#666" };

    return (
        <div>
            <h2>Profil kompanije</h2>

            {error && (
                <div style={{ color: "var(--danger, red)", marginBottom: "1rem", padding: "0.75rem", background: "#fff0f0", borderRadius: 6 }}>
                    {error}
                </div>
            )}

            <form onSubmit={handleSubmit}>

                {/* ── READONLY ── */}
                <div className="form-card">
                    <h3>Nepromenjive informacije</h3>
                    <p style={{ fontSize: 13, color: "#888", marginBottom: "1rem" }}>
                        Ova polja nije moguće menjati.
                    </p>
                    <div className="form-grid">
                        <div className="form-group">
                            <label>Email</label>
                            <input value={readonly.email} readOnly disabled style={readonlyStyle} />
                        </div>
                        <div className="form-group">
                            <label>Delatnost</label>
                            <input value={readonly.industry} readOnly disabled style={readonlyStyle} />
                        </div>
                        <div className="form-group">
                            <label>PIB</label>
                            <input value={readonly.pib} readOnly disabled style={readonlyStyle} />
                        </div>
                        <div className="form-group">
                            <label>Matični broj</label>
                            <input value={readonly.mb} readOnly disabled style={readonlyStyle} />
                        </div>
                    </div>
                </div>

                {/* ── EDITABLE ── */}
                <div className="form-card">
                    <h3>Osnovne informacije</h3>
                    <div className="form-grid">
                        <div className="form-group form-col-span-2">
                            <label>Naziv kompanije</label>
                            <input name="naziv" value={form.naziv} onChange={handle} placeholder="Naziv kompanije" />
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
                            <label>Ime vlasnika</label>
                            <input name="ownerName" value={form.ownerName} onChange={handle} placeholder="Ime i prezime" />
                        </div>
                        <div className="form-group">
                            <label>Veličina kompanije</label>
                            <select name="companySize" value={form.companySize} onChange={handle}>
                                <option value="">Odaberite</option>
                                <option value="1-10">1–10</option>
                                <option value="11-50">11–50</option>
                                <option value="51-200">51–200</option>
                                <option value="201-500">201–500</option>
                                <option value="500+">500+</option>
                            </select>
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

                    <h3>Kontakt osoba</h3>
                    <div className="form-grid">
                        <div className="form-group">
                            <label>Ime</label>
                            <input name="contactFirstName" value={form.contactFirstName} onChange={handle} placeholder="Marko" />
                        </div>
                        <div className="form-group">
                            <label>Prezime</label>
                            <input name="contactLastName" value={form.contactLastName} onChange={handle} placeholder="Petrović" />
                        </div>
                        <div className="form-group">
                            <label>Pozicija</label>
                            <input name="contactPosition" value={form.contactPosition} onChange={handle} placeholder="HR Menadžer" />
                        </div>
                        <div className="form-group">
                            <label>Telefon kontakt osobe</label>
                            <input name="contactPhone" value={form.contactPhone} onChange={handle} placeholder="+381..." />
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
