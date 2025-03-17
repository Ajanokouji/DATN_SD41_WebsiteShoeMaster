// src/types/customer/customer.ts

import { PaginationParams } from "../common/pagination";

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
    createdOnDate: string;     // Thời gian tạo khách hàng
    updatedAt: string;     // Thời gian cập nhật thông tin khách hàng
}

// DTO trả về từ server khi lấy chi tiết thông tin khách hàng
export interface CustomerDetailResDto extends CustomerResDto {
    ordersCount: number;   // Số lượng đơn hàng của khách hàng (ví dụ thông tin bổ sung)
}

export interface CustomerFilterParams extends PaginationParams {
  name?: string;
}
