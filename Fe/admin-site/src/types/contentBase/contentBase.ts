// src/types/content/contentBase.ts

import { PaginationParams } from "../common/pagination";

export interface ContentBaseReqDto {
  title: string;
  seoUri: string;
  seoTitle: string;
  seoDescription: string;
  seoKeywords: string;
  publishStartDate: string; // ISO Date string
  publishEndDate: string;   // ISO Date string
  isPublish: boolean;
  content: string;
  createdByUserId: string;
  lastModifiedByUserId: string;
  lastModifiedOnDate: string;
  createdOnDate: string;
  isdeleted: boolean;
}

export interface ContentBaseResDto {
  id: string;
  title: string;
  seoUri: string;
  seoTitle: string;
  seoDescription: string;
  seoKeywords: string;
  publishStartDate: string;
  publishEndDate: string;
  isPublish: boolean;
  content: string;
  createdByUserId: string;
  lastModifiedByUserId: string;
  lastModifiedOnDate: string;
  createdOnDate: string;
  isdeleted: boolean;
}

export interface ContentBaseFilterParams extends PaginationParams {
  title?: string;
  seoUri?: string;
  isDeleted?: boolean; // Added the missing property

}