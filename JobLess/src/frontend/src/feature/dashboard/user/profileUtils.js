import { GENDER, EDUCATION_LEVEL } from "../../../api/clientApi";

export function emptyProfileForm() {
  return {
    firstName: "",
    lastName: "",
    phoneNumber: "",
    gender: null,
    dateOfBirth: "",
    city: "",
    address: "",
    educationLevel: "",
    institutionName: "",
    educationStartYear: "",
    educationEndYear: "",
    yearsOfExperience: "",
    skills: "",
    professionalSummary: "",
    linkedInUrl: "",
  };
}

function formatDateForInput(value) {
  if (!value) return "";
  const date = new Date(value);
  if (Number.isNaN(date.getTime())) return "";
  return date.toISOString().slice(0, 10);
}

export function profileToForm(profile) {
  return {
    firstName: profile.firstName ?? "",
    lastName: profile.lastName ?? "",
    phoneNumber: profile.phoneNumber ?? "",
    gender: profile.gender ?? null,
    dateOfBirth: formatDateForInput(profile.dateOfBirth),
    city: profile.city ?? "",
    address: profile.address ?? "",
    educationLevel: profile.educationLevel ?? "",
    institutionName: profile.institutionName ?? "",
    educationStartYear: profile.educationStartYear ?? "",
    educationEndYear: profile.educationEndYear ?? "",
    yearsOfExperience: profile.yearsOfExperience ?? "",
    skills: profile.skills ?? "",
    professionalSummary: profile.professionalSummary ?? "",
    linkedInUrl: profile.linkedInUrl ?? "",
  };
}

function parseOptionalInt(value) {
  if (value === "" || value === null || value === undefined) return null;
  const parsed = Number(value);
  return Number.isNaN(parsed) ? null : parsed;
}

export function formToPayload(form, email, clientId = 0) {
  const educationLevel = parseOptionalInt(form.educationLevel);
  const yearsOfExperience = parseOptionalInt(form.yearsOfExperience);
  const educationStartYear = parseOptionalInt(form.educationStartYear);
  const educationEndYear = parseOptionalInt(form.educationEndYear);

  return {
    clientId,
    email,
    firstName: form.firstName.trim(),
    lastName: form.lastName.trim(),
    phoneNumber: form.phoneNumber.trim() || null,
    gender: Number(form.gender),
    dateOfBirth: form.dateOfBirth ? new Date(form.dateOfBirth).toISOString() : null,
    city: form.city.trim() || null,
    address: form.address.trim() || null,
    educationLevel: educationLevel === null ? null : educationLevel,
    institutionName: form.institutionName.trim() || null,
    educationStartYear,
    educationEndYear,
    yearsOfExperience,
    skills: form.skills.trim() || null,
    professionalSummary: form.professionalSummary.trim() || null,
    linkedInUrl: form.linkedInUrl.trim() || null,
    isActive: true,
    createdAt: new Date().toISOString(),
  };
}

const CURRENT_YEAR = new Date().getFullYear();

export function validateProfileForm(form) {
  const errors = {};

  if (!form.firstName.trim()) errors.firstName = "Ime je obavezno.";
  if (!form.lastName.trim()) errors.lastName = "Prezime je obavezno.";

  if (form.gender === null || form.gender === undefined || form.gender === "") {
    errors.gender = "Pol je obavezan.";
  } else if (![GENDER.MALE, GENDER.FEMALE].includes(Number(form.gender))) {
    errors.gender = "Izaberite pol.";
  }

  if (form.dateOfBirth) {
    const dob = new Date(form.dateOfBirth);
    if (Number.isNaN(dob.getTime()) || dob >= new Date()) {
      errors.dateOfBirth = "Unesite validan datum rođenja u prošlosti.";
    }
  }

  if (form.educationLevel === "" || form.educationLevel === null || form.educationLevel === undefined) {
    errors.educationLevel = "Stepen obrazovanja je obavezan.";
  } else if (!Object.values(EDUCATION_LEVEL).includes(Number(form.educationLevel))) {
    errors.educationLevel = "Izaberite stepen obrazovanja.";
  }

  if (!form.institutionName.trim()) {
    errors.institutionName = "Naziv institucije je obavezan.";
  }

  if (form.educationStartYear !== "" && form.educationStartYear !== null) {
    const start = Number(form.educationStartYear);
    if (Number.isNaN(start) || start < 1950 || start > CURRENT_YEAR + 1) {
      errors.educationStartYear = `Unesite godinu između 1950 i ${CURRENT_YEAR + 1}.`;
    }
  }

  if (form.educationEndYear !== "" && form.educationEndYear !== null) {
    const end = Number(form.educationEndYear);
    if (Number.isNaN(end) || end < 1950 || end > CURRENT_YEAR + 1) {
      errors.educationEndYear = `Unesite godinu između 1950 i ${CURRENT_YEAR + 1}.`;
    }
  }

  const startYear = Number(form.educationStartYear);
  const endYear = Number(form.educationEndYear);
  if (
    form.educationStartYear !== "" &&
    form.educationStartYear !== null &&
    form.educationEndYear !== "" &&
    form.educationEndYear !== null &&
    !errors.educationStartYear &&
    !errors.educationEndYear &&
    !Number.isNaN(startYear) &&
    !Number.isNaN(endYear) &&
    startYear > endYear
  ) {
    errors.educationEndYear = "Godina završetka ne može biti pre godine početka.";
  }

  if (form.yearsOfExperience !== "" && form.yearsOfExperience !== null) {
    const years = Number(form.yearsOfExperience);
    if (Number.isNaN(years) || years < 0 || years > 60) {
      errors.yearsOfExperience = "Unesite broj između 0 i 60.";
    }
  }

  if (form.linkedInUrl.trim() && !/^https?:\/\/.+/i.test(form.linkedInUrl.trim())) {
    errors.linkedInUrl = "Unesite validan URL (npr. https://linkedin.com/in/...).";
  }

  if (form.phoneNumber.trim()) {
    if (!/^\+[0-9][0-9\s\-()/]{5,18}$/.test(form.phoneNumber.trim())) {
      errors.phoneNumber = "Broj telefona mora biti u formatu +381 60 123 4567.";
    }
  }

  return errors;
}

export function isProfileComplete(profile) {
  if (!profile) return false;

  return Boolean(
    profile.firstName?.trim() &&
    profile.lastName?.trim() &&
    (profile.gender === 0 || profile.gender === 1) &&
    profile.educationLevel &&
    profile.institutionName?.trim(),
  );
}

export function getStoredEmail() {
  try {
    const user = JSON.parse(localStorage.getItem("user") || "null");
    return user?.email ?? "";
  } catch {
    return "";
  }
}

export function isCandidateRole(role) {
  return role?.toLowerCase() === "candidate";
}

export function formatProfileDate(value) {
  if (!value) return "—";
  const date = new Date(value);
  if (Number.isNaN(date.getTime())) return "—";
  return date.toLocaleDateString("sr-RS", {
    day: "2-digit",
    month: "2-digit",
    year: "numeric",
  });
}
