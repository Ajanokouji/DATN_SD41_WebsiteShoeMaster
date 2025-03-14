import { PaginationProps } from "@/types/common/pagination";
import { ChevronLeft, ChevronRight } from "lucide-react";
import React, { useState } from "react";

const Pagination: React.FC<PaginationProps> = ({
  currentPage,
  totalPages,
  pageSize,
  totalRecords,
  onPageChange,
  onPageSizeChange,
}) => {
  const pageSizeOptions = [20, 50, 100];
  const pageChunkSize = 6; // Hiển thị mỗi lần 6 trang
  const [startPage, setStartPage] = useState(1);

  const handleNextRange = () => {
    setStartPage((prev) => Math.min(prev + pageChunkSize, totalPages - pageChunkSize + 1));
  };

  const handlePrevRange = () => {
    setStartPage((prev) => Math.max(prev - pageChunkSize, 1));
  };

  const endPage = Math.min(startPage + pageChunkSize - 1, totalPages);

  return (
    <div className="flex items-center justify-between py-4">
      <div className="text-sm text-gray-600">
        Views {(currentPage - 1) * pageSize + 1}- 
        {Math.min(currentPage * pageSize, totalRecords)} / {totalRecords} items
      </div>

      <div className="flex items-center gap-4">
        <select
          value={pageSize}
          onChange={(e) => onPageSizeChange(parseInt(e.target.value, 10))}
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
            onClick={() => onPageChange(currentPage - 1)}
            disabled={currentPage === 1}
            className="rounded-full border border-gray-300 p-1 w-8 text-gray-600 disabled:cursor-not-allowed disabled:opacity-50"
          >
            <ChevronLeft />
          </button>

          {startPage > 1 && (
            <button
              onClick={handlePrevRange}
              className="rounded-full border p-1 w-8 border-gray-300 text-gray-600"
            >
              ...
            </button>
          )}

          {Array.from({ length: endPage - startPage + 1 }).map((_, index) => {
            const pageNumber = startPage + index;
            return (
              <button
                key={pageNumber}
                onClick={() => onPageChange(pageNumber)}
                className={`rounded-full border p-1 w-8 ${
                  currentPage === pageNumber
                    ? "bg-gray-500 text-white"
                    : "border-gray-300 text-gray-600"
                }`}
              >
                {pageNumber}
              </button>
            );
          })}

          {endPage < totalPages && (
            <button
              onClick={handleNextRange}
              className="rounded-full border p-1 w-8 border-gray-300 text-gray-600"
            >
              ...
            </button>
          )}

          <button
            onClick={() => onPageChange(currentPage + 1)}
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
