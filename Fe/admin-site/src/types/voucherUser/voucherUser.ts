import { PaginationParams } from "../common/pagination";

export default interface VoucherUserReqDto {
  voucherId: string;
  userId: string;
  isUsed: boolean;
  createdByUserId: string;
  lastModifiedByUserId: string;
  lastModifiedOnDate: string;
  createdOnDate: string;
}

export interface VoucherUserResDto {
  id: string;
  voucherId: string;
  userId: string;
  isUsed: boolean;
  createdByUserId: string;
  lastModifiedByUserId: string;
  lastModifiedOnDate: string;
  createdOnDate: string;
}

export interface VoucherUserFilterParams extends PaginationParams {
  voucherId?: string;
  userId?: string;
  isUsed?: boolean;
  lastModifiedOnDate?: string;
  createdOnDate?: string;
}


//User for search
export interface UserResDtoForSearch {
  id: string;
  type: number;
  username?: string;
  name?: string;
  phoneNumber?: string;
  address?: string;
  email?: string;
  avartarUrl?: string;
  isActive?: boolean;
  createdByUserId: string;
  lastModifiedByUserId: string;
  lastModifiedOnDate: string;
  createdOnDate: string;
  isDeleted: boolean;
}

export interface UserFilterParamsForSearch extends PaginationParams {
  userName?: string;
}