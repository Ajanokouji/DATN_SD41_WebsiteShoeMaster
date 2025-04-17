import React, { useState, useEffect } from 'react';
import { motion } from "framer-motion";
import { X } from "lucide-react";
import { isVoucherCodeExist } from '../../services/vouchersService';

export const VoucherForm = ({ voucher, onSubmit, onCancel }) => {
  const [formData, setFormData] = useState({
    code: '',
    voucherName: '',
    voucherType: 1,
    discountPercentage: 0,
    discountAmount: 0,
    description: '',
    startDate: '',
    endDate: '',
    status: 1,
    minimumOrderAmount: 0
  });

  const [errorMessage, setErrorMessage] = useState('');
  const [loading, setLoading] = useState(false);

  useEffect(() => {
    if (voucher) {
      const formatDateTimeLocal = (dateStr) => {
        if (!dateStr) return '';
        const utcDate = new Date(dateStr.replace(" ", "T") + "Z"); // Coi là UTC
        const localDate = new Date(utcDate.getTime() - utcDate.getTimezoneOffset() * 60000);
        return localDate.toISOString().slice(0, 19);
      };

      setFormData({
        ...voucher,
        startDate: voucher.startDate ? formatDateTimeLocal(voucher.startDate) : '',
        endDate: voucher.endDate ? formatDateTimeLocal(voucher.endDate) : ''
      });
    }
  }, [voucher]);

  const formatCurrency = (amount) => {
    if (!amount && amount !== 0) return '';
    return new Intl.NumberFormat('vi-VN').format(amount).replace(/\./g, ',');
  };

  const handleChange = (e) => {
    const { name, value } = e.target;

    if (['startDate', 'endDate'].includes(name)) {
      setErrorMessage('');
    }

    setFormData(prev => ({
      ...prev,
      [name]: ['voucherType', 'status'].includes(name) ? Number(value) : value
    }));
  };

  const handleSubmit = async (e) => {
    e.preventDefault();

    if (new Date(formData.startDate) > new Date(formData.endDate)) {
      setErrorMessage('⛔ Ngày bắt đầu không được lớn hơn ngày kết thúc.');
      return;
    }

    setErrorMessage('');
    setLoading(true);

    try {
      const codeExists = await isVoucherCodeExist(formData.code, voucher ? voucher.id : null);
      if (codeExists) {
        setErrorMessage('⛔ Mã voucher đã tồn tại.');
        setLoading(false);
        return;
      }

      const dataToSubmit = {
        ...formData,
        discountAmount: formData.voucherType === 2 ? Number(formData.discountAmount) : 0,
        discountPercentage: formData.voucherType === 1 ? Number(formData.discountPercentage) : 0,
        minimumOrderAmount: Number(formData.minimumOrderAmount),
        startDate: new Date(formData.startDate).toISOString(),
        endDate: new Date(formData.endDate).toISOString()
      };

      onSubmit(dataToSubmit);
    } catch (error) {
      setErrorMessage('⛔ Đã xảy ra lỗi khi kiểm tra mã voucher.');
    } finally {
      setLoading(false);
    }
  };

  const handleOverlayClick = (e) => {
    if (e.target === e.currentTarget) {
      onCancel();
    }
  };

  return (
    <div 
      className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50 overflow-y-auto py-8"
      onClick={handleOverlayClick}
    >
      <motion.div
        className="bg-gray-800 rounded-lg p-6 w-full max-w-2xl mx-4 my-auto"
        initial={{ opacity: 0, scale: 0.95 }}
        animate={{ opacity: 1, scale: 1 }}
        exit={{ opacity: 0, scale: 0.95 }}
      >
        <div className="flex justify-between items-center mb-6">
          <h2 className="text-xl font-semibold text-white">
            {voucher ? 'Chỉnh Sửa Voucher' : 'Thêm Voucher Mới'}
          </h2>
          <button
            onClick={onCancel}
            className="text-gray-400 hover:text-white transition-colors"
          >
            <X size={24} />
          </button>
        </div>

        {errorMessage && (
          <div className="mb-4 p-3 rounded-md bg-red-600 text-white text-sm font-medium shadow-sm">
            {errorMessage}
          </div>
        )}

        <form onSubmit={handleSubmit} className="space-y-4">
          <div className="grid grid-cols-2 gap-4">
            <div>
              <label className="block text-sm font-medium text-gray-400 mb-1">
                Mã Voucher
              </label>
              <input
                type="text"
                name="code"
                value={formData.code}
                onChange={handleChange}
                className="w-full bg-gray-700 text-white rounded-lg px-3 py-2 focus:outline-none focus:ring-2 focus:ring-blue-500"
                required
              />
            </div>

            <div>
              <label className="block text-sm font-medium text-gray-400 mb-1">
                Tên Voucher
              </label>
              <input
                type="text"
                name="voucherName"
                value={formData.voucherName}
                onChange={handleChange}
                className="w-full bg-gray-700 text-white rounded-lg px-3 py-2 focus:outline-none focus:ring-2 focus:ring-blue-500"
                required
              />
            </div>

            <div>
              <label className="block text-sm font-medium text-gray-400 mb-1">
                Loại Giảm Giá
              </label>
              <select
                name="voucherType"
                value={formData.voucherType}
                onChange={handleChange}
                className="w-full bg-gray-700 text-white rounded-lg px-3 py-2 focus:outline-none focus:ring-2 focus:ring-blue-500"
              >
                <option value={1}>Giảm giá theo phần trăm</option>
                <option value={2}>Giảm giá theo số tiền</option>
              </select>
            </div>

            <div>
              <label className="block text-sm font-medium text-gray-400 mb-1">
                {formData.voucherType === 1 ? 'Phần Trăm Giảm (%)' : 'Số Tiền Giảm (VNĐ)'}
              </label>
              <input
                type="number"
                step={formData.voucherType === 1 ? "0.01" : "1"}
                name={formData.voucherType === 1 ? 'discountPercentage' : 'discountAmount'}
                value={formData.voucherType === 1 ? formData.discountPercentage : formData.discountAmount}
                onChange={handleChange}
                className="w-full bg-gray-700 text-white rounded-lg px-3 py-2 focus:outline-none focus:ring-2 focus:ring-blue-500"
                required
                min="0"
                max={formData.voucherType === 1 ? "100" : undefined}
              />
            </div>

            <div>
              <label className="block text-sm font-medium text-gray-400 mb-1">
                Ngày Bắt Đầu
              </label>
              <input
                type="datetime-local"
                step="1"
                name="startDate"
                value={formData.startDate}
                onChange={handleChange}
                className="w-full bg-gray-700 text-white rounded-lg px-3 py-2 focus:outline-none focus:ring-2 focus:ring-blue-500"
                required
              />
            </div>

            <div>
              <label className="block text-sm font-medium text-gray-400 mb-1">
                Ngày Kết Thúc
              </label>
              <input
                type="datetime-local"
                step="1"
                name="endDate"
                value={formData.endDate}
                onChange={handleChange}
                className="w-full bg-gray-700 text-white rounded-lg px-3 py-2 focus:outline-none focus:ring-2 focus:ring-blue-500"
                required
              />
            </div>

            <div>
              <label className="block text-sm font-medium text-gray-400 mb-1">
                Số Tiền Tối Thiểu
              </label>
              <input
                type="number"
                name="minimumOrderAmount"
                value={formData.minimumOrderAmount}
                onChange={handleChange}
                className="w-full bg-gray-700 text-white rounded-lg px-3 py-2 focus:outline-none focus:ring-2 focus:ring-blue-500"
                required
                min="0"
              />
              <span className="text-sm text-gray-400 mt-1 inline-block">
                {formatCurrency(formData.minimumOrderAmount)} đ
              </span>
            </div>

            <div>
              <label className="block text-sm font-medium text-gray-400 mb-1">
                Trạng Thái
              </label>
              <select
                name="status"
                value={formData.status}
                onChange={handleChange}
                className="w-full bg-gray-700 text-white rounded-lg px-3 py-2 focus:outline-none focus:ring-2 focus:ring-blue-500"
              >
                <option value={1}>Hoạt động</option>
                <option value={0}>Không hoạt động</option>
              </select>
            </div>
          </div>

          <div>
            <label className="block text-sm font-medium text-gray-400 mb-1">
              Mô Tả
            </label>
            <textarea
              name="description"
              value={formData.description}
              onChange={handleChange}
              className="w-full bg-gray-700 text-white rounded-lg px-3 py-2 focus:outline-none focus:ring-2 focus:ring-blue-500"
              rows="2"
              style={{ resize: 'vertical', minHeight: '60px' }}
            />
          </div>

          <div className="flex justify-end gap-3 mt-6">
            <button
              type="button"
              onClick={onCancel}
              className="px-4 py-2 bg-gray-600 text-white rounded-lg hover:bg-gray-500 transition-colors"
            >
              Hủy
            </button>
            <button
              type="submit"
              className="px-4 py-2 bg-indigo-500 text-white rounded-lg hover:bg-indigo-600 transition-colors"
            >
              {voucher ? 'Cập Nhật' : 'Thêm Mới'}
            </button>
          </div>
        </form>
      </motion.div>

      {/* Màn hình loading */}
      {loading && (
        <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50">
          <div className="bg-gray-800 text-white p-4 rounded-lg">
            <span>Đang kiểm tra mã Voucher...</span>
            {/* Bạn có thể thêm một spinner ở đây nếu muốn */}
          </div>
        </div>
      )}
    </div>
  );
};