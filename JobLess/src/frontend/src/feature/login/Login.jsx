import { useState, useCallback } from "react";
import { useNavigate } from "react-router-dom";
import { useAuth } from "../../context/AuthContext";
import { syncClientProfileAfterAuth } from "../../api/clientApi";
import { isCandidateRole } from "../dashboard/user/profileUtils";

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
  }
};

// ============================================================
// SCHEMAS
// ============================================================
const schemas = {
  login: {
    email: [v.required, v.email],
    password: [v.required],
  },
 
  register: {
    email: [v.required, v.email],
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
  const { field, err, validate, values } = useForm(
    { email: "", password: "" },
    schemas.login
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
    schemas.login
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
  const { field, err, validate , values} = useForm(
    {
      email: "",  password: "", confirmPassword: "",
    },
    schemas.register
  );
  formRef.current = { validate , values};
  return (
    <>
      <div className="form-group">
        <label className="form-label">Email adresa</label>
        <input className={`form-input${err("email") ? " input-error" : ""}`} type="email" placeholder="vase.ime@email.com" {...field("email")} />
        <FieldError msg={err("email")} />
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
  const { field, err, validate , values} = useForm(
    {
      email: "",  password: "", confirmPassword: "",
    },
    schemas.register
  );
  formRef.current = { validate , values};

  return (
    <>
      <div className="form-group">
        <label className="form-label">Email kompanije</label>
        <input className={`form-input${err("email") ? " input-error" : ""}`} type="email" placeholder="hr@kompanija.rs" {...field("email")} />
        <FieldError msg={err("email")} />
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

    try {
      if (isLogin) {
        const res = await fetch("/api/Auth/login", {
          method: "POST",
          headers: { "Content-Type": "application/json" },
          body: JSON.stringify({ email: values.email, password: values.password }),
        });
        const data = await res.json();
        if (!res.ok) {
          setServerError(data.message ?? "Pogrešni kredencijali.");
          return;
        }
        login(data);
        if (isCandidateRole(data.role)) {
          await syncClientProfileAfterAuth(data.email);
          navigate("/user");
        } else {
          navigate("/company");
        }
      } else {
        const res = await fetch("/api/Auth/register", {
          method: "POST",
          headers: { "Content-Type": "application/json" },
          body: JSON.stringify({
            email: values.email,
            password: values.password,
            role: role === "company" ? "Company" : "Candidate",
          }),
        });
        const data = await res.json();
        if (!res.ok) {
          setServerError(data.message ?? "Greška pri registraciji.");
          return;
        }
        login(data);
        if (isCandidateRole(data.role)) {
          await syncClientProfileAfterAuth(data.email);
          navigate("/user");
        } else {
          navigate("/company");
        }
      }
    } catch {
      setServerError("Nije moguće povezati se sa serverom.");
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