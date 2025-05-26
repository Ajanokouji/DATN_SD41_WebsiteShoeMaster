import { PaginationParams } from "../common/pagination";

export interface UserReqDto {
  username?: string;
  password?: string;
  name?: string;
  email?: string;
  phoneNumber?: string;
  address?: string;
  isActive?: boolean;
}

export interface UserResDto {
  id: string;
  userName: string | null;
  name: string | null;
  type: string;
  phoneNumber: string | null;
  email: string | null;
  address: string | null;
  avatar: string | null;
  userDetailJson: string | null;
  isActive: boolean;
  createdByUserId: string;
  lastModifiedByUserId: string;
  lastModifiedOnDate: string;
  createdOnDate: string;
  isdeleted: boolean;
}

export interface UserFilterParams extends PaginationParams {
  userName?: string;
  password?: string;
}
