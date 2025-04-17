import React from 'react';
import { motion } from "framer-motion";
import { X } from "lucide-react";

const VoucherDetailsForm = ({ voucher, onClose }) => {
  const handleOutsideClick = (e) => {
    if (e.target === e.currentTarget) {
      onClose();
    }
  };

  const displayValue = (value, fallback = 'Không có') => {
    return value ? value : fallback;
  };

  const formatDate = (dateStr) => {
    //Biến nó thành giờ UTC chuẩn vì trong db đang ko lưu giờ UTC chuẩn
    return dateStr ? new Date((dateStr).replace(" ", "T") + 'Z').toLocaleString() : 'Chưa xác định';
  };

  const formatCurrency = (amount) => {
    if (!amount && amount !== 0) return 'Không có';
    return new Intl.NumberFormat('vi-VN')
      .format(amount)
      .replace(/\./g, ',') + 'đ';
  };

  return (
    <div
      className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50 overflow-y-auto py-8"
      onClick={handleOutsideClick}
    >
      <motion.div
        className="bg-gray-800 rounded-lg p-6 w-full max-w-2xl mx-4 my-auto"
        initial={{ opacity: 0, scale: 0.95 }}
        animate={{ opacity: 1, scale: 1 }}
        exit={{ opacity: 0, scale: 0.95 }}
      >
        {/* Header */}
        <div className="flex justify-between items-center border-b border-gray-700 pb-4 mb-6">
          <h2 className="text-xl font-semibold text-white">Chi Tiết Voucher</h2>
          <button
            onClick={onClose}
            className="text-gray-400 hover:text-white transition-colors"
          >
            <X size={26} />
          </button>
        </div>

        {/* Content */}
        <div className="grid grid-cols-1 md:grid-cols-2 gap-6 text-sm text-gray-300">
          <div>
            <span className="block text-gray-400 font-medium">Mã Voucher:</span>
            <span className="block">{voucher.code}</span>
          </div>

          <div>
            <span className="block text-gray-400 font-medium">Tên Voucher:</span>
            <span className="block">{voucher.voucherName}</span>
          </div>

          <div>
            <span className="block text-gray-400 font-medium">Giá Trị:</span>
            <span className="block">
              {voucher.discountAmount
                ? formatCurrency(voucher.discountAmount)
                : `${voucher.discountPercentage}%`}
            </span>
          </div>

          <div>
            <span className="block text-gray-400 font-medium">Số Tiền Tối Thiểu:</span>
            <span className="block">
              {voucher.minimumOrderAmount
                ? formatCurrency(voucher.minimumOrderAmount)
                : 'Không có'}
            </span>
          </div>

          <div>
            <span className="block text-gray-400 font-medium">Ngày Bắt Đầu:</span>
            <span className="block">{formatDate(voucher.startDate)}</span>
          </div>

          <div>
            <span className="block text-gray-400 font-medium">Ngày Kết Thúc:</span>
            <span className="block">{formatDate(voucher.endDate)}</span>
          </div>

          <div>
            <span className="block text-gray-400 font-medium">Ngày Tạo:</span>
            <span className="block">{formatDate(voucher.createdOnDate)}</span>
          </div>

          <div>
            <span className="block text-gray-400 font-medium">Ngày Cập Nhật:</span>
            <span className="block">{formatDate(voucher.lastModifiedOnDate)}</span>
          </div>

          <div>
            <span className="block text-gray-400 font-medium">Trạng Thái:</span>
            <span className="block">{voucher.status === 1 ? 'Hoạt động' : 'Không hoạt động'}</span>
          </div>

          <div>
            <span className="block text-gray-400 font-medium">Mô Tả:</span>
            <span className="block">{displayValue(voucher.description)}</span>
          </div>
        </div>

        {/* Footer */}
        <div className="flex justify-end mt-8">
          <button
            onClick={onClose}
            className="bg-indigo-600 hover:bg-indigo-700 text-white font-medium px-5 py-2 rounded-lg transition-all"
          >
            Đóng
          </button>
        </div>
      </motion.div>
    </div>
  );
};

export default VoucherDetailsForm;