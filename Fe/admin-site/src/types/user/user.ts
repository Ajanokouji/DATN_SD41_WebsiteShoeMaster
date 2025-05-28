import { PaginationParams } from "../common/pagination";

export default interface UserReqDto {
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
  username: string | null;
  name: string | null;
  type: number;
  password: string | null;
  phoneNumber: string | null;
  email: string | null;
  address: string | null;
  avartarUrl: string | null;
  userDetailJson: string | null;
  isActive: boolean;
  createdByUserId: string;
  lastModifiedByUserId: string;
  lastModifiedOnDate: string;
  createdOnDate: string;
  isdeleted: boolean;
}

export interface UserFilterParams extends PaginationParams {
  username?: string;
  password?: string;
  type?: number;
}
