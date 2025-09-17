import { useState } from "react";

export default function SettingsPage() {
  const [activeTab, setActiveTab] = useState<"profile" | "security" | "notifications">("profile");

  return (
    <div className="p-6">
      <h1 className="text-2xl font-bold mb-6">Settings</h1>

      {/* Tabs */}
      <div className="flex space-x-4 border-b mb-6">
        <button
          onClick={() => setActiveTab("profile")}
          className={`pb-2 px-4 border-b-2 ${
            activeTab === "profile"
              ? "border-indigo-600 text-indigo-600 font-medium"
              : "border-transparent text-gray-600 hover:text-indigo-600"
          }`}
        >
          Profile
        </button>
        <button
          onClick={() => setActiveTab("security")}
          className={`pb-2 px-4 border-b-2 ${
            activeTab === "security"
              ? "border-indigo-600 text-indigo-600 font-medium"
              : "border-transparent text-gray-600 hover:text-indigo-600"
          }`}
        >
          Security
        </button>
        <button
          onClick={() => setActiveTab("notifications")}
          className={`pb-2 px-4 border-b-2 ${
            activeTab === "notifications"
              ? "border-indigo-600 text-indigo-600 font-medium"
              : "border-transparent text-gray-600 hover:text-indigo-600"
          }`}
        >
          Notifications
        </button>
      </div>

      {/* Content */}
      <div className="bg-white rounded-xl shadow p-6 space-y-6">
        {activeTab === "profile" && (
          <div className="space-y-4">
            <h2 className="text-lg font-semibold">Profile Settings</h2>
            <div>
              <label className="block text-gray-700 font-medium">Display Name</label>
              <input
                type="text"
                className="border rounded-lg p-2 w-full mt-1"
                placeholder="John Doe"
              />
            </div>
            <div>
              <label className="block text-gray-700 font-medium">Email</label>
              <input
                type="email"
                className="border rounded-lg p-2 w-full mt-1"
                placeholder="john@example.com"
              />
            </div>
            <button className="bg-indigo-600 text-white px-4 py-2 rounded-lg hover:bg-indigo-700">
              Save Changes
            </button>
          </div>
        )}

        {activeTab === "security" && (
          <div className="space-y-4">
            <h2 className="text-lg font-semibold">Security Settings</h2>
            <div>
              <label className="block text-gray-700 font-medium">Current Password</label>
              <input
                type="password"
                className="border rounded-lg p-2 w-full mt-1"
                placeholder="••••••••"
              />
            </div>
            <div>
              <label className="block text-gray-700 font-medium">New Password</label>
              <input
                type="password"
                className="border rounded-lg p-2 w-full mt-1"
                placeholder="••••••••"
              />
            </div>
            <div>
              <label className="block text-gray-700 font-medium">Confirm New Password</label>
              <input
                type="password"
                className="border rounded-lg p-2 w-full mt-1"
                placeholder="••••••••"
              />
            </div>
            <button className="bg-red-600 text-white px-4 py-2 rounded-lg hover:bg-red-700">
              Update Password
            </button>
          </div>
        )}

        {activeTab === "notifications" && (
          <div className="space-y-4">
            <h2 className="text-lg font-semibold">Notification Preferences</h2>
            <div className="flex items-center">
              <input type="checkbox" id="email" className="mr-2" />
              <label htmlFor="email" className="text-gray-700">
                Email Notifications
              </label>
            </div>
            <div className="flex items-center">
              <input type="checkbox" id="sms" className="mr-2" />
              <label htmlFor="sms" className="text-gray-700">
                SMS Notifications
              </label>
            </div>
            <div className="flex items-center">
              <input type="checkbox" id="app" className="mr-2" />
              <label htmlFor="app" className="text-gray-700">
                In-App Notifications
              </label>
            </div>
            <button className="bg-green-600 text-white px-4 py-2 rounded-lg hover:bg-green-700">
              Save Preferences
            </button>
          </div>
        )}
      </div>
    </div>
  );
}
