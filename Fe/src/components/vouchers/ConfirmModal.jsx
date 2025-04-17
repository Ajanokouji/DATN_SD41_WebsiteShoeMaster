import React from 'react';
import { motion } from 'framer-motion';

const ConfirmModal = ({ message, onConfirm, onCancel }) => {
  const handleOverlayClick = (e) => {
    if (e.target === e.currentTarget) {
      onCancel();
    }
  };

  return (
    <motion.div
      initial={{ opacity: 0 }}
      animate={{ opacity: 1 }}
      exit={{ opacity: 0 }}
      className="fixed inset-0 z-[100] flex items-center justify-center bg-black bg-opacity-50"
      onClick={handleOverlayClick}
    >
      <motion.div
        initial={{ scale: 0.9 }}
        animate={{ scale: 1 }}
        exit={{ scale: 0.9 }}
        className="bg-gray-800 text-white rounded-xl p-6 shadow-xl max-w-sm w-full"
        onClick={(e) => e.stopPropagation()} // Ngăn nổi bọt để không đóng khi click vào modal
      >
        <h3 className="text-lg font-semibold mb-4">Xác nhận</h3>
        <p className="mb-6">{message}</p>
        <div className="flex justify-end gap-4">
          <button
            onClick={onCancel}
            className="px-4 py-2 bg-gray-600 hover:bg-gray-700 rounded"
          >
            Hủy
          </button>
          <button
            onClick={onConfirm}
            className="px-4 py-2 bg-indigo-500 hover:bg-indigo-600 rounded"
          >
            Đồng ý
          </button>
        </div>
      </motion.div>
    </motion.div>
  );
};

export default ConfirmModal;