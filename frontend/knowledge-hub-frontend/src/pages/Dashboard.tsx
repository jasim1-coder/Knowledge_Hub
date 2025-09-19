import StatsCard from "../components/StatusCard";
import RecentActivity from "../components/RecentActivity";
import { Link } from "react-router-dom";

const Dashboard = () => {
  type User = {
    userName: string;
    email: string;
    roles: string[];
    userId: string;
  };

  const storedUser = localStorage.getItem("user");
  let user: User | null = null;

  if (storedUser) {
    user = JSON.parse(storedUser) as User;
    console.log(`Welcome back, ${user.userName}`);
  }

  return (
    <div className="flex h-screen">
      <main className="flex-1 p-8 bg-gray-100 overflow-y-auto">
        {/* Header */}
        <header className="mb-8">
          <h1 className="text-3xl font-bold">
            Welcome back, {user ? user.userName : "Guest"} 👋
          </h1>
          <p className="text-gray-600">
            Ready to chat with your documents? Here's what you can do today.
          </p>
        </header>

        {/* Stats Section */}
        <section className="grid grid-cols-1 md:grid-cols-3 gap-4 mb-8">
          <StatsCard title="Documents" value={0} />
          <StatsCard title="Chats" value={12} />
          <StatsCard title="Team Members" value={8} subtitle="+15% This Month" />
        </section>

        {/* Quick Actions */}
        <section className="mb-8 flex gap-4">
          <Link to="/chat">
            <button className="bg-blue-600 text-white px-6 py-3 rounded shadow hover:bg-blue-700">
              Start a New Chat
            </button>
          </Link>
          <Link to="/upload">
            <button className="bg-green-600 text-white px-6 py-3 rounded shadow hover:bg-green-700">
              Upload Documents
            </button>
          </Link>
        </section>

        {/* Recent Activity */}
        <section>
          <h2 className="text-xl font-semibold mb-4">Recent Activity</h2>
          <RecentActivity
            title="Discussed Q4 Financial Report"
            time="2 hours ago"
          />
          <RecentActivity
            title="Uploaded Marketing Strategy.pdf"
            time="1 day ago"
          />
          <RecentActivity title="Asked about team structure" time="2 days ago" />
        </section>
      </main>
    </div>
  );
};

export default Dashboard;
