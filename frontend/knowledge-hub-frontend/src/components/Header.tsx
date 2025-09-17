import React from 'react';
import { Search } from 'lucide-react';

const Header: React.FC = () => {
  return (
    <div className="bg-white border-b border-gray-200 px-6 py-3 flex items-center justify-between">
      <div className="flex items-center gap-4">
        <button className="p-2 hover:bg-gray-100 rounded-lg">
          <div className="w-6 h-6 bg-blue-500 rounded flex items-center justify-center">
            <span className="text-white text-sm font-bold">KH</span>
          </div>
        </button>
        <span className="font-semibold text-gray-900">Knowledge Hub</span>
      </div>
      
      <div className="flex-1 max-w-md mx-4">
        <div className="relative">
          <Search className="absolute left-3 top-1/2 transform -translate-y-1/2 text-gray-400" size={16} />
          <input 
            type="text" 
            placeholder="Search documents, chats..."
            className="w-full pl-10 pr-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-transparent"
          />
        </div>
      </div>
      
      <div className="flex items-center gap-3">
        <button className="p-2 hover:bg-gray-100 rounded-lg">
          <div className="w-8 h-8 bg-gray-300 rounded-full flex items-center justify-center">
            <span className="text-gray-600 font-medium text-sm">JD</span>
          </div>
        </button>
      </div>
    </div>
  );
};

export default Header;