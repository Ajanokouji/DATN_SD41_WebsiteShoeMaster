import { PaginationParams } from "../common/pagination";

export default interface VoucherProductReqDto {
  voucherId: string;
  productId: string;
  varientProductId: string;
  createdByUserId: string;
  lastModifiedByUserId: string;
  lastModifiedOnDate: string;
  createdOnDate: string;
}

export interface VoucherProductResDto {
    id: string;
    voucherId: string;
    productId: string;
    varientProductId: string;
    createdByUserId: string;
    lastModifiedByUserId: string;
    lastModifiedOnDate: string;
    createdOnDate: string;
}

export interface VoucherProductFilterParams extends PaginationParams {
  voucherId?: string;
  productId?: string;
  varientProductId?: string;
  lastModifiedOnDate?: string;
  createdOnDate?: string;
}