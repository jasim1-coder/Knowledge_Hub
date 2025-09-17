import { Link, useLocation } from "react-router-dom";
import {
  LayoutDashboard,
  Upload,
  MessageCircle,
  FileText,
  Settings,
  User,
  LogOut,
} from "lucide-react";
import { useState } from "react";

const Navigation = () => {
  const { pathname } = useLocation();
  const [open, setOpen] = useState(false);

  const navItems = [
    { name: "Dashboard", path: "/", icon: <LayoutDashboard size={18} /> },
    { name: "Upload Documents", path: "/upload", icon: <Upload size={18} /> },
    { name: "Chat", path: "/chat", icon: <MessageCircle size={18} /> },
    { name: "Documents", path: "/documents", icon: <FileText size={18} /> },
    { name: "Settings", path: "/settings", icon: <Settings size={18} /> },
  ];

  return (
    <aside className="w-64 h-screen bg-gray-800 text-white flex flex-col p-4">
      {/* Brand */}
      <h1 className="text-2xl font-bold mb-8">Knowledge Hub</h1>

      {/* Main Nav */}
      <nav className="flex flex-col gap-2">
        {navItems.map((item) => (
          <Link
            key={item.path}
            to={item.path}
            className={`flex items-center gap-2 p-2 rounded hover:bg-gray-700 transition-colors ${
              pathname === item.path ? "bg-gray-700" : ""
            }`}
          >
            {item.icon} {item.name}
          </Link>
        ))}
      </nav>

      {/* Quick Links */}
      <div className="mt-auto">
        <h2 className="text-lg font-semibold mt-6">Quick Links</h2>
        <ul className="mt-2 flex flex-col gap-2 text-sm">
          <li className="hover:underline cursor-pointer">Recent Chats</li>
          <li className="hover:underline cursor-pointer">My Documents</li>
          <li className="hover:underline cursor-pointer">Teams</li>
        </ul>
      </div>

      {/* Profile Section */}
      <div className="relative mt-6">
        <button
          onClick={() => setOpen(!open)}
          className="flex items-center gap-3 w-full p-2 rounded hover:bg-gray-700 transition-colors"
        >
          <div className="w-8 h-8 rounded-full bg-gray-600 flex items-center justify-center text-sm font-bold">
            J
          </div>
          <span className="flex-1 text-left">John Doe</span>
        </button>

        {open && (
          <div className="absolute bottom-12 left-0 w-full bg-gray-900 border border-gray-700 rounded shadow-lg">
            <Link
              to="/settings"
              className="flex items-center gap-2 px-3 py-2 hover:bg-gray-700"
            >
              <User size={16} /> Profile Settings
            </Link>
            <button
              onClick={() => alert("Logging out...")}
              className="flex items-center gap-2 w-full text-left px-3 py-2 hover:bg-gray-700"
            >
              <LogOut size={16} /> Logout
            </button>
          </div>
        )}
      </div>
    </aside>
  );
};

export default Navigation;
