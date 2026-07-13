/**
 * API klijent za JobLess frontend.
 * Profil kandidata, oglasi, prijave i notifikacije preko Api Gateway-a.
 * @module api/clientApi
 */

const JSON_HEADERS = { "Content-Type": "application/json" };

/**
 * Headeri sa Bearer tokenom.
 * @param {string} [token]
 * @returns {Record<string, string>}
 */
function authHeaders(token) {
  return token
    ? { ...JSON_HEADERS, Authorization: `Bearer ${token}` }
    : { ...JSON_HEADERS };
}

async function handleResponse(response) {
  if (!response.ok) {
    const text = await response.text();
    throw new Error(text || `HTTP ${response.status}`);
  }

  if (response.status === 204) return null;
  return response.json();
}

/**
 * Učitava profil kandidata po emailu.
 * @param {string} email
 * @param {string} token
 * @returns {Promise<object|null>}
 */
export async function getClientProfileByEmail(email, token) {
  const encoded = encodeURIComponent(email);
  const response = await fetch(`/api/clients/profile/by-email?email=${encoded}`, {
    headers: {
      Authorization: `Bearer ${token}`,
    },
  });

  if (response.status === 404) return null;

  return handleResponse(response);
}

export async function createClientProfile(payload, token) {
  const response = await fetch("/api/clients/profile", {
    method: "PUT",
    headers: authHeaders(token),
    body: JSON.stringify(payload),
  });

  return handleResponse(response);
}

export async function getClientProfile(clientId, token) {
  const response = await fetch(`/api/clients/${clientId}/profile`, {
    headers: {
      Authorization: `Bearer ${token}`,
    },
  });

  if (response.status === 404) return null;

  return handleResponse(response);
}

export async function updateClientProfile(clientId, payload, token) {
  const response = await fetch(`/api/clients/${clientId}/profile`, {
    method: "PUT",
    headers: authHeaders(token),
    body: JSON.stringify(payload),
  });

  return handleResponse(response);
}

export const CLIENT_ID_KEY = "clientProfileId";

export function getStoredClientId() {
  return localStorage.getItem(CLIENT_ID_KEY);
}

export function storeClientId(clientId) {
  localStorage.setItem(CLIENT_ID_KEY, String(clientId));
}

export function clearStoredClientId() {
  localStorage.removeItem(CLIENT_ID_KEY);
}

export async function syncClientProfileAfterAuth(email, token) {
  if (!email || !token) {
    clearStoredClientId();
    return null;
  }

  const profile = await getClientProfileByEmail(email, token);

  if (profile) {
    storeClientId(profile.clientId);
    return profile;
  }

  clearStoredClientId();
  return null;
}

export const GENDER = {
  MALE: 0,
  FEMALE: 1,
};

export const EDUCATION_LEVEL = {
  PRIMARY_SCHOOL: 1,
  SECONDARY_SCHOOL: 2,
  HIGHER_PROFESSIONAL: 3,
  BACHELOR: 4,
  MASTER: 5,
  DOCTORATE: 6,
};

export const EDUCATION_LEVEL_OPTIONS = [
  { value: EDUCATION_LEVEL.PRIMARY_SCHOOL, label: "Osnovna škola" },
  { value: EDUCATION_LEVEL.SECONDARY_SCHOOL, label: "Srednja škola" },
  { value: EDUCATION_LEVEL.HIGHER_PROFESSIONAL, label: "Viša stručna sprema" },
  { value: EDUCATION_LEVEL.BACHELOR, label: "Fakultet (osnovne akademske studije)" },
  { value: EDUCATION_LEVEL.MASTER, label: "Master studije" },
  { value: EDUCATION_LEVEL.DOCTORATE, label: "Doktorske studije" },
];

export function genderLabel(gender) {
  if (gender === "" || gender === null || gender === undefined) return "Nepoznato";
  const value = Number(gender);
  if (value === GENDER.FEMALE) return "Ženski";
  if (value === GENDER.MALE) return "Muški";
  return "Nepoznato";
}

export function educationLevelLabel(level) {
  if (level === "" || level === null || level === undefined) return "—";
  const option = EDUCATION_LEVEL_OPTIONS.find((item) => item.value === Number(level));
  return option?.label ?? "—";
}

export const APPLICATION_STATUS = {
  PENDING: 0,
  ACCEPTED: 1,
  REJECTED: 2,
};

export function applicationStatusLabel(status) {
  const value = Number(status);
  if (value === APPLICATION_STATUS.ACCEPTED) return "Prihvaćen";
  if (value === APPLICATION_STATUS.REJECTED) return "Odbijen";
  return "U razmatranju";
}

function parseApiError(text, fallback) {
  try {
    const parsed = JSON.parse(text);
    if (parsed.message) return parsed.message;
    if (parsed.title) return parsed.title;
    if (Array.isArray(parsed.errors)) return parsed.errors.join(" ");
    const firstValidation = Object.values(parsed.errors || {})[0];
    if (Array.isArray(firstValidation) && firstValidation.length > 0) {
      return firstValidation[0];
    }
    return fallback;
  } catch {
    return text || fallback;
  }
}

export async function getCompanyById(companyId) {
  const response = await fetch(`/api/Companies/One?id=${companyId}`);
  if (!response.ok) return null;
  const data = await response.json();
  return data?.company ?? data ?? null;
}

export async function getAdvertisementById(advertisementId) {
  const response = await fetch(`/api/Advertisements/One?id=${advertisementId}`);
  if (!response.ok) return null;
  const data = await response.json();
  return data?.advertisement ?? data ?? null;
}

export async function refreshAccessToken(email, refreshToken) {
  const response = await fetch("/api/Auth/refresh", {
    method: "POST",
    headers: JSON_HEADERS,
    body: JSON.stringify({ email, refreshToken }),
  });
  if (!response.ok) return null;
  return response.json();
}

/**
 * Lista notifikacija ulogovanog korisnika.
 * @param {string} token
 * @returns {Promise<Array>}
 */
export async function getMyNotifications(token) {
  const response = await fetch("/api/notifications/me", {
    headers: {
      Authorization: `Bearer ${token}`,
    },
  });

  if (!response.ok) {
    const text = await response.text();
    throw new Error(parseApiError(text, "Greška pri učitavanju obaveštenja."));
  }

  const data = await response.json();
  return Array.isArray(data) ? data : [];
}

export async function markNotificationAsRead(notificationId, token) {
  const response = await fetch(`/api/notifications/${notificationId}/read`, {
    method: "PUT",
    headers: {
      Authorization: `Bearer ${token}`,
    },
  });

  if (response.status === 204 || response.ok) return true;

  const text = await response.text();
  throw new Error(parseApiError(text, "Greška pri označavanju obaveštenja."));
}

/**
 * Kandidat se prijavljuje na oglas.
 * @param {{ advertisementId: number, companyId: number }} payload
 * @param {string} token
 * @returns {Promise<object>}
 */
export async function applyForJob(payload, token) {
  const response = await fetch("/api/job-applications", {
    method: "POST",
    headers: {
      ...JSON_HEADERS,
      Authorization: `Bearer ${token}`,
    },
    body: JSON.stringify(payload),
  });

  if (!response.ok) {
    const text = await response.text();
    throw new Error(parseApiError(text, "Greška pri prijavi na oglas."));
  }

  return handleResponse(response);
}

/**
 * Prijave ulogovanog kandidata.
 * @param {string} token
 * @returns {Promise<Array>}
 */
export async function getMyJobApplications(token) {
  const response = await fetch("/api/job-applications/my", {
    headers: {
      Authorization: `Bearer ${token}`,
    },
  });

  if (!response.ok) {
    const text = await response.text();
    throw new Error(parseApiError(text, "Greška pri učitavanju prijava."));
  }

  const data = await response.json();
  return Array.isArray(data) ? data : [];
}

/**
 * Prijave za oglase kompanije (opciono filter po oglasu/statusu).
 * @param {string} token
 * @param {Object} [filters]
 * @param {number} [filters.advertisementId]
 * @param {number} [filters.status]
 * @returns {Promise<Array>}
 */
export async function getCompanyJobApplications(token, { advertisementId, status } = {}) {
  const params = new URLSearchParams();
  if (advertisementId) params.set("advertisementId", String(advertisementId));
  if (status !== undefined && status !== null && status !== "") {
    params.set("status", String(status));
  }

  const query = params.toString();
  const url = query
    ? `/api/job-applications/company?${query}`
    : "/api/job-applications/company";

  const response = await fetch(url, {
    headers: {
      Authorization: `Bearer ${token}`,
    },
  });

  if (!response.ok) {
    const text = await response.text();
    throw new Error(parseApiError(text, "Greška pri učitavanju prijava kandidata."));
  }

  const data = await response.json();
  return Array.isArray(data) ? data : [];
}

/**
 * Kompanija menja status prijave (accept/reject).
 * @param {number} applicationId
 * @param {number} status 1=Accepted, 2=Rejected
 * @param {string} token
 * @returns {Promise<object>}
 */
export async function updateJobApplicationStatus(applicationId, status, token) {
  const response = await fetch(`/api/job-applications/${applicationId}/status`, {
    method: "PATCH",
    headers: {
      ...JSON_HEADERS,
      Authorization: `Bearer ${token}`,
    },
    body: JSON.stringify({ status }),
  });

  if (!response.ok) {
    const text = await response.text();
    throw new Error(parseApiError(text, "Greška pri promeni statusa prijave."));
  }

  return handleResponse(response);
}
