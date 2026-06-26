import { useState, useCallback } from "react";
import { useNavigate } from "react-router-dom";
import { useAuth } from "../../context/AuthContext";
import { syncClientProfileAfterAuth, createClientProfile, storeClientId } from "../../api/clientApi";
import { isCandidateRole } from "../dashboard/user/profileUtils";

import "./Login.css";

const companySizeMap = {
    "1-10": 1,
    "11-50": 2,
    "51-200": 3,
    "201-500": 4,
    "500+": 5,
};

// ============================================================
// VALIDATORS
// ============================================================
const v = {
    required: (val) => (!val || !val.toString().trim() ? "Ovo polje je obavezno." : null),
    email: (val) => {
        if (!val) return "Email je obavezan.";
        return /^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(val) ? null : "Unesite ispravnu email adresu.";
    },
    phone: (val) => {
        if (!val) return "Telefon je obavezan.";
        return /^[0-9+\s\-()/]{6,20}$/.test(val) ? null : "Unesite ispravan broj telefona.";
    },
    phoneOptional: (val) => {
        if (!val || !String(val).trim()) return null;
        return /^[0-9+\s\-()/]{6,20}$/.test(val) ? null : "Unesite ispravan broj telefona.";
    },
    gender: (val) => (val === "" || val === null || val === undefined ? "Pol je obavezan." : null),
    password: (val) => {
        if (!val) return "Lozinka je obavezna.";
        if (val.length < 8) return "Lozinka mora imati najmanje 8 karaktera.";
        if (!/[A-Z]/.test(val)) return "Mora sadržati bar jedno veliko slovo.";
        if (!/[0-9]/.test(val)) return "Mora sadržati bar jedan broj.";
        return null;
    },
    confirmPassword: (val, all) => {
        if (!val) return "Potvrda lozinke je obavezna.";
        return val === all.password ? null : "Lozinke se ne poklapaju.";
    },
    pib: (val) => {
        if (!val) return "PIB je obavezan.";
        return /^\d{9}$/.test(val.trim()) ? null : "PIB mora imati tačno 9 cifara.";
    },
    maticni: (val) => {
        if (!val) return "Matični broj je obavezan.";
        return /^\d{8}$/.test(val.trim()) ? null : "Matični broj mora imati tačno 8 cifara.";
    },
    select: (val) => (!val || val === "" ? "Molimo odaberite opciju." : null),
    url: (val) => {
        if (!val) return null;
        try { new URL(val.startsWith("http") ? val : `https://${val}`); return null; }
        catch { return "Unesite ispravan URL."; }
    },
};

// ============================================================
// SCHEMAS
// ============================================================
const schemas = {
    candidateLogin: {
        email: [v.required, v.email],
        password: [v.required],
    },
    companyLogin: {
        email: [v.required, v.email],
        password: [v.required],
    },
    candidateRegister: {
        firstName: [v.required],
        lastName: [v.required],
        email: [v.required, v.email],
        phoneNumber: [v.phoneOptional],
        gender: [v.gender],
        password: [v.required, v.password],
        confirmPassword: [v.required, v.confirmPassword],
    },
    companyRegister: {
        companyName: [v.required],
        pib: [v.required, v.pib],
        maticniBroj: [v.required, v.maticni],
        email: [v.required, v.email],
        phone: [v.required, v.phone],
        industry: [v.required, v.select],
        employeeCount: [v.required, v.select],
        city: [v.required],
        website: [v.url],
        //contactName: [v.required],
        contactFirstName: [v.required],
        contactLastName: [v.required],
        contactPosition: [v.required],
        password: [v.required, v.password],
        confirmPassword: [v.required, v.confirmPassword],
    },
};

function validateField(name, value, allValues, schema) {
    for (const fn of schema[name] || []) {
        const err = fn(value, allValues);
        if (err) return err;
    }
    return null;
}

function validateAll(values, schema) {
    const errors = {};
    for (const name of Object.keys(schema)) {
        const err = validateField(name, values[name], values, schema);
        if (err) errors[name] = err;
    }
    return errors;
}

// ============================================================
// useForm HOOK
// ============================================================
function useForm(initial, schema) {
  const [values, setValues] = useState(initial);
  const [errors, setErrors] = useState({});
  const [touched, setTouched] = useState({});

  const handleChange = useCallback((e) => {
    const { name, value } = e.target;
    setValues((prev) => {
      const next = { ...prev, [name]: value };
      if (touched[name]) {
        const err = validateField(name, value, next, schema);
        setErrors((pe) => ({ ...pe, [name]: err }));
      }
      return next;
    });
  }, [touched, schema]);

  const handleBlur = useCallback((e) => {
    const { name, value } = e.target;
    setTouched((prev) => ({ ...prev, [name]: true }));
    const err = validateField(name, value, values, schema);
    setErrors((prev) => ({ ...prev, [name]: err }));
  }, [values, schema]);

  const validate = useCallback(() => {
    const allTouched = Object.keys(schema).reduce((a, k) => ({ ...a, [k]: true }), {});
    setTouched(allTouched);
    const errs = validateAll(values, schema);
    setErrors(errs);
    return Object.values(errs).every((e) => !e);
  }, [values, schema]);

  const field = (name) => ({
    name,
    value: values[name] ?? "",
    onChange: handleChange,
    onBlur: handleBlur,
  });

  const err = (name) => (touched[name] ? errors[name] : null);

  return { field, err, validate , values};
}

// ============================================================
// FIELD ERROR
// ============================================================
function FieldError({ msg }) {
  if (!msg) return null;
  return <span className="field-error">{msg}</span>;
}

// ============================================================
// CANDIDATE LOGIN
// ============================================================
function CandidateLogin({ formRef }) {
    const { field, err, validate, values } = useForm(
        { email: "", password: "" },
        schemas.candidateLogin
    );
    formRef.current = { validate, values };

  return (
    <>
      <div className="form-group">
        <label className="form-label">Email adresa</label>
        <input className={`form-input${err("email") ? " input-error" : ""}`} type="email" placeholder="vase.ime@email.com" {...field("email")} />
        <FieldError msg={err("email")} />
      </div>
      <div className="form-group">
        <label className="form-label">Lozinka</label>
        <button className="forgot-link" type="button">Zaboravili ste?</button>
        <input className={`form-input${err("password") ? " input-error" : ""}`} type="password" placeholder="••••••••" {...field("password")} />
        <FieldError msg={err("password")} />
      </div>
    </>
  );
}

// ============================================================
// COMPANY LOGIN
// ============================================================
function CompanyLogin({ formRef }) {
  const { field, err, validate, values } = useForm(
    { email: "", password: "" },
    schemas.companyLogin
  );
  formRef.current = { validate, values };

  return (
    <>
      <div className="form-group">
        <label className="form-label">Email kompanije</label>
        <input className={`form-input${err("email") ? " input-error" : ""}`} type="email" placeholder="kontakt@kompanija.rs" {...field("email")} />
        <FieldError msg={err("email")} />
      </div>
      <div className="form-group">
        <label className="form-label">Lozinka</label>
        <button className="forgot-link" type="button">Zaboravili ste?</button>
        <input className={`form-input${err("password") ? " input-error" : ""}`} type="password" placeholder="••••••••" {...field("password")} />
        <FieldError msg={err("password")} />
      </div>
    </>
  );
}

// ============================================================
// CANDIDATE REGISTER
// ============================================================
function CandidateRegister({ formRef }) {
  const { field, err, validate, values } = useForm(
    {
      firstName: "",
      lastName: "",
      email: "",
      phoneNumber: "",
      gender: "",
      password: "",
      confirmPassword: "",
    },
    schemas.candidateRegister
  );
  formRef.current = { validate, values };
  const genderField = field("gender");

  return (
    <>
      <div className="form-section-label">Lični podaci</div>

      <div className="form-row">
        <div className="form-group">
          <label className="form-label">Ime</label>
          <input className={`form-input${err("firstName") ? " input-error" : ""}`} type="text" placeholder="Marko" {...field("firstName")} />
          <FieldError msg={err("firstName")} />
        </div>
        <div className="form-group">
          <label className="form-label">Prezime</label>
          <input className={`form-input${err("lastName") ? " input-error" : ""}`} type="text" placeholder="Petrović" {...field("lastName")} />
          <FieldError msg={err("lastName")} />
        </div>
      </div>

      <div className="form-group">
        <label className="form-label">Email adresa</label>
        <input className={`form-input${err("email") ? " input-error" : ""}`} type="email" placeholder="vase.ime@email.com" {...field("email")} />
        <FieldError msg={err("email")} />
      </div>

      <div className="form-group">
        <label className="form-label">Telefon</label>
        <input className={`form-input${err("phoneNumber") ? " input-error" : ""}`} type="tel" placeholder="+381 60 123 4567" {...field("phoneNumber")} />
        <FieldError msg={err("phoneNumber")} />
      </div>

      <div className="form-group">
        <span className="form-label">Pol</span>
        <div className="form-row" style={{ gap: "1rem", marginTop: "0.5rem" }}>
          <label className="form-toggle" style={{ flex: 1 }}>
            <input type="radio" name="gender" value="0" checked={values.gender === "0"} onChange={genderField.onChange} onBlur={genderField.onBlur} />
            <span>Muški</span>
          </label>
          <label className="form-toggle" style={{ flex: 1 }}>
            <input type="radio" name="gender" value="1" checked={values.gender === "1"} onChange={genderField.onChange} onBlur={genderField.onBlur} />
            <span>Ženski</span>
          </label>
        </div>
        <FieldError msg={err("gender")} />
      </div>

      <div className="form-section-label">Sigurnost naloga</div>
      <div className="form-group">
        <label className="form-label">Lozinka</label>
        <input className={`form-input${err("password") ? " input-error" : ""}`} type="password" placeholder="Min. 8 karaktera" {...field("password")} />
        <FieldError msg={err("password")} />
      </div>
      <div className="form-group">
        <label className="form-label">Potvrdite lozinku</label>
        <input className={`form-input${err("confirmPassword") ? " input-error" : ""}`} type="password" placeholder="••••••••" {...field("confirmPassword")} />
        <FieldError msg={err("confirmPassword")} />
      </div>
    </>
  );
}

// ============================================================
// COMPANY REGISTER
// ============================================================
function CompanyRegister({ formRef }) {
    const { field, err, validate, values } = useForm(
        {
            companyName: "", pib: "", maticniBroj: "", email: "", phone: "",
            industry: "", employeeCount: "", city: "", website: "",
            contactFirstName: "", contactLastName: "", contactPosition: "", password: "", confirmPassword: "",
        },
        schemas.companyRegister
    );
    formRef.current = { validate, values };

    return (
        <>
            <div className="form-group">
                <label className="form-label">Naziv kompanije</label>
                <input className={`form-input${err("companyName") ? " input-error" : ""}`} type="text" placeholder="npr. Tech Solutions d.o.o." {...field("companyName")} />
                <FieldError msg={err("companyName")} />
            </div>
            <div className="form-row">
                <div className="form-group">
                    <label className="form-label">PIB</label>
                    <input className={`form-input${err("pib") ? " input-error" : ""}`} type="text" placeholder="101234567" {...field("pib")} />
                    <FieldError msg={err("pib")} />
                </div>
                <div className="form-group">
                    <label className="form-label">Matični broj</label>
                    <input className={`form-input${err("maticniBroj") ? " input-error" : ""}`} type="text" placeholder="12345678" {...field("maticniBroj")} />
                    <FieldError msg={err("maticniBroj")} />
                </div>
            </div>
            <div className="form-group">
                <label className="form-label">Email kompanije</label>
                <input className={`form-input${err("email") ? " input-error" : ""}`} type="email" placeholder="hr@kompanija.rs" {...field("email")} />
                <FieldError msg={err("email")} />
            </div>
            <div className="form-group">
                <label className="form-label">Telefon</label>
                <input className={`form-input${err("phone") ? " input-error" : ""}`} type="tel" placeholder="+381 11 123 4567" {...field("phone")} />
                <FieldError msg={err("phone")} />
            </div>

            <div className="form-section-label">Detalji kompanije</div>

            <div className="form-group">
                <label className="form-label">Delatnost</label>
                <select className={`form-input${err("industry") ? " input-error" : ""}`} {...field("industry")}>
                    <option value="">Odaberite delatnost</option>
                    <option value="Informacione tehnologije">Informacione tehnologije</option>
                    <option value="Finansije i bankarstvo">Finansije i bankarstvo</option>
                    <option value="Maloprodaja i usluge">Maloprodaja i usluge</option>
                    <option value="Industrija i proizvodnja">Industrija i proizvodnja</option>
                    <option value="Zdravstvo">Zdravstvo</option>
                    <option value="Građevinarstvo">Građevinarstvo</option>
                    <option value="Mediji i marketing">Mediji i marketing</option>
                    <option value="Ostalo">Ostalo</option>
                </select>
                <FieldError msg={err("industry")} />
            </div>
            <div className="form-row">
                <div className="form-group">
                    <label className="form-label">Broj zaposlenih</label>
                    <select className={`form-input${err("employeeCount") ? " input-error" : ""}`} {...field("employeeCount")}>
                        <option value="">Veličina</option>
                        <option value="1-10">1–10</option>
                        <option value="11-50">11–50</option>
                        <option value="51-200">51–200</option>
                        <option value="201-500">201–500</option>
                        <option value="500+">500+</option>
                    </select>
                    <FieldError msg={err("employeeCount")} />
                </div>
                <div className="form-group">
                    <label className="form-label">Grad</label>
                    <input className={`form-input${err("city") ? " input-error" : ""}`} type="text" placeholder="Beograd" {...field("city")} />
                    <FieldError msg={err("city")} />
                </div>
            </div>
            <div className="form-group">
                <label className="form-label">Web sajt</label>
                <input className={`form-input${err("website") ? " input-error" : ""}`} type="url" placeholder="https://www.kompanija.rs" {...field("website")} />
                <FieldError msg={err("website")} />
            </div>

            <div className="form-section-label">Kontakt osoba</div>

            <div className="form-row">
                <div className="form-group">
                    <label className="form-label">Ime kontakt osobe</label>
                    <input className={`form-input${err("contactFirstName") ? " input-error" : ""}`} type="text" placeholder="Marko" {...field("contactFirstName")} />
                    <FieldError msg={err("contactFirstName")} />
                </div>
                <div className="form-group">
                    <label className="form-label">Prezime kontakt osobe</label>
                    <input className={`form-input${err("contactLastName") ? " input-error" : ""}`} type="text" placeholder="Petrović" {...field("contactLastName")} />
                    <FieldError msg={err("contactLastName")} />
                </div>
            </div>
            <div className="form-group">
                <label className="form-label">Pozicija</label>
                <input className={`form-input${err("contactPosition") ? " input-error" : ""}`} type="text" placeholder="HR Menadžer" {...field("contactPosition")} />
                <FieldError msg={err("contactPosition")} />
            </div>
            <div className="form-section-label">Sigurnost naloga</div>

            <div className="form-group">
                <label className="form-label">Lozinka</label>
                <input className={`form-input${err("password") ? " input-error" : ""}`} type="password" placeholder="Min. 8 karaktera" {...field("password")} />
                <FieldError msg={err("password")} />
            </div>
            <div className="form-group">
                <label className="form-label">Potvrdite lozinku</label>
                <input className={`form-input${err("confirmPassword") ? " input-error" : ""}`} type="password" placeholder="••••••••" {...field("confirmPassword")} />
                <FieldError msg={err("confirmPassword")} />
            </div>
        </>
    );
}

// ============================================================
// AUTH HELPERS
// ============================================================
async function parseAuthResponse(res) {
  const text = await res.text();
  if (!res.ok) {
    if (res.status === 404) {
      throw new Error(
        "Auth servis nije dostupan. Pokrenite: docker compose up --build -d"
      );
    }
    if (res.status === 502 || res.status === 503) {
      throw new Error(
        "Backend servisi nisu dostupni. Proverite da li svi kontejneri rade."
      );
    }
    try {
      const json = JSON.parse(text);
      throw new Error(json.message || "Greška pri autentifikaciji.");
    } catch (err) {
      if (err instanceof SyntaxError) {
        throw new Error(text || "Greška pri autentifikaciji.");
      }
      throw err;
    }
  }
  return text ? JSON.parse(text) : null;
}

async function authRequest(path, body) {
  const res = await fetch(path, {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify(body),
  });
  return parseAuthResponse(res);
}

async function findCompanyIdByEmail(email) {
  const res = await fetch(
    `/api/Companies/Search?query=${encodeURIComponent(email)}&pageNumber=1&pageSize=20`
  );
  if (!res.ok) return null;

  const data = await res.json();
  const companies = data.companies || [];
  const match = companies.find(
    (c) => c.email?.toLowerCase() === email.toLowerCase()
  );
  return match?.id ?? null;
}

// ============================================================
// MAIN COMPONENT
// ============================================================
export default function Login() {
  const [mode, setMode] = useState("login");
  const [role, setRole] = useState("candidate");
  const [serverError, setServerError] = useState(null);
  const [loading, setLoading] = useState(false);
  const navigate = useNavigate();

  const { login } = useAuth();

  const isLogin = mode === "login";
  const isCandidate = role === "candidate";

  // formRef daje pristup validate() funkciji aktivne pod-forme
  const formRef = { current: null };

  const handleSubmit = async (e) => {
    e.preventDefault();
    setServerError(null);

    const isValid = formRef.current?.validate?.();
    if (!isValid) return;

    const values = formRef.current?.values ?? {};
    setLoading(true);

    try {
      if (isLogin) {
        const data = await authRequest("/api/Auth/login", {
          email: values.email,
          password: values.password,
        });

        if (!data) {
          setServerError("Pogrešni kredencijali.");
          return;
        }

        if (isCandidateRole(data.role)) {
          login(data);
          await syncClientProfileAfterAuth(data.email);
          navigate("/user");
          return;
        }

        const companyId = await findCompanyIdByEmail(data.email);
        login({ ...data, id: companyId, companyId });
        navigate("/company");
        return;
      }

      if (isCandidate) {
        const data = await authRequest("/api/Auth/register", {
          email: values.email,
          password: values.password,
          role: "Candidate",
        });

        login(data);

        const profile = await createClientProfile({
          email: values.email,
          firstName: values.firstName.trim(),
          lastName: values.lastName.trim(),
          phoneNumber: values.phoneNumber.trim() || null,
          gender: Number(values.gender),
          isActive: true,
          clientId: 0,
          createdAt: new Date().toISOString(),
        });

        storeClientId(profile.clientId);
        navigate("/user");
        return;
      }

      const payload = {
        name: values.companyName,
        taxIdentificationNumber: values.pib,
        registrationNumber: values.maticniBroj,
        email: values.email,
        phoneNumber: values.phone,
        industry: values.industry,
        companySize: companySizeMap[values.employeeCount],
        location: values.city,
        website: values.website || null,
        contactPersonFirstName: values.contactFirstName,
        contactPersonLastName: values.contactLastName,
        ownerName: `${values.contactFirstName} ${values.contactLastName}`,
        contactPersonPosition: values.contactPosition,
        contactPersonPhoneNumber: values.phone,
        passwordHash: values.password,
        ownerId: 0,
        description: null,
        address: values.city,
      };

      const authData = await authRequest("/api/Auth/register", {
        email: values.email,
        password: values.password,
        role: "Company",
      });

      const res = await fetch("/api/Companies", {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(payload),
      });

      if (!res.ok) {
        const text = await res.text();
        try {
          const json = JSON.parse(text);
          const firstError = Object.values(json.errors || {})[0]?.[0];
          setServerError(firstError || json.message || "Greška pri kreiranju profila kompanije.");
        } catch {
          setServerError(text || "Greška pri kreiranju profila kompanije.");
        }
        return;
      }

      const companyId = await res.json();
      login({ ...authData, id: companyId, companyId });
      navigate("/company");
    } catch (err) {
      setServerError(err.message || "Došlo je do greške.");
    } finally {
      setLoading(false);
    }
  };

  return (
    <>
      <div className="auth-root">
        <div className="auth-card">
          <div className="auth-logo">
            Job<span>Link</span>
          </div>
          <p className="auth-subtitle">
            {isLogin ? "Dobrodošli nazad. Prijavite se da nastavite." : "Kreirajte nalog i pronađite svoju priliku."}
          </p>

          {/* Login / Register toggle */}
          <div className="mode-toggle">
            <button
              className={`mode-btn ${isLogin ? "active" : ""}`}
              onClick={() => { setMode("login"); setServerError(null); }}
            >
              Prijava
            </button>
            <button
              className={`mode-btn ${!isLogin ? "active" : ""}`}
              onClick={() => { setMode("register"); setServerError(null); }}
            >
              Registracija
            </button>
          </div>

          {/* Role tabs */}
          <div className="role-tabs">
            <button
              className={`role-tab ${isCandidate ? "active" : ""}`}
              onClick={() => { setRole("candidate"); setServerError(null); }}
            >
              <span className="role-icon">👤</span>
              Kandidat
            </button>
            <button
              className={`role-tab ${!isCandidate ? "active" : ""}`}
              onClick={() => { setRole("company"); setServerError(null); }}
            >
              <span className="role-icon">🏢</span>
              Kompanija
            </button>
          </div>

          {/* Server greška */}
          {serverError && (
            <div className="server-error" role="alert">{serverError}</div>
          )}

          {/* Form */}
          <form onSubmit={handleSubmit} noValidate>
            {isLogin
              ? isCandidate
                ? <CandidateLogin formRef={formRef} />
                : <CompanyLogin formRef={formRef} />
              : isCandidate
                ? <CandidateRegister formRef={formRef} />
                : <CompanyRegister formRef={formRef} />
            }

            <button type="submit" className="submit-btn" disabled={loading}>
              {loading ? "Molimo sačekajte..." : isLogin ? "Prijavite se" : "Kreirajte nalog"}
            </button>
          </form>

          <div className="toggle-link">
            {isLogin ? (
              <>
                Nemate nalog?
                <button onClick={() => { setMode("register"); setServerError(null); }}>Registrujte se</button>
              </>
            ) : (
              <>
                Već imate nalog?
                <button onClick={() => { setMode("login"); setServerError(null); }}>Prijavite se</button>
              </>
            )}
          </div>
        </div>
      </div>
    </>
  );
}
