import { PaginationParams } from "../common/pagination";

export default interface VoucherReqDto {
  id: string;
  voucherName: string;
  voucherType: number;
  startDate: string;
  endDate: string;
  createdByUserId: string;
  lastModifiedByUserId: string;
  lastModifiedOnDate: string;
  createdOnDate: string;
  status: number;
  isDeleted: boolean;
  code: string; // Mã voucher
  discountAmount: number | null; // Số tiền giảm giá
  discountPercentage: number | null; // Phần trăm giảm giá
  description: string | null; // Mô tả voucher
  minimumOrderAmount: number | null; // Giá trị đơn hàng tối thiểu
  totalMaxUsage: number | null; //Tổng lượt sử dụng tối đa
  maxUsagePerCustomer: number | null; //Số lượt sử dụng tối đa trên mỗi khách hàng
  maxDiscountAmount: number | null; //Số tiền giảm giá tối đa đối với voucher giảm theo phần trăm
  redeemCount: number | null; //Tổng Số lượt đã sử dụng voucher
  imageUrl: string | null; // Đường dẫn đến hình ảnh của voucher
}

export interface VoucherResDto {
  id: string;
  voucherName: string;
  status: number | null;
  voucherType: number | null;
  createdOnDate: string;
  lastModifiedOnDate: string;
  createdByUserId: string;
  lastModifiedByUserId: string;
  startDate: string;
  endDate: string;
  isDeleted: boolean;
  code: string; // Mã voucher
  discountAmount: number | null; // Số tiền giảm giá
  discountPercentage: number | null; // Phần trăm giảm giá
  description: string | null; // Mô tả voucher
  minimumOrderAmount: number | null; // Giá trị đơn hàng tối thiểu
  totalMaxUsage: number | null; //Tổng lượt sử dụng tối đa
  maxUsagePerCustomer: number | null; //Số lượt sử dụng tối đa trên mỗi khách hàng
  maxDiscountAmount: number | null; //Số tiền giảm giá tối đa đối với voucher giảm theo phần trăm
  redeemCount: number | null; //Tổng Số lượt đã sử dụng voucher
  imageUrl: string | null; // Đường dẫn đến hình ảnh của voucher
}

export interface VoucherFilterParams extends PaginationParams {
  ten_giam_gia?: string;
  statusTotal?: number; //Trạng thái tổng: Dựa vào EndDate StartDate và Status để quyết định
}
