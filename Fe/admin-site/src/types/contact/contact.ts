import { PaginationParams } from "../common/pagination";

export default interface ContactReqDto {
  name: string;
  fullName: string;
  address?: string;
  dateOfBirth: string;
  imageUrl: string;
  email?: string;
  phoneNumber?: string;
  content?: string;
  createdByUserId: string;
  lastModifiedByUserId: string;
  lastModifiedOnDate: string;
  createdOnDate: string;
  isdeleted: boolean;
}

export interface ContactResDto {
  id: string;
  name: string;
  fullName: string;
  address?: string;
  dateOfBirth: string;
  imageUrl: string;
  email?: string;
  phoneNumber?: string;
  content?: string;
  createdOnDate: string;
  lastModifiedOnDate: string;
  createdByUserId: string;
  lastModifiedByUserId: string;
}

export interface ContactFilterParams extends PaginationParams {
  name?: string;
  fullName?: string;
}
