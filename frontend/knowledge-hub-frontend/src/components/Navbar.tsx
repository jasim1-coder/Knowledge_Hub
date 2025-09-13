// src/components/Navbar.tsx
import { Link } from "react-router-dom";

export const Navbar = () => {
  return (
    <nav style={{ padding: "1rem", background: "#1E40AF", color: "#fff" }}>
      <h1 style={{ display: "inline", marginRight: "2rem" }}>Knowledge Hub</h1>
      <Link style={{ marginRight: "1rem", color: "#fff" }} to="/">Home</Link>
      <Link style={{ marginRight: "1rem", color: "#fff" }} to="/chat">Chat</Link>
      <Link style={{ color: "#fff" }} to="/upload">Upload</Link>
    </nav>
  );
};
