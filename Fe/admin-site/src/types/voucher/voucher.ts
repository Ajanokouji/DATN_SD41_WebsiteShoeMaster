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
}

export interface VoucherFilterParams extends PaginationParams {
  ten_giam_gia?: string;
}
