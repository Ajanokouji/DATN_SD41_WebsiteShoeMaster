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
  userName: string;
  fullName: string;
  email: string;
  phoneNumber: string;
  avatar?: string;
  role: string;
  status: number;
  createdAt: string;
  updatedAt: string;
}

export interface UserFilterParams extends PaginationParams {
  userName?: string;
  password?: string;
}
