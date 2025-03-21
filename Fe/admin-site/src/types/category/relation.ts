import { PaginationParams } from "../common/pagination";

export default interface RelationReqDto {
  idProduct: string;
  categoriesId: string;
  productName: string;
  categoryName: string;
  relationType: string;
  status: string;
  description: string;
  createdByUserId: string;
  lastModifiedByUserId: string;
  lastModifiedOnDate: string;
  createdOnDate: string;
  order: number;
  isdeleted: boolean;
  isPublish: boolean;
  relatedObj?: RelatedObj[];
}

export interface RelatedObj {
  objectCode: string;
  objectName: string;
  objectType: number;
  objectKeyword: string;
  objectContent: string;
}

export interface RelationResDto {
  id: string;
  idProduct: string;
  categoriesId: string;
  productName: string;
  categoryName: string;
  order: number | null;
  description: string | null;
  createdOnDate: string;
  lastModifiedOnDate: string;
}

export interface RelationFilterParams extends PaginationParams {
  IdDanhMuc?: string;
  IdSanPham?: string;
  TenSanPham?: string;
  TenDanhMuc?: string;
  status?: string;
  name?: string;
}
