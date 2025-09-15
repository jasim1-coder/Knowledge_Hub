// src/pages/DocumentsPage.tsx
import { useEffect, useState } from "react";
import { getUserDocuments, type Document } from "../services/api";

export const DocumentsPage = () => {
  const [userId, setUserId] = useState<string>("");
  const [documents, setDocuments] = useState<Document[]>([]);
  const [loading, setLoading] = useState<boolean>(false);
  const [error, setError] = useState<string | null>(null);

  const fetchDocuments = async () => {
    if (!userId) return;
    setLoading(true);
    setError(null);
    try {
      const docs = await getUserDocuments(userId);
      setDocuments(docs);
    } catch (err) {
      console.error(err);
      setError("Failed to load documents");
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    if (userId) fetchDocuments();
  }, [userId]);

  return (
    <div style={{ padding: "2rem" }}>
      <h2>Your Documents</h2>

      <div style={{ marginBottom: "1rem" }}>
        <label>
          User ID:{" "}
          <input
            type="text"
            value={userId}
            onChange={(e) => setUserId(e.target.value)}
            placeholder="Enter your UserId"
          />
        </label>
        <button onClick={fetchDocuments} disabled={!userId || loading} style={{ marginLeft: "1rem" }}>
          {loading ? "Loading..." : "Fetch Documents"}
        </button>
      </div>

      {error && <p style={{ color: "red" }}>{error}</p>}

      {documents.length === 0 && !loading && <p>No documents found.</p>}

      {documents.map((doc) => (
        <div key={doc.id} style={{ border: "1px solid #ccc", padding: "1rem", marginBottom: "1rem" }}>
          <h4>{doc.fileName}</h4>
          <p>File Path: {doc.filePath}</p>
          <h5>Sections:</h5>
          <ul>
            {doc.sections.map((s) => (
              <li key={s.id}>{s.content.substring(0, 100)}{s.content.length > 100 ? "..." : ""}</li>
            ))}
          </ul>
        </div>
      ))}
    </div>
  );
};
