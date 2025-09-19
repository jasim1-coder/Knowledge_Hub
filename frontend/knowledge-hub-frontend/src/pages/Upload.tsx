import { useState } from "react";
import axios from "axios";

export const UploadPage = () => {


  type UploadResponse = {
    id: string;
    fileName: string;
    fileSize: number;
    status: number;
    userId: string;
    sectionCount: number;
  };



  const [file, setFile] = useState<File | null>(null);
  const [description, setDescription] = useState("");
  const [uploading, setUploading] = useState(false);

  const handleUpload = async () => {
    if (!file) return alert("Please select a file first!");

    // Get user from localStorage
    const storedUser = localStorage.getItem("user");
    if (!storedUser) return alert("User not logged in!");

    const user = JSON.parse(storedUser);
    const userId = user.userId;

    const formData = new FormData();
    formData.append("file", file);
    formData.append("userId", userId);
    formData.append("description", description);




    try {
      setUploading(true);
      const response = await axios.post<UploadResponse>(
        "http://localhost:5117/api/Document/upload",
        formData,
        {
          headers: { "Content-Type": "multipart/form-data" },
        }
      );
      alert(`✅ Uploaded successfully: ${response.data.fileName}`);
      console.log(response.data);
    } catch (error: any) {
      console.error(error);
      alert("❌ Upload failed");
    } finally {
      setUploading(false);
    }
  };

  return (
    <div className="flex h-screen">
      <main className="flex-1 p-6 bg-gray-100">
        <h1 className="text-2xl font-bold mb-4">Upload Documents</h1>

        <div className="bg-white rounded shadow p-6 space-y-4">
          {/* File Input */}
          <input
            type="file"
            className="block"
            onChange={(e) => setFile(e.target.files?.[0] || null)}
          />

          {/* Description */}
          <input
            type="text"
            placeholder="Enter description"
            value={description}
            onChange={(e) => setDescription(e.target.value)}
            className="w-full p-2 border rounded"
          />

          {/* Upload Button */}
          <button
            onClick={handleUpload}
            disabled={uploading}
            className={`px-6 py-2 rounded text-white ${uploading ? "bg-gray-400" : "bg-green-600 hover:bg-green-700"
              }`}
          >
            {uploading ? "Uploading..." : "Upload"}
          </button>
        </div>
      </main>
    </div>
  );
};
