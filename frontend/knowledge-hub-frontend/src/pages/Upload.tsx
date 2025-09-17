import  { useState } from "react";

export const UploadPage = () => {
  const [file, setFile] = useState<File | null>(null);

  const handleUpload = () => {
    if (!file) return;
    alert(`Uploaded: ${file.name}`);
  };

  return (
    <div className="flex h-screen">
      <main className="flex-1 p-6 bg-gray-100">
        <h1 className="text-2xl font-bold mb-4">Upload Documents</h1>

        <div className="bg-white rounded shadow p-6">
          <input
            type="file"
            className="mb-4 block"
            onChange={(e) => setFile(e.target.files?.[0] || null)}
          />
          <button
            onClick={handleUpload}
            className="bg-green-600 text-white px-6 py-2 rounded hover:bg-green-700"
          >
            Upload
          </button>
        </div>
      </main>
    </div>
  );
};
