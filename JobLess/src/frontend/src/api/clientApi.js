const JSON_HEADERS = { "Content-Type": "application/json" };

async function handleResponse(response) {
  if (!response.ok) {
    const text = await response.text();
    throw new Error(text || `HTTP ${response.status}`);
  }

  if (response.status === 204) return null;
  return response.json();
}

export async function getClientProfileByEmail(email) {
  const encoded = encodeURIComponent(email);
  const response = await fetch(`/api/clients/profile/by-email?email=${encoded}`);

  if (response.status === 404) return null;

  return handleResponse(response);
}

export async function createClientProfile(payload) {
  const response = await fetch("/api/clients/profile", {
    method: "PUT",
    headers: JSON_HEADERS,
    body: JSON.stringify(payload),
  });

  return handleResponse(response);
}

export async function getClientProfile(clientId) {
  const response = await fetch(`/api/clients/${clientId}/profile`);

  if (response.status === 404) return null;

  return handleResponse(response);
}

export async function updateClientProfile(clientId, payload) {
  const response = await fetch(`/api/clients/${clientId}/profile`, {
    method: "PUT",
    headers: JSON_HEADERS,
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

export async function syncClientProfileAfterAuth(email) {
  if (!email) {
    clearStoredClientId();
    return null;
  }

  const profile = await getClientProfileByEmail(email);

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
    return parsed.message ?? fallback;
  } catch {
    return text || fallback;
  }
}

export async function applyToJob(clientId, advertisementId) {
  const numericClientId = Number(clientId);
  const numericAdId = Number(advertisementId);

  if (!numericClientId || !numericAdId) {
    throw new Error("Nedostaju podaci za prijavu. Popunite profil i pokušajte ponovo.");
  }

  const response = await fetch(`/api/clients/${numericClientId}/applications`, {
    method: "POST",
    headers: JSON_HEADERS,
    body: JSON.stringify({ advertisementId: numericAdId }),
  });

  if (!response.ok) {
    const text = await response.text();
    throw new Error(parseApiError(text, "Greška pri prijavi."));
  }

  return response.json();
}

export async function getClientApplications(clientId) {
  const response = await fetch(`/api/clients/${Number(clientId)}/applications`);

  if (!response.ok) {
    const text = await response.text();
    throw new Error(text || "Greška pri učitavanju prijava.");
  }

  const data = await response.json();
  return Array.isArray(data) ? data : [];
}

export async function getApplicationsByAdvertisements(advertisementIds, status = null) {
  if (!advertisementIds?.length) return [];

  const params = new URLSearchParams();
  advertisementIds.forEach((id) => params.append("advertisementIds", String(id)));
  if (status !== null && status !== undefined && status !== "") {
    params.set("status", String(status));
  }

  const response = await fetch(`/api/clients/applications/by-advertisements?${params.toString()}`);

  if (!response.ok) {
    const text = await response.text();
    throw new Error(parseApiError(text, "Greška pri učitavanju prijava."));
  }

  const data = await response.json();
  return Array.isArray(data) ? data : [];
}

export async function updateApplicationStatus(applicationId, status) {
  const response = await fetch(`/api/clients/applications/${applicationId}/status`, {
    method: "PUT",
    headers: JSON_HEADERS,
    body: JSON.stringify({ status: Number(status) }),
  });

  if (!response.ok) {
    const text = await response.text();
    throw new Error(parseApiError(text, "Greška pri ažuriranju statusa."));
  }

  return response.json();
}
