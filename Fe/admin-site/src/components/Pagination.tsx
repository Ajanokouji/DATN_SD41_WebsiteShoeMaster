import { ChevronLeft, ChevronRight } from "lucide-react";
import React, { useState } from "react";

const Pagination = () => {
  const [currentPage, setCurrentPage] = useState(1);
  const totalPages = 6; // Tổng số trang
  const pageSizeOptions = [20, 50, 100];
  const [pageSize, setPageSize] = useState(20);

  const handlePageChange = (page: number) => {
    setCurrentPage(page);
  };

  const handlePageSizeChange = (e: React.ChangeEvent<HTMLSelectElement>) => {
    setPageSize(parseInt(e.target.value, 10));
    setCurrentPage(1);
  };

  return (
    <div className="flex items-center justify-between py-4">
      <div className="text-sm text-gray-600">
        Viewss {(currentPage - 1) * pageSize + 1}-
        {Math.min(currentPage * pageSize, 3459)} / 3459 category
      </div>

      <div className="flex items-center gap-4">
        <select
          value={pageSize}
          onChange={handlePageSizeChange}
          className="rounded-full border border-gray-300 bg-white px-2 py-1 text-sm focus:outline-none focus:ring-2 focus:ring-gray-500"
        >
          {pageSizeOptions.map((size) => (
            <option key={size} value={size}>
              {size} / page
            </option>
          ))}
        </select>

        <div className="flex items-center gap-1">
          <button
            onClick={() => handlePageChange(currentPage - 1)}
            disabled={currentPage === 1}
            className="rounded-full border border-gray-300 p-1 w-8 text-gray-600 disabled:cursor-not-allowed disabled:opacity-50"
          >
            <ChevronLeft />
          </button>

          {Array.from({ length: totalPages }).map((_, index) => (
            <button
              key={index}
              onClick={() => handlePageChange(index + 1)}
              className={`rounded-full border p-1 w-8 ${
                currentPage === index + 1
                  ? "bg-gray-500 text-white"
                  : "border-gray-300 text-gray-600"
              }`}
            >
              {index + 1}
            </button>
          ))}

          <button
            onClick={() => handlePageChange(currentPage + 1)}
            disabled={currentPage === totalPages}
            className="rounded-full border border-gray-300 p-1 w-8 text-gray-600 disabled:cursor-not-allowed disabled:opacity-50"
          >
            <ChevronRight />
          </button>
        </div>
      </div>
    </div>
  );
};

export default Pagination;
