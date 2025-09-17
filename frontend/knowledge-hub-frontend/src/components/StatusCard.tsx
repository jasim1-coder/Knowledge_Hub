import React from "react";

interface StatsCardProps {
  title: string;
  value: number;
  subtitle?: string;
}

const StatsCard: React.FC<StatsCardProps> = ({ title, value, subtitle }) => {
  return (
    <div className="bg-white shadow rounded p-4 flex flex-col">
      <h3 className="text-gray-500">{title}</h3>
      <p className="text-2xl font-bold">{value}</p>
      {subtitle && <span className="text-green-500 text-sm">{subtitle}</span>}
    </div>
  );
};

export default StatsCard;
