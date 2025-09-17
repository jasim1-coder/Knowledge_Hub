
const sampleDocs = [
  { name: "Q4 Report.pdf", date: "2 days ago" },
  { name: "Team Structure.docx", date: "5 days ago" },
  { name: "Marketing Strategy.pdf", date: "1 week ago" },
];

export const DocumentsPage = () => {
  return (
    <div className="flex h-screen">
      <main className="flex-1 p-6 bg-gray-100">
        <h1 className="text-2xl font-bold mb-4">My Documents</h1>

        <div className="bg-white shadow rounded p-4">
          {sampleDocs.map((doc, idx) => (
            <div
              key={idx}
              className="flex justify-between items-center border-b py-2 last:border-b-0"
            >
              <span>{doc.name}</span>
              <span className="text-gray-500 text-sm">{doc.date}</span>
            </div>
          ))}
        </div>
      </main>
    </div>
  );
};
