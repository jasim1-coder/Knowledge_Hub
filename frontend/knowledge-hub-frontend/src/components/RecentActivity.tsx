import React from "react";

interface ActivityProps {
  title: string;
  time: string;
}

const RecentActivity: React.FC<ActivityProps> = ({ title, time }) => {
  return (
    <div className="bg-white shadow rounded p-3 mb-2">
      <p className="font-medium">{title}</p>
      <p className="text-gray-400 text-sm">{time}</p>
    </div>
  );
};

export default RecentActivity;
