// src/services/api.ts
export interface HealthResponse {
  status: string;
  time: string;
}

const API_BASE_URL = import.meta.env.VITE_API_BASE_URL;

export const getHealth = async (): Promise<HealthResponse> => {
  const res = await fetch(`${API_BASE_URL}/api/health`);
  if (!res.ok) throw new Error("Failed to fetch health status");
  return res.json();
};
