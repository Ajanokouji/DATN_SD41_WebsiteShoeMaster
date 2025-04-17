import axios from 'axios';

// Sử dụng HTTP trong development, HTTPS trong production
const API_BASE_URL = 'https://localhost:7293/api';

// Cấu hình axios mặc định
axios.defaults.withCredentials = true;
axios.defaults.headers.common['Content-Type'] = 'application/json';
axios.defaults.headers.common['Accept'] = 'application/json';

// 1. Lấy danh sách voucher với điều kiện lọc
export const getVouchers = async (queryModel) => {
  try {
    // Tạo URL với query parameters
    const url = new URL(`${API_BASE_URL}/voucher/filter`);
    url.searchParams.append('currentPage', queryModel.CurrentPage || 1);
    url.searchParams.append('size', queryModel.PageSize || 10);
    url.searchParams.append('pageSize', queryModel.PageSize || 10);
    
    // Tạo request body với các tham số còn lại{}
    const requestBody = {
      ten_giam_gia: queryModel.ten_giam_gia || undefined,
      id_giam_gia: queryModel.id_giam_gia || undefined,
      loai_giam_gia: queryModel.loai_giam_gia || undefined,
      thoi_gian_bat_dau: queryModel.thoi_gian_bat_dau || undefined,
      thoi_gian_ket_thuc: queryModel.thoi_gian_ket_thuc || undefined,
      trang_thai: queryModel.trang_thai || undefined,
      create_on_date: queryModel.create_on_date || undefined,
      last_modifi_on_date: queryModel.last_modifi_on_date || undefined
    };
    
    const response = await axios.post(url.toString(), requestBody);
    
    // Kiểm tra và xử lý response
    if (response.data && response.data.data) {
      return {
        data: response.data.data
      };
    } else {
      return {
        data: {
          content: [],
          currentPage: queryModel.CurrentPage || 1,
          pageSize: queryModel.PageSize || 10,
          totalPages: 1,
          totalRecords: 0
        }
      };
    }
  } catch (error) {
    console.error('Error fetching vouchers:', error);
    throw error;
  }
};

// 2. Lấy số lượng voucher theo điều kiện
export const getVoucherCount = async (queryParams = {}) => {
  try {
    const response = await axios.post(`${API_BASE_URL}/voucher/count`, null, {
      params: queryParams
    });
    return response.data;
  } catch (error) {
    console.error('Error counting vouchers:', error);
    throw error;
  }
};

// 4. Tạo mới voucher
export const createVoucher = async (voucher) => {
  try {
    voucher.code = voucher.code.trim();
    voucher.voucherName = voucher.voucherName.trim();
    voucher.description = voucher.description.trim();

    const response = await axios.post(`${API_BASE_URL}/voucher`, voucher);
    return response.data;
  } catch (error) {
    console.error('Error creating voucher:', error);
    throw error;
  }
};

// 5. Cập nhật voucher (PATCH - chỉ cập nhật các trường được gửi)
export const updateVoucher = async (id, voucher) => {
  try {
    voucher.code = voucher.code.trim();
    voucher.voucherName = voucher.voucherName.trim();
    voucher.description = voucher.description.trim();

    const response = await axios.patch(`${API_BASE_URL}/voucher/${id}`, voucher);
    return response.data;
  } catch (error) {
    console.error(`Error updating voucher with id ${id}:`, error);
    throw error;
  }
};

// 6. Xóa một voucher
export const deleteVoucher = async (id) => {
  try {
    const response = await axios.delete(`${API_BASE_URL}/voucher/${id}`);
    return response.data;
  } catch (error) {
    console.error(`Error deleting voucher with id ${id}:`, error);
    throw error;
  }
};

// 7. Xóa nhiều voucher
export const deleteVouchers = async (ids) => {
  try {
    const response = await axios.delete(`${API_BASE_URL}/voucher`, {
      data: ids
    });
    return response.data;
  } catch (error) {
    console.error('Error deleting vouchers:', error);
    throw error;
  }
};

// 8. Check Voucher Code đã tồn tại chưa
export const isVoucherCodeExist = async (code, ids) => {
  try {
    const url = new URL(`${API_BASE_URL}/voucher/iscodeexist`);
    const params = new URLSearchParams();
    if (ids) {
      params.append('voucherId', ids);
    }

    const response = await axios.post(url.toString(), code, {
      params: params,
      headers: {
        'Content-Type': 'application/json'
      }
    });
    return response.data;
  } catch (error) {
    console.error('Error check voucher code exist?:', error);
    throw error;
  }
};