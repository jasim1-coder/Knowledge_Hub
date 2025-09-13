// src/pages/HomePage.tsx
import React, { useEffect, useState } from "react";
import { getHealth } from "../services/api";
import type { HealthResponse } from "../services/api";

export const HomePage = () => {
  const [status, setStatus] = useState<string>("Loading...");

  useEffect(() => {
    getHealth()
      .then((data: HealthResponse) => setStatus(data.status))
      .catch(() => setStatus("Error connecting to backend"));
      console.log(import.meta.env.VITE_API_BASE_URL);

  }, []);

  return (
    <div style={{ padding: "2rem" }}>
      <h2>Welcome to Knowledge Hub</h2>
      <p>Backend Status: <strong>{status}</strong></p>
      <p>This is the Phase 1 skeleton. Chat and Upload pages are placeholders for now.</p>
    </div>
  );
};
