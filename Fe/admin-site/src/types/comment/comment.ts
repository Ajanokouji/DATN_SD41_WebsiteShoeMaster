import { PaginationParams } from "../common/pagination";

export default interface CommentReqDto {
  objectId: string | null;
  ref: string | null;
  message: string;
  status: number;
  userId: string | null;
  username: string | null;
  isPublish: boolean;
  parantId: string | null;
  tolalReply: number;
  createdByUserId: string;
  lastModifiedByUserId: string;
  lastModifiedOnDate: string;
  createdOnDate: string;
  isdeleted: boolean;
}

export interface CommentResDto {
  id: string;
  objectId: string | null;
  ref: string | null;
  message: string;
  status: number;
  userId: string | null;
  username: string | null;
  isPublish: boolean;
  parantId: string | null;
  tolalReply: number;
  createdByUserId: string;
  lastModifiedByUserId: string;
  lastModifiedOnDate: string;
  createdOnDate: string;
  isdeleted: boolean;
}

export interface CommentFilterParams extends PaginationParams {
  objectId?: string;
}
