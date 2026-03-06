import { useState, useCallback } from "react";
import { useNavigate } from "react-router-dom";
import "./Login.css";

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
    pibOrMaticni: [v.required],
    email: [v.required, v.email],
    password: [v.required],
  },
  candidateRegister: {
    firstName: [v.required],
    lastName: [v.required],
    email: [v.required, v.email],
    phone: [v.required, v.phone],
    education: [v.required, v.select],
    workArea: [v.required, v.select],
    experience: [v.required, v.select],
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
    contactName: [v.required],
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

  return { field, err, validate };
}

// ============================================================
// FIELD ERROR — minimalna komponenta
// ============================================================
function FieldError({ msg }) {
  if (!msg) return null;
  return <span className="field-error">{msg}</span>;
}

// ============================================================
// CANDIDATE LOGIN
// ============================================================
function CandidateLogin({ formRef }) {
  const { field, err, validate } = useForm(
    { email: "", password: "" },
    schemas.candidateLogin
  );
  formRef.current = { validate };

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
  const { field, err, validate } = useForm(
    { pibOrMaticni: "", email: "", password: "" },
    schemas.companyLogin
  );
  formRef.current = { validate };

  return (
    <>
      <div className="form-group">
        <label className="form-label">PIB / Matični broj</label>
        <input className={`form-input${err("pibOrMaticni") ? " input-error" : ""}`} type="text" placeholder="npr. 101234567" {...field("pibOrMaticni")} />
        <FieldError msg={err("pibOrMaticni")} />
      </div>
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
  const { field, err, validate } = useForm(
    { firstName: "", lastName: "", email: "", phone: "", education: "", workArea: "", experience: "", password: "", confirmPassword: "" },
    schemas.candidateRegister
  );
  formRef.current = { validate };

  return (
    <>
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
        <label className="form-label">Broj telefona</label>
        <input className={`form-input${err("phone") ? " input-error" : ""}`} type="tel" placeholder="+381 60 123 4567" {...field("phone")} />
        <FieldError msg={err("phone")} />
      </div>

      <div className="form-section-label">Profesionalni podaci</div>

      <div className="form-group">
        <label className="form-label">Stručna sprema</label>
        <select className={`form-input${err("education") ? " input-error" : ""}`} {...field("education")}>
          <option value="">Odaberite stepen obrazovanja</option>
          <option value="highschool">Srednja škola</option>
          <option value="higher">Viša škola / strukovne studije</option>
          <option value="bachelor">Osnovne akademske studije</option>
          <option value="master">Master / Magistratura</option>
          <option value="phd">Doktorat</option>
        </select>
        <FieldError msg={err("education")} />
      </div>
      <div className="form-group">
        <label className="form-label">Oblast rada</label>
        <select className={`form-input${err("workArea") ? " input-error" : ""}`} {...field("workArea")}>
          <option value="">Odaberite oblast</option>
          <option value="it">IT i tehnologija</option>
          <option value="marketing">Marketing i PR</option>
          <option value="finance">Finansije i računovodstvo</option>
          <option value="law">Pravo</option>
          <option value="engineering">Inžinjerstvo</option>
          <option value="medicine">Medicina i farmacija</option>
          <option value="education">Obrazovanje</option>
          <option value="other">Ostalo</option>
        </select>
        <FieldError msg={err("workArea")} />
      </div>
      <div className="form-group">
        <label className="form-label">Godine iskustva</label>
        <select className={`form-input${err("experience") ? " input-error" : ""}`} {...field("experience")}>
          <option value="">Odaberite iskustvo</option>
          <option value="0">Bez iskustva (Junior)</option>
          <option value="1-2">1–2 godine</option>
          <option value="3-5">3–5 godina</option>
          <option value="6-10">6–10 godina</option>
          <option value="10+">10+ godina</option>
        </select>
        <FieldError msg={err("experience")} />
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
  const { field, err, validate } = useForm(
    {
      companyName: "", pib: "", maticniBroj: "", email: "", phone: "",
      industry: "", employeeCount: "", city: "", website: "",
      contactName: "", contactPosition: "", password: "", confirmPassword: "",
    },
    schemas.companyRegister
  );
  formRef.current = { validate };

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
          <option value="it">Informacione tehnologije</option>
          <option value="finance">Finansije i bankarstvo</option>
          <option value="retail">Maloprodaja i usluge</option>
          <option value="industry">Industrija i proizvodnja</option>
          <option value="health">Zdravstvo</option>
          <option value="construction">Građevinarstvo</option>
          <option value="media">Mediji i marketing</option>
          <option value="other">Ostalo</option>
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
          <label className="form-label">Ime i prezime</label>
          <input className={`form-input${err("contactName") ? " input-error" : ""}`} type="text" placeholder="Kontakt osoba" {...field("contactName")} />
          <FieldError msg={err("contactName")} />
        </div>
        <div className="form-group">
          <label className="form-label">Pozicija</label>
          <input className={`form-input${err("contactPosition") ? " input-error" : ""}`} type="text" placeholder="HR Menadžer" {...field("contactPosition")} />
          <FieldError msg={err("contactPosition")} />
        </div>
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
// MAIN COMPONENT
// ============================================================
export default function Login() {
  const [mode, setMode] = useState("login");
  const [role, setRole] = useState("candidate");
  const [serverError, setServerError] = useState(null);
  const navigate = useNavigate();

  const isLogin = mode === "login";
  const isCandidate = role === "candidate";

  // formRef daje pristup validate() funkciji aktivne pod-forme
  const formRef = { current: null };

  const handleSubmit = (e) => {
    e.preventDefault();
    setServerError(null);

    const isValid = formRef.current?.validate?.();
    if (!isValid) return;

    // --------------------------------------------------------
    // TODO (backend) — PRIJAVA:
    // Pozovi login API endpoint.
    // Ako korisnik NE POSTOJI:
    //   setServerError("Korisnik sa ovim podacima ne postoji.")
    // Ako je lozinka pogrešna:
    //   setServerError("Pogrešna lozinka.")
    // Ako je uspeh → sačuvaj token i navigiraj.
    //
    // const res = await fetch("/api/auth/login", {
    //   method: "POST",
    //   headers: { "Content-Type": "application/json" },
    //   body: JSON.stringify({ ...values, role }),
    // });
    // const data = await res.json();
    // if (!res.ok) { setServerError(data.message); return; }
    // localStorage.setItem("token", data.token);
    // --------------------------------------------------------

    // --------------------------------------------------------
    // TODO (backend) — REGISTRACIJA:
    // Pozovi register API endpoint.
    // Ako korisnik VEĆ POSTOJI:
    //   setServerError("Korisnik sa ovom email adresom već postoji.")
    //
    // const res = await fetch("/api/auth/register", {
    //   method: "POST",
    //   headers: { "Content-Type": "application/json" },
    //   body: JSON.stringify({ ...values, role }),
    // });
    // const data = await res.json();
    // if (data.errorCode === "USER_EXISTS") {
    //   setServerError("Korisnik sa ovom email adresom već postoji.");
    //   return;
    // }
    // --------------------------------------------------------

    if (role === "company") {
      navigate("/company");
    } else {
      navigate("/user");
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

            <button type="submit" className="submit-btn">
              {isLogin ? "Prijavite se" : "Kreirajte nalog"}
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