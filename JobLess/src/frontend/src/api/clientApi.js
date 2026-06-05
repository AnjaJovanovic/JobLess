const JSON_HEADERS = { "Content-Type": "application/json" };

async function handleResponse(response) {
  if (!response.ok) {
    const text = await response.text();
    throw new Error(text || `HTTP ${response.status}`);
  }

  if (response.status === 204) return null;
  return response.json();
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

export function getStoredEmail() {
  try {
    const user = JSON.parse(localStorage.getItem("user") || "null");
    return user?.email ?? "";
  } catch {
    return "";
  }
}

export const GENDER = {
  MALE: 0,
  FEMALE: 1,
};

export function genderLabel(gender) {
  if (gender === GENDER.FEMALE) return "Ženski";
  if (gender === GENDER.MALE) return "Muški";
  return "Nepoznato";
}
