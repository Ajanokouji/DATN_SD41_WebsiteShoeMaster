import React from 'react';
import { motion } from "framer-motion";
import { Edit, Trash2, Eye } from 'lucide-react';

export const DataTable = ({ data, columns, onEdit, onDelete, onViewDetails, currentPage, pageSize }) => {
  const formatDateTime = (dateString) => {
    if (!dateString) return '';
    const date = new Date((dateString).replace(" ", "T") + "Z"); //Biến nó thành giờ UTC chuẩn vì trong db đang ko lưu giờ UTC chuẩn
    return date.toLocaleString();
  };

  const getStatusText = (status) => {
    return status === 1 ? 'Hoạt động' : 'Không hoạt động';
  };

  const getStatusClass = (status) => {
    return status === 1 ? 'text-green-500' : 'text-red-500';
  };

  const getVoucherType = (type) => {
    return type === 1 ? 'Giảm giá theo phần trăm' : 'Giảm giá theo số tiền';
  };

  const getDiscountValue = (voucher) => {
    if (voucher.voucherType === 1) {
      return `${voucher.discountPercentage}%`;
    } else {
      return `${voucher.discountAmount?.toLocaleString()}đ`;
    }
  };

  const isEmpty = !data || !Array.isArray(data) || data.length === 0;

  return (
    <motion.div
      className="bg-gray-800 bg-opacity-50 backdrop-blur-md shadow-lg rounded-xl p-6 border border-gray-700 mb-8"
      initial={{ opacity: 0, y: 20 }}
      animate={{ opacity: 1, y: 0 }}
      transition={{ delay: 0.2 }}
    >
    <div className="overflow-x-auto">
        <table className="min-w-full divide-y divide-gray-700">
          <thead>
            <tr>
              <th className="px-6 py-3 text-left text-xs font-medium text-gray-400 uppercase tracking-wider w-1/8">STT</th>
              {columns.map((column) => (
                <th 
                  key={column.key}
                  className="px-6 py-3 text-left text-xs font-medium text-gray-400 uppercase tracking-wider w-1/6"
                >
                  {column.label}
                </th>
              ))}
              <th className="px-6 py-3 text-left text-xs font-medium text-gray-400 uppercase tracking-wider w-1/6">
                Hành động
              </th>
            </tr>
          </thead>

          <tbody className="divide-y divide-gray-700">
            {isEmpty ? (
              <tr>
                <td 
                  colSpan={columns.length + 1} 
                  className="px-6 py-4 whitespace-nowrap text-sm text-gray-300 text-center"
                >
                  Trống
                </td>
              </tr>
            ) : (
              data.map((item, index) => (
                <motion.tr
                  key={item.id}
                  initial={{ opacity: 0 }}
                  animate={{ opacity: 1 }}
                  transition={{ duration: 0.3 }}
                >
                  <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-300">
                    {((currentPage - 1) * pageSize) + (index + 1)}
                  </td>
                  {columns.map((column) => (
                    <td 
                      key={`${item.id}-${column.key}`}
                      className="px-6 py-4 whitespace-normal text-sm text-gray-300"
                    >
                      {column.key === 'startDate' || column.key === 'endDate'
                        ? formatDateTime(item[column.key])
                        : column.key === 'status'
                        ? (
                          <div className={`flex items-center p-2 rounded ${getStatusClass(item[column.key])}`}>
                            {getStatusText(item[column.key])}
                </div>
                        )
                        : column.key === 'voucherType'
                        ? getVoucherType(item[column.key])
                        : column.key === 'discountValue'
                        ? getDiscountValue(item)
                        : item[column.key]}
                    </td>
                  ))}
                  <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-300">
                    <button 
                      className="text-blue-400 hover:text-blue-300 mr-2"
                      onClick={() => onViewDetails(item)}
                    >
                      <Eye size={18} />
                    </button>
                    <button 
                      className="text-indigo-400 hover:text-indigo-300 mr-2"
                      onClick={() => onEdit(item)}
                    >
                      <Edit size={18} />
                    </button>
                    <button 
                      className="text-red-400 hover:text-red-300"
                      onClick={() => onDelete(item.id)}
                    >
                      <Trash2 size={18} />
                    </button>
                  </td>
                </motion.tr>
              ))
            )}
          </tbody>
        </table>
    </div>
    </motion.div>
  );
}; 