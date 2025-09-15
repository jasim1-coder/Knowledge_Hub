// src/pages/UploadPage.tsx
import { useState } from "react";
import { uploadDocument, type DocumentUploadResponse } from "../services/api";

export const UploadPage = () => {
  const [file, setFile] = useState<File | null>(null);
  const [userId, setUserId] = useState<string>("");
  const [uploading, setUploading] = useState<boolean>(false);
  const [response, setResponse] = useState<DocumentUploadResponse | null>(null);

  const handleFileChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    if (e.target.files && e.target.files[0]) {
      setFile(e.target.files[0]);
    }
  };

  const handleUpload = async () => {
    if (!file) return alert("Please select a file");
    if (!userId) return alert("Please provide a userId");

    try {
      setUploading(true);
      const res = await uploadDocument(file, userId);
      setResponse(res);
      alert("File uploaded successfully!");
    } catch (err) {
      console.error(err);
      alert("Upload failed");
    } finally {
      setUploading(false);
    }
  };

  return (
    <div style={{ padding: "2rem" }}>
      <h2>Upload Document</h2>

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
      </div>

      <div style={{ marginBottom: "1rem" }}>
        <input type="file" onChange={handleFileChange} />
      </div>

      <button onClick={handleUpload} disabled={uploading}>
        {uploading ? "Uploading..." : "Upload"}
      </button>

      {response && (
        <div style={{ marginTop: "1rem" }}>
          <h4>Upload Response:</h4>
          <pre>{JSON.stringify(response, null, 2)}</pre>
        </div>
      )}
    </div>
  );
};
