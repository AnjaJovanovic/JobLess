import { GENDER } from "../../../api/clientApi";

export function emptyProfileForm() {
  return {
    firstName: "",
    lastName: "",
    phoneNumber: "",
    gender: null,
  };
}

export function profileToForm(profile) {
  return {
    firstName: profile.firstName ?? "",
    lastName: profile.lastName ?? "",
    phoneNumber: profile.phoneNumber ?? "",
    gender: profile.gender ?? null,
  };
}

export function formToPayload(form, email) {
  return {
    email,
    firstName: form.firstName.trim(),
    lastName: form.lastName.trim(),
    phoneNumber: form.phoneNumber.trim() || null,
    gender: Number(form.gender),
    isActive: true,
    clientId: 0,
    createdAt: new Date().toISOString(),
  };
}

export function validateProfileForm(form) {
  const errors = {};

  if (!form.firstName.trim()) errors.firstName = "Ime je obavezno.";
  if (!form.lastName.trim()) errors.lastName = "Prezime je obavezno.";
  if (form.gender === null || form.gender === undefined || form.gender === "") {
    errors.gender = "Pol je obavezan.";
  } else if (![GENDER.MALE, GENDER.FEMALE].includes(Number(form.gender))) {
    errors.gender = "Izaberite pol.";
  }

  return errors;
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
