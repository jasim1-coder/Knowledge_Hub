import  { useState } from "react";
import Navigation from "../components/Navigation";

export const ChatPage = () => {
  const [messages, setMessages] = useState<{ sender: string; text: string }[]>(
    []
  );
  const [input, setInput] = useState("");

  const sendMessage = () => {
    if (!input.trim()) return;
    setMessages([...messages, { sender: "You", text: input }]);
    setInput("");
  };

  return (
    <div className="flex h-screen">
      <Navigation />
      <main className="flex-1 p-6 bg-gray-100 flex flex-col">
        <h1 className="text-2xl font-bold mb-4">Chat with Documents</h1>
        
        <div className="flex-1 overflow-y-auto bg-white rounded p-4 shadow">
          {messages.length === 0 ? (
            <p className="text-gray-500">Start chatting with your documents...</p>
          ) : (
            messages.map((msg, idx) => (
              <div
                key={idx}
                className={`mb-3 ${
                  msg.sender === "You" ? "text-right" : "text-left"
                }`}
              >
                <span className="px-3 py-2 inline-block rounded-lg bg-blue-100 text-gray-800">
                  <strong>{msg.sender}: </strong>
                  {msg.text}
                </span>
              </div>
            ))
          )}
        </div>

        <div className="mt-4 flex">
          <input
            className="flex-1 p-2 border rounded-l"
            placeholder="Ask something..."
            value={input}
            onChange={(e) => setInput(e.target.value)}
          />
          <button
            onClick={sendMessage}
            className="bg-blue-600 text-white px-4 rounded-r hover:bg-blue-700"
          >
            Send
          </button>
        </div>
      </main>
    </div>
  );
};
