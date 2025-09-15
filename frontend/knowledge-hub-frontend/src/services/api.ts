// src/services/api.ts
export interface HealthResponse {
  status: string;
  time: string;
}

export interface DocumentUploadResponse {
  id: string;
  fileName: string;
  filePath: string;
  userId: string;
  sections: { id: string; content: string }[];
}

export interface DocumentSection {
  id: string;
  content: string;
}

export interface Document {
  id: string;
  fileName: string;
  filePath: string;
  userId: string;
  sections: DocumentSection[];
}



const API_BASE_URL = import.meta.env.VITE_API_BASE_URL;

export const getHealth = async (): Promise<HealthResponse> => {
  const res = await fetch(`${API_BASE_URL}/api/health`);
  if (!res.ok) throw new Error("Failed to fetch health status");
  return res.json();
};


export const uploadDocument = async (
  file: File,
  userId: string
): Promise<DocumentUploadResponse> => {
  const formData = new FormData();
  formData.append("file", file);
  formData.append("userId", userId);

  const res = await fetch(`${API_BASE_URL}/api/document/upload`, {
    method: "POST",
    body: formData,
  });

  if (!res.ok) throw new Error("Failed to upload document");
  return res.json();
};


export const getUserDocuments = async (userId: string): Promise<Document[]> => {
  const res = await fetch(`${API_BASE_URL}/api/document/user/${userId}`);
  if (!res.ok) throw new Error("Failed to fetch documents");
  return res.json();
};