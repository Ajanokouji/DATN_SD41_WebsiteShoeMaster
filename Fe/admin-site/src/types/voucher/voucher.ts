import { PaginationParams } from "../common/pagination";

export default interface VoucherReqDto {
  voucherName: string;
  voucherType: number;
  startDate: string;
  endDate: string;
  createdByUserId: string;
  lastModifiedByUserId: string;
  lastModifiedOnDate: string;
  createdOnDate: string;
  status: number;
  isdeleted: boolean;
  code: string; // Mã voucher
  discountAmount: number | null; // Số tiền giảm giá
  discountPercentage: number | null; // Phần trăm giảm giá
  description: string | null; // Mô tả voucher
  minimumOrderAmount: number | null; // Giá trị đơn hàng tối thiểu
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
  code: string; // Mã voucher
  discountAmount: number | null; // Số tiền giảm giá
  discountPercentage: number | null; // Phần trăm giảm giá
  description: string | null; // Mô tả voucher
  minimumOrderAmount: number | null; // Giá trị đơn hàng tối thiểu
}

export interface VoucherFilterParams extends PaginationParams {
  ten_giam_gia?: string;
}
