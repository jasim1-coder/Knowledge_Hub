import { useEffect, useState } from "react";
import axios from "axios";

// Backend Document type
interface Document {
  id: string;
  fileName: string;
  filePath: string;
  userId: string;
  sections: { id: string; content: string }[];
}

// LocalStorage User type
interface User {
  userName: string;
  email: string;
  roles: string[];
  userId: string;
}

// Embeddings Status type
interface EmbeddingsStatus {
  documentId: string;
  hasEmbeddings: boolean;
  checkedAt: string;
}

export const DocumentsPage: React.FC = () => {
  const [search, setSearch] = useState<string>("");
  const [documents, setDocuments] = useState<Document[]>([]);
  const [loading, setLoading] = useState<boolean>(true);
  const [embeddingsStatus, setEmbeddingsStatus] = useState<
    Record<string, EmbeddingsStatus | null>
  >({});
  const [processing, setProcessing] = useState<string | null>(null);

  useEffect(() => {
    const fetchDocuments = async () => {
      try {
        const storedUser = localStorage.getItem("user");
        if (!storedUser) {
          console.error("No user found in localStorage");
          setLoading(false);
          return;
        }

        const user: User = JSON.parse(storedUser);
        const response = await axios.get<Document[]>(
          `http://localhost:5117/api/Document/user/${user.userId}`
        );
        setDocuments(response.data);

        // Fetch embeddings status for each document
        response.data.forEach((doc) => fetchEmbeddingsStatus(doc.id));
      } catch (error) {
        console.error("Error fetching documents:", error);
      } finally {
        setLoading(false);
      }
    };

    fetchDocuments();
  }, []);

  const fetchEmbeddingsStatus = async (documentId: string) => {
    try {
      const response = await axios.get<EmbeddingsStatus>(
        `http://localhost:5117/api/Rag/document/${documentId}/embeddings-status`
      );
      setEmbeddingsStatus((prev) => ({
        ...prev,
        [documentId]: response.data,
      }));
    } catch (error) {
      console.error("Error fetching embeddings status:", error);
      setEmbeddingsStatus((prev) => ({
        ...prev,
        [documentId]: null,
      }));
    }
  };

  const generateEmbeddings = async (documentId: string) => {
    try {
      setProcessing(documentId);
      await axios.post(
        `http://localhost:5117/api/Rag/generate-embeddings/document/${documentId}`
      );
      await fetchEmbeddingsStatus(documentId); // refresh status
      alert("✅ Embeddings generated successfully!");
    } catch (error) {
      console.error("Error generating embeddings:", error);
      alert("❌ Failed to generate embeddings.");
    } finally {
      setProcessing(null);
    }
  };

  const filteredDocs = documents.filter((doc) =>
    doc.fileName.toLowerCase().includes(search.toLowerCase())
  );

  return (
    <div className="flex h-screen">
      <main className="flex-1 p-6 bg-gray-100">
        <h1 className="text-2xl font-bold mb-2">Document Management</h1>
        <p className="mb-4 text-gray-600">
          Organize and manage your knowledge base documents
        </p>

        <div className="mb-4 flex justify-between items-center">
          <button className="bg-blue-600 text-white px-4 py-2 rounded hover:bg-blue-700">
            Upload New Document
          </button>
          <input
            type="text"
            placeholder="Search documents..."
            value={search}
            onChange={(e) => setSearch(e.target.value)}
            className="border rounded px-3 py-2 w-64"
          />
        </div>

        {loading ? (
          <p className="text-gray-500">Loading documents...</p>
        ) : (
          <div className="bg-white shadow rounded overflow-x-auto">
            <table className="min-w-full divide-y divide-gray-200">
              <thead className="bg-gray-50">
                <tr>
                  <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">
                    Name
                  </th>
                  <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">
                    Path
                  </th>
                  <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">
                    Sections
                  </th>
                  <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">
                    Embeddings
                  </th>
                  <th className="px-6 py-3 text-center text-xs font-medium text-gray-500 uppercase">
                    Actions
                  </th>
                </tr>
              </thead>
              <tbody className="bg-white divide-y divide-gray-200">
                {filteredDocs.map((doc) => {
                  const status = embeddingsStatus[doc.id];
                  return (
                    <tr key={doc.id}>
                      <td className="px-6 py-4 whitespace-nowrap">
                        {doc.fileName}
                      </td>
                      <td className="px-6 py-4 whitespace-nowrap truncate max-w-xs">
                        {doc.filePath}
                      </td>
                      <td className="px-6 py-4 whitespace-nowrap">
                        {doc.sections.length} sections
                      </td>
                      <td className="px-6 py-4 whitespace-nowrap">
                        {status ? (
                          status.hasEmbeddings ? (
                            <span className="px-2 inline-flex text-xs leading-5 font-semibold rounded-full bg-green-100 text-green-800">
                              ✅ Ready
                            </span>
                          ) : (
                            <span className="px-2 inline-flex text-xs leading-5 font-semibold rounded-full bg-yellow-100 text-yellow-800">
                              ⏳ Not Generated
                            </span>
                          )
                        ) : (
                          <span className="text-gray-500">Unknown</span>
                        )}
                      </td>
                      <td className="px-6 py-4 whitespace-nowrap text-center">
                        <button
                          onClick={() => generateEmbeddings(doc.id)}
                          disabled={processing === doc.id}
                          className={`px-4 py-2 rounded ${
                            processing === doc.id
                              ? "bg-gray-400 text-white cursor-not-allowed"
                              : "bg-indigo-600 text-white hover:bg-indigo-700"
                          }`}
                        >
                          {processing === doc.id
                            ? "Processing..."
                            : "Generate Embeddings"}
                        </button>
                      </td>
                    </tr>
                  );
                })}
                {filteredDocs.length === 0 && (
                  <tr>
                    <td
                      colSpan={5}
                      className="text-center py-4 text-gray-500"
                    >
                      No documents found
                    </td>
                  </tr>
                )}
              </tbody>
            </table>
          </div>
        )}
      </main>
    </div>
  );
};
