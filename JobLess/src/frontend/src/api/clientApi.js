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

export function genderLabel(gender) {
  if (gender === "" || gender === null || gender === undefined) return "Nepoznato";
  const value = Number(gender);
  if (value === GENDER.FEMALE) return "Ženski";
  if (value === GENDER.MALE) return "Muški";
  return "Nepoznato";
}
