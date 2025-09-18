import { useState } from "react";
import { FiTrash2, FiDownload } from "react-icons/fi";

// Document type
interface Document {
  name: string;
  date: string;
  status: "Indexed" | "Indexing" | "Error";
  size: string;
  type: string;
}

// Sample documents
const sampleDocs: Document[] = [
  { name: "Q4 Financial Report.pdf", date: "1/15/2024", status: "Indexed", size: "2.4 MB", type: "PDF" },
  { name: "Marketing Strategy 2024.docx", date: "1/14/2024", status: "Indexing", size: "1.8 MB", type: "DOCX" },
  { name: "Team Structure Overview.pdf", date: "1/13/2024", status: "Indexed", size: "856 KB", type: "PDF" },
  { name: "Product Requirements.txt", date: "1/12/2024", status: "Error", size: "45 KB", type: "TXT" },
  { name: "Company Policies.pdf", date: "1/10/2024", status: "Indexed", size: "3.2 MB", type: "PDF" },
];

interface Action {
  Icon: React.ElementType; // ✅ Correct type for react-icons
  onClick: (doc: Document) => void;
  color: string;
}


export const DocumentsPage: React.FC = () => {
  const [search, setSearch] = useState<string>("");

  const filteredDocs = sampleDocs.filter((doc) =>
    doc.name.toLowerCase().includes(search.toLowerCase())
  );

const actions: Action[] = [
  { Icon: FiDownload, onClick: (doc) => alert(`Download ${doc.name}`), color: "text-blue-500 hover:text-blue-700" },
  { Icon: FiTrash2, onClick: (doc) => alert(`Delete ${doc.name}`), color: "text-red-500 hover:text-red-700" },
];

  return (
    <div className="flex h-screen">
      <main className="flex-1 p-6 bg-gray-100">
        <h1 className="text-2xl font-bold mb-2">Document Management</h1>
        <p className="mb-4 text-gray-600">Organize and manage your knowledge base documents</p>

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

        <div className="bg-white shadow rounded overflow-x-auto">
          <table className="min-w-full divide-y divide-gray-200">
            <thead className="bg-gray-50">
              <tr>
                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Name</th>
                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Upload Date</th>
                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Status</th>
                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Size</th>
                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Type</th>
                <th className="px-6 py-3 text-center text-xs font-medium text-gray-500 uppercase tracking-wider">Actions</th>
              </tr>
            </thead>
            <tbody className="bg-white divide-y divide-gray-200">
              {filteredDocs.map((doc, idx) => (
                <tr key={idx}>
                  <td className="px-6 py-4 whitespace-nowrap">{doc.name}</td>
                  <td className="px-6 py-4 whitespace-nowrap">{doc.date}</td>
                  <td className="px-6 py-4 whitespace-nowrap">
                    <span
                      className={`px-2 inline-flex text-xs leading-5 font-semibold rounded-full ${
                        doc.status === "Indexed"
                          ? "bg-green-100 text-green-800"
                          : doc.status === "Indexing"
                          ? "bg-yellow-100 text-yellow-800"
                          : "bg-red-100 text-red-800"
                      }`}
                    >
                      {doc.status}
                    </span>
                  </td>
                  <td className="px-6 py-4 whitespace-nowrap">{doc.size}</td>
                  <td className="px-6 py-4 whitespace-nowrap">{doc.type}</td>
                  <td className="px-6 py-4 whitespace-nowrap text-center flex justify-center gap-2">

                  </td>
                </tr>
              ))}
              {filteredDocs.length === 0 && (
                <tr>
                  <td colSpan={6} className="text-center py-4 text-gray-500">
                    No documents found
                  </td>
                </tr>
              )}
            </tbody>
          </table>
        </div>
      </main>
    </div>
  );
};
