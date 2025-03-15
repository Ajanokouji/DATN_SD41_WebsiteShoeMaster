import { PaginationParams } from "../common/pagination";

export default interface RelationReqDto {
  code: string;
  name: string;
  description: string;
  type: string;
  completeCode: string;
  completeName: string;
  completePath: string;
  parentPath: string;
  metadataObj: MetadataObj[];
  createdByUserId: string;
  lastModifiedByUserId: string;
  lastModifiedOnDate: string;
  createdOnDate: string;
  sortOrder: number;
}

export interface MetadataObj {
  fieldName: string;
  fieldDisplayName: string;
  fieldType: number;
  fieldValues: string;
  fieldValueTexts: string;
  fieldValueType: string;
  fieldSelectionValues: FieldSelectionValues[];
}

export interface FieldSelectionValues {
  key: string;
  code: string;
  value: string;
  order: number;
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

export interface RelationDetailResDto {
  id: string;
  code: string;
  name: string;
  description: string;
  type: string;
  completeCode: string;
  completeName: string;
  completePath: string;
  parentPath: string;
  metadataObj: MetadataObj[];
  createdByUserId: string;
  lastModifiedByUserId: string;
  lastModifiedOnDate: string;
  createdOnDate: string;
  sortOrder: number;
}

export interface RelationFilterParams extends PaginationParams {
  IdDanhMuc?: string;
  IdSanPham?: string;
  TenSanPham?: string;
  TenDanhMuc?: string;
  status?: string;
  name?: string;
}
