import { useState, useEffect } from "react";
import axios from "axios";
import dayjs from "dayjs";

interface MessageDto {
  id: string;
  role: string; // "user" or "assistant"
  content: string;
  sentAt: string;
}

interface ChatDto {
  id: string;
  createdAt: string;
  messages: MessageDto[];
}

interface ChatResponseDto {
  answer: string;
  sources: string[];
}

export const ChatPage = () => {
  const [messages, setMessages] = useState<{ sender: string; text: string; sentAt: string }[]>([]);
  const [input, setInput] = useState("");
  const [loading, setLoading] = useState(false);

  // 🔹 Load chat history sorted by time
  useEffect(() => {
    const fetchHistory = async () => {
      const storedUser = localStorage.getItem("user");
      if (!storedUser) {
        console.warn("No user logged in.");
        return;
      }
      const user = JSON.parse(storedUser);

      try {
        const response = await axios.get<ChatDto[]>(
          `http://localhost:5117/api/Chat/user/${user.userId}`
        );

        // Flatten + sort messages by SentAt
        const historyMessages = response.data
          .flatMap((chat) =>
            chat.messages.map((m) => ({
              sender: m.role === "user" ? "You" : "Bot",
              text: m.content,
              sentAt: m.sentAt,
            }))
          )
          .sort((a, b) => new Date(a.sentAt).getTime() - new Date(b.sentAt).getTime());

        setMessages(historyMessages);
      } catch (error) {
        console.error("Failed to load chat history", error);
      }
    };

    fetchHistory();
  }, []);

  const sendMessage = async () => {
    if (!input.trim()) return;

    const now = new Date().toISOString();

    setMessages((prev) => [...prev, { sender: "You", text: input, sentAt: now }]);

    const storedUser = localStorage.getItem("user");
    if (!storedUser) {
      alert("User not logged in!");
      return;
    }
    const user = JSON.parse(storedUser);

    try {
      setLoading(true);

      const body = {
        userId: user.userId,
        question: input,
        documentIds: [], // later add selected docs
      };

      const response = await axios.post<ChatResponseDto>(
        "http://localhost:5117/api/Chat/ask",
        body
      );

      const botMessage = response.data.answer;
      const sources = response.data.sources;
      const replyTime = new Date().toISOString();

      setMessages((prev) => [
        ...prev,
        { sender: "Bot", text: botMessage, sentAt: replyTime },
        ...(sources.length > 0
          ? [{ sender: "Sources", text: sources.join(", "), sentAt: replyTime }]
          : []),
      ]);
    } catch (error) {
      console.error(error);
      setMessages((prev) => [
        ...prev,
        { sender: "Bot", text: "❌ Something went wrong. Try again.", sentAt: new Date().toISOString() },
      ]);
    } finally {
      setInput("");
      setLoading(false);
    }
  };

  // 🔹 Helper to format dates
  const formatDateLabel = (dateStr: string) => {
    const today = dayjs().startOf("day");
    const msgDate = dayjs(dateStr).startOf("day");

    if (msgDate.isSame(today)) return "Today";
    if (msgDate.isSame(today.subtract(1, "day"))) return "Yesterday";
    return msgDate.format("MMMM D, YYYY");
  };

  return (
    <div className="flex h-screen">
      <main className="flex-1 p-6 bg-gray-100 flex flex-col">
        <h1 className="text-2xl font-bold mb-4">Chat with Documents</h1>

        <div className="flex-1 overflow-y-auto bg-white rounded p-4 shadow">
          {messages.length === 0 ? (
            <p className="text-gray-500">Start chatting with your documents...</p>
          ) : (
            messages.map((msg, idx) => {
              const prevMsg = messages[idx - 1];
              const showDate =
                !prevMsg ||
                dayjs(msg.sentAt).startOf("day").diff(dayjs(prevMsg.sentAt).startOf("day")) !== 0;

              return (
                <div key={idx}>
                  {showDate && (
                    <div className="text-center text-gray-500 text-xs my-2">
                      {formatDateLabel(msg.sentAt)}
                    </div>
                  )}

                  <div
                    className={`mb-3 ${
                      msg.sender === "You"
                        ? "text-right"
                        : msg.sender === "Bot"
                        ? "text-left"
                        : "text-sm text-gray-500 italic"
                    }`}
                  >
                    <span
                      className={`px-3 py-2 inline-block rounded-lg ${
                        msg.sender === "You"
                          ? "bg-blue-100 text-gray-800"
                          : msg.sender === "Bot"
                          ? "bg-green-100 text-gray-800"
                          : ""
                      }`}
                    >
                      <strong>{msg.sender}: </strong>
                      {msg.text}
                      <div className="text-xs text-gray-400 mt-1">
                        {new Date(msg.sentAt).toLocaleTimeString([], {
                          hour: "2-digit",
                          minute: "2-digit",
                        })}
                      </div>
                    </span>
                  </div>
                </div>
              );
            })
          )}
        </div>

        <div className="mt-4 flex">
          <input
            className="flex-1 p-2 border rounded-l"
            placeholder="Ask something..."
            value={input}
            onChange={(e) => setInput(e.target.value)}
            disabled={loading}
          />
          <button
            onClick={sendMessage}
            disabled={loading}
            className={`px-4 rounded-r text-white ${
              loading
                ? "bg-gray-400 cursor-not-allowed"
                : "bg-blue-600 hover:bg-blue-700"
            }`}
          >
            {loading ? "Thinking..." : "Send"}
          </button>
        </div>
      </main>
    </div>
  );
};
