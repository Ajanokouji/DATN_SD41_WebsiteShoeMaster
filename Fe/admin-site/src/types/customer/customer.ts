// src/types/customer/customer.ts

// DTO gửi lên server khi tạo mới hoặc cập nhật khách hàng
export interface CustomerReqDto {
    id?: string;            // ID khách hàng (chỉ sử dụng khi cập nhật thông tin khách hàng)
    name: string;          // Tên khách hàng
    email: string;         // Email khách hàng
    phoneNumber: string;         // Số điện thoại khách hàng
    address: string;       // Địa chỉ khách hàng
    code?: string;         // Mã khách hàng (chỉ sử dụng khi cập nhật thông tin khách hàng)
    TTLHRelatedIds: string[]; // Mặc định giá trị
    ttthidMain?: string; // Mặc định giá trị
}

// DTO trả về từ server khi lấy danh sách khách hàng hoặc thông tin chi tiết khách hàng
export interface CustomerResDto {
    id: string;            // ID khách hàng
    code: string;          // Mã khách hàng
    name: string;          // Tên khách hàng
    email: string;         // Email khách hàng
    status: string;        // Trạng thái khách hàng
    phoneNumber: string;   // Số điện thoại khách hàng
    description: string;   // Mô tả khách hàng
    address: string;       // Địa chỉ khách hàng
    createdAt: string;     // Thời gian tạo khách hàng
    updatedAt: string;     // Thời gian cập nhật thông tin khách hàng
}

// DTO trả về từ server khi lấy chi tiết thông tin khách hàng
export interface CustomerDetailResDto extends CustomerResDto {
    ordersCount: number;   // Số lượng đơn hàng của khách hàng (ví dụ thông tin bổ sung)
}

export interface PaginationParams {
    currentPage: number; // Trang hiện tại
    pageSize: number;    // Số lượng bản ghi mỗi trang
    search?: string;     // Từ khóa tìm kiếm (nếu có)
}

// DTO trả về từ server cho phân trang (áp dụng cho tất cả các API trả về danh sách khách hàng)
export interface PaginatedResponse<T> {
    data: T[];            // Mảng các đối tượng dữ liệu
    totalRecords: number; // Tổng số bản ghi (số lượng khách hàng)
    currentPage: number;  // Trang hiện tại
    totalPages: number;   // Tổng số trang (tính toán từ totalRecords và pageSize)
    pageSize: number;     // Kích thước trang (số lượng bản ghi mỗi trang)
}