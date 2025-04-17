import React, { useState, useEffect } from 'react';
import { motion } from "framer-motion";
import { ChevronLeft, ChevronRight, RefreshCw } from "lucide-react";
import { getVouchers, createVoucher, updateVoucher, deleteVoucher } from '../services/vouchersService';
import { VoucherForm } from '../components/vouchers/VoucherForm';
import { DataTable } from '../components/vouchers/DataTable';
import VoucherDetailsForm from '../components/vouchers/VoucherDetailsForm';
import Header from "../components/common/Header";
import ConfirmModal from "../components/vouchers/ConfirmModal";
import { useSearchParams, useNavigate } from 'react-router-dom';

const ErrorForm = ({ message, onRetry }) => (
  <motion.div
    initial={{ opacity: 0, scale: 0.95 }}
    animate={{ opacity: 1, scale: 1 }}
    exit={{ opacity: 0, scale: 0.95 }}
    className="relative z-10"
  >
    <div className="bg-gray-800 bg-opacity-90 p-8 rounded-xl shadow-xl max-w-md w-full mx-auto">
      <div className="text-center">
        <h3 className="text-xl font-semibold text-red-400 mb-2">Đã xảy ra lỗi</h3>
        <p className="text-gray-300 mb-6">{message}</p>
        <button
          onClick={onRetry}
          className="bg-indigo-500 hover:bg-indigo-600 text-white px-6 py-2 rounded-lg flex items-center gap-2 mx-auto transition-colors"
        >
          <RefreshCw size={18} />
          <span>Tải lại</span>
        </button>
      </div>
    </div>
  </motion.div>
);

const VouchersPage = () => {
  const [vouchers, setVouchers] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const [showError, setShowError] = useState(false);
  const [showForm, setShowForm] = useState(false);
  const [showDetails, setShowDetails] = useState(false);
  const [selectedVoucher, setSelectedVoucher] = useState(null);
  const [searchParams, setSearchParams] = useSearchParams();
  const navigate = useNavigate();

  const isInteger = (value) => Number.isInteger(Number(value)) && !isNaN(value);
  const validatePage = (page, totalPages) => (!isInteger(page) || page < 1 || page > totalPages ? 1 : parseInt(page));
  const validateSize = (size) => (!isInteger(size) || ![5, 10, 20, 50].includes(parseInt(size)) ? 10 : parseInt(size));

  const [currentPage, setCurrentPage] = useState(() => validatePage(searchParams.get('page'), 1));
  const [pageSize, setPageSize] = useState(() => validateSize(searchParams.get('size')));
  const [totalPages, setTotalPages] = useState(1);
  const [totalItems, setTotalItems] = useState(0);

  const [submitting, setSubmitting] = useState(false);
  const [submitMessage, setSubmitMessage] = useState('');

  const [confirmModal, setConfirmModal] = useState({
    show: false,
    message: '',
    onConfirm: null,
    onCancel: null,
  });

  const fetchVouchers = async () => {
    try {
      setLoading(true);
      setError(null);
      setShowError(false);
      const response = await getVouchers({ CurrentPage: currentPage, PageSize: pageSize });
      if (response?.data) {
        const data = response.data;
        setTotalPages(data.totalPages || 1);
        setTotalItems(data.totalRecords || 0);
        setVouchers(data.content || []);
      } else {
        setVouchers([]);
        setTotalPages(1);
        setTotalItems(0);
      }
    } catch (err) {
      console.error('Error fetching vouchers:', err);
      setError(err.message || 'Có lỗi xảy ra khi tải dữ liệu');
      setShowError(true);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    const page = searchParams.get('page');
    const validatedPage = validatePage(page, totalPages);
    if (validatedPage !== currentPage) {
      setCurrentPage(validatedPage);
    }
  }, [searchParams, totalPages]);

  useEffect(() => {
    if (currentPage !== null) fetchVouchers();
  }, [currentPage, pageSize]);

  const handleEdit = (voucher) => {
    setSelectedVoucher(voucher);
    setShowForm(true);
  };

  const handleDelete = (id) => {
    setConfirmModal({
      show: true,
      message: 'Bạn có chắc chắn muốn xóa voucher này?',
      onConfirm: async () => {
        setConfirmModal({ show: false });
        try {
          setSubmitting(true);
          await deleteVoucher(id);
          fetchVouchers();
          setSubmitMessage('🗑️ Xóa voucher thành công!');
        } catch (err) {
          console.error('Error deleting voucher:', err);
          setError(err.message || 'Có lỗi xảy ra khi xóa voucher');
          setShowError(true);
          setSubmitMessage('❌ Xóa voucher thất bại. Vui lòng thử lại.');
        } finally {
          setSubmitting(false);
          setTimeout(() => setSubmitMessage(''), 3000);
        }
      },
      onCancel: () => setConfirmModal({ show: false }),
    });
  };

  const handleSubmit = async (voucherData) => {
    const isEdit = !!selectedVoucher;
    const confirmMessage = isEdit
      ? 'Bạn có chắc chắn muốn cập nhật voucher này?'
      : 'Bạn có chắc chắn muốn thêm voucher mới?';

    setConfirmModal({
      show: true,
      message: confirmMessage,
      onConfirm: async () => {
        setConfirmModal({ show: false });
        try {
          setSubmitting(true);
          setSubmitMessage('');

          if (isEdit) {
            await updateVoucher(selectedVoucher.id, voucherData);
            setSubmitMessage('🎉 Cập nhật voucher thành công!');
          } else {
            await createVoucher(voucherData);
            setSubmitMessage('🎉 Thêm voucher mới thành công!');
          }

          fetchVouchers();
          setShowForm(false);
          setSelectedVoucher(null);
        } catch (err) {
          console.error('Lỗi khi lưu voucher:', err);
          setSubmitMessage('❌ Lưu voucher thất bại. Vui lòng thử lại.');
        } finally {
          setSubmitting(false);
          setTimeout(() => setSubmitMessage(''), 3000);
        }
      },
      onCancel: () => setConfirmModal({ show: false }),
    });
  };

  const handlePageChange = (newPage) => {
    const validatedPage = validatePage(newPage, totalPages);
    setCurrentPage(validatedPage);
    const newParams = new URLSearchParams(searchParams);
    newParams.set('page', validatedPage.toString());
    setSearchParams(newParams, { replace: true });
  };

  const handlePageSizeChange = (e) => {
    const newSize = parseInt(e.target.value);
    const validatedSize = validateSize(newSize);
    setPageSize(validatedSize);
    setCurrentPage(1);
    const newParams = new URLSearchParams(searchParams);
    newParams.set('size', validatedSize.toString());
    newParams.set('page', '1');
    setSearchParams(newParams, { replace: true });
  };

  const handleViewDetails = (voucher) => {
    setSelectedVoucher(voucher);
    setShowDetails(true);
  };

  const renderPagination = () => {
    const pages = [];
    const maxVisiblePages = 5;
    let startPage = Math.max(1, currentPage - Math.floor(maxVisiblePages / 2));
    let endPage = Math.min(totalPages, startPage + maxVisiblePages - 1);
    if (endPage - startPage + 1 < maxVisiblePages) {
      startPage = Math.max(1, endPage - maxVisiblePages + 1);
    }

    if (startPage > 1) {
      pages.push(<button key="1" onClick={() => handlePageChange(1)} className="px-3 py-1 rounded-lg hover:bg-gray-700">1</button>);
      if (startPage > 2) pages.push(<span key="start-ellipsis" className="px-3 py-1">...</span>);
    }

    for (let i = startPage; i <= endPage; i++) {
      pages.push(
        <button key={i} onClick={() => handlePageChange(i)} className={`px-3 py-1 rounded-lg ${i === currentPage ? 'bg-indigo-500 text-white' : 'hover:bg-gray-700'}`}>
          {i}
        </button>
      );
    }

    if (endPage < totalPages) {
      if (endPage < totalPages - 1) pages.push(<span key="end-ellipsis" className="px-3 py-1">...</span>);
      pages.push(<button key={totalPages} onClick={() => handlePageChange(totalPages)} className="px-3 py-1 rounded-lg hover:bg-gray-700">{totalPages}</button>);
    }

    return pages;
  };

  const columns = [
    { key: 'code', label: 'Mã Voucher' },
    { key: 'voucherName', label: 'Tên Voucher' },
    { key: 'discountValue', label: 'Giá Trị' },
    { key: 'startDate', label: 'Ngày Bắt Đầu' },
    { key: 'endDate', label: 'Ngày Kết Thúc' },
    { key: 'status', label: 'Trạng Thái' }
  ];

  if (loading) {
    return (
      <div className="flex-1 overflow-auto relative z-10">
        <Header title="Giảm Giá" />
        <main className="max-w-7xl mx-auto py-6 px-4 lg:px-8">
          <div className="flex items-center justify-center h-64">
            <div className="text-gray-400">Đang tải dữ liệu...</div>
          </div>
        </main>
      </div>
    );
  }

  return (
    <div className="flex-1 overflow-auto relative z-10">
      <Header title="Giảm Giá" />
      <main className="max-w-7xl mx-auto py-6 px-4 lg:px-8">
        {showError ? (
          <div className="flex items-center justify-center h-[calc(100vh-200px)]">
            <ErrorForm message={error} onRetry={fetchVouchers} />
          </div>
        ) : (
          <motion.div
            className="bg-gray-800 bg-opacity-50 backdrop-blur-md shadow-lg rounded-xl p-6 border border-gray-700 mb-8"
            initial={{ opacity: 0, y: 20 }}
            animate={{ opacity: 1, y: 0 }}
            transition={{ delay: 0.2 }}
          >
            <div className="flex justify-between items-center mb-6">
              <h2 className="text-xl font-semibold text-gray-100">Danh Sách Voucher</h2>
              <button
                onClick={() => {
                  setSelectedVoucher(null);
                  setShowForm(true);
                }}
                className="bg-indigo-500 hover:bg-indigo-600 text-white px-4 py-2 rounded-lg flex items-center gap-2 transition-colors"
              >
                Thêm Voucher
              </button>
            </div>

            <DataTable
              data={vouchers}
              columns={columns}
              onEdit={handleEdit}
              onDelete={handleDelete}
              onViewDetails={handleViewDetails}
              currentPage={currentPage}
              pageSize={pageSize}
            />

            <div className="mt-4 flex items-center justify-between border-t border-gray-700 pt-4">
              <div className="flex items-center gap-2">
                <span className="text-sm text-gray-400">Hiển thị</span>
                <select
                  value={pageSize}
                  onChange={handlePageSizeChange}
                  className="bg-gray-700 text-white rounded-lg px-2 py-1 text-sm"
                >
                  <option value={5}>5</option>
                  <option value={10}>10</option>
                  <option value={20}>20</option>
                  <option value={50}>50</option>
                </select>
                <span className="text-sm text-gray-400">trong tổng số {totalItems} bản ghi</span>
              </div>

              <div className="flex items-center gap-2">
                <button onClick={() => handlePageChange(currentPage - 1)} disabled={currentPage === 1} className="p-2 text-gray-400 hover:text-white disabled:opacity-50 disabled:cursor-not-allowed">
                  <ChevronLeft size={20} />
                </button>
                <div className="flex items-center gap-1">{renderPagination()}</div>
                <button onClick={() => handlePageChange(currentPage + 1)} disabled={currentPage === totalPages} className="p-2 text-gray-400 hover:text-white disabled:opacity-50 disabled:cursor-not-allowed">
                  <ChevronRight size={20} />
                </button>
              </div>
            </div>
          </motion.div>
        )}

        {submitting && (
          <div className="fixed inset-0 z-[99] bg-black bg-opacity-50 flex items-center justify-center">
            <div className="bg-gray-800 px-6 py-4 rounded-lg shadow-lg text-white text-center">
              <span className="animate-pulse">Đang xử lý...</span>
            </div>
          </div>
        )}

        {submitMessage && (
          <motion.div
            className="fixed top-5 right-5 z-[99] bg-gray-800 text-white px-4 py-2 rounded shadow-lg border border-gray-600"
            style={{ borderLeftWidth: 10 }}
            initial={{ opacity: 0, x: 50 }}
            animate={{ opacity: 1, x: 0 }}
            exit={{ opacity: 0, x: 50 }}
            transition={{ duration: 0.3 }}
          >
            {submitMessage}
          </motion.div>
        )}

        {showForm && (
          <VoucherForm
            voucher={selectedVoucher}
            onSubmit={handleSubmit}
            onCancel={() => {
              setShowForm(false);
              setSelectedVoucher(null);
            }}
          />
        )}

        {showDetails && selectedVoucher && (
          <VoucherDetailsForm
            voucher={selectedVoucher}
            onClose={() => setShowDetails(false)}
          />
        )}

        {confirmModal.show && (
          <ConfirmModal
            message={confirmModal.message}
            onConfirm={confirmModal.onConfirm}
            onCancel={confirmModal.onCancel}
          />
        )}
      </main>
    </div>
  );
};

export default VouchersPage;