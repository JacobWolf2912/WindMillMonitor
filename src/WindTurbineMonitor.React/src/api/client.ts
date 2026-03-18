// Determine API base URL based on environment
const API_BASE = (() => {
  const hostname = window.location.hostname;
  const protocol = window.location.protocol;

  // If running on localhost, use local API
  if (hostname === 'localhost' || hostname === '127.0.0.1') {
    return 'http://localhost:5021';
  }

  // On Azure, construct the API URL
  if (hostname.includes('azurewebsites.net')) {
    return `${protocol}//windturbine-api.azurewebsites.net`;
  }

  // Default fallback
  return `${protocol}//${hostname}:5021`;
})();

function getHeaders(): Record<string, string> {
  const headers: Record<string, string> = { "Content-Type": "application/json" };
  const token = localStorage.getItem("jwt_token");
  if (token) {
    headers["Authorization"] = `Bearer ${token}`;
  }
  return headers;
}

export async function get<T>(path: string): Promise<T> {
  const res = await fetch(`${API_BASE}${path}`, {
    headers: getHeaders(),
  });
  if (!res.ok) throw new Error(`GET ${path} failed: ${res.status}`);
  return res.json();
}

export async function patch(path: string): Promise<void> {
  const res = await fetch(`${API_BASE}${path}`, {
    method: "PATCH",
    headers: getHeaders(),
  });
  if (!res.ok) throw new Error(`PATCH ${path} failed: ${res.status}`);
}

export async function post<T>(path: string, body: unknown): Promise<T> {
  const res = await fetch(`${API_BASE}${path}`, {
    method: "POST",
    headers: getHeaders(),
    body: JSON.stringify(body),
  });
  if (!res.ok) throw new Error(`POST ${path} failed: ${res.status}`);
  return res.json();
}
