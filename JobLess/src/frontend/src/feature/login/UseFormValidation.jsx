// ============================================================
// VALIDATION UTILITIES
// ============================================================

export const validators = {
    required: (value) => (!value || !value.toString().trim() ? "Ovo polje je obavezno." : null),
    email: (value) => {
      if (!value) return "Email je obavezan.";
      return /^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(value) ? null : "Unesite ispravnu email adresu.";
    },
    phone: (value) => {
      if (!value) return "Telefon je obavezan.";
      return /^[0-9+\s\-()]{6,20}$/.test(value) ? null : "Unesite ispravan broj telefona.";
    },
    password: (value) => {
      if (!value) return "Lozinka je obavezna.";
      if (value.length < 8) return "Lozinka mora imati najmanje 8 karaktera.";
      if (!/[A-Z]/.test(value)) return "Lozinka mora sadržati bar jedno veliko slovo.";
      if (!/[0-9]/.test(value)) return "Lozinka mora sadržati bar jedan broj.";
      return null;
    },
    confirmPassword: (value, allValues) => {
      if (!value) return "Potvrda lozinke je obavezna.";
      return value === allValues.password ? null : "Lozinke se ne poklapaju.";
    },
    pib: (value) => {
      if (!value) return "PIB je obavezan.";
      return /^\d{9}$/.test(value.trim()) ? null : "PIB mora imati tačno 9 cifara.";
    },
    maticni: (value) => {
      if (!value) return "Matični broj je obavezan.";
      return /^\d{8}$/.test(value.trim()) ? null : "Matični broj mora imati tačno 8 cifara.";
    },
    url: (value) => {
      if (!value) return null; // optional
      try {
        new URL(value.startsWith("http") ? value : `https://${value}`);
        return null;
      } catch {
        return "Unesite ispravan URL (npr. www.kompanija.rs).";
      }
    },
    select: (value) => (!value || value === "" ? "Molimo odaberite opciju." : null),
  };
  
  // ============================================================
  // FIELD SCHEMAS PER FORM
  // ============================================================
  
  export const schemas = {
    candidateLogin: {
      email: [validators.required, validators.email],
      password: [validators.required],
    },
    companyLogin: {
      pibOrMaticni: [validators.required],
      email: [validators.required, validators.email],
      password: [validators.required],
    },
    candidateRegister: {
      firstName: [validators.required],
      lastName: [validators.required],
      email: [validators.required, validators.email],
      phone: [validators.required, validators.phone],
      education: [validators.required, validators.select],
      workArea: [validators.required, validators.select],
      experience: [validators.required, validators.select],
      password: [validators.required, validators.password],
      confirmPassword: [validators.required, validators.confirmPassword],
    },
    companyRegister: {
      companyName: [validators.required],
      pib: [validators.required, validators.pib],
      maticniBroj: [validators.required, validators.maticni],
      email: [validators.required, validators.email],
      phone: [validators.required, validators.phone],
      industry: [validators.required, validators.select],
      employeeCount: [validators.required, validators.select],
      city: [validators.required],
      website: [validators.url],
      contactName: [validators.required],
      contactPosition: [validators.required],
      password: [validators.required, validators.password],
      confirmPassword: [validators.required, validators.confirmPassword],
    },
  };
  
  // ============================================================
  // VALIDATE SINGLE FIELD
  // ============================================================
  
  export function validateField(fieldName, value, allValues, schema) {
    const fieldValidators = schema[fieldName];
    if (!fieldValidators) return null;
    for (const validator of fieldValidators) {
      const error = validator(value, allValues);
      if (error) return error;
    }
    return null;
  }
  
  // ============================================================
  // VALIDATE ENTIRE FORM
  // ============================================================
  
  export function validateForm(values, schema) {
    const errors = {};
    for (const fieldName of Object.keys(schema)) {
      const error = validateField(fieldName, values[fieldName], values, schema);
      if (error) errors[fieldName] = error;
    }
    return errors;
  }