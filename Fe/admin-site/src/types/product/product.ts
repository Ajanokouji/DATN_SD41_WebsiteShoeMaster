import { PaginationParams } from "../common/pagination";

export default interface ProductReqDto {
  code: string;
  name: string;
  description: string;
  status: string;
  imageUrl: string;
  mediaObjs: string[];
  createdByUserId: string;
  lastModifiedByUserId: string;
  lastModifiedDate: string;
  createdOnDate: string;
  publicOnDate: string;
  sortOrder: string;
  workFlowStates: string;
  completeName: string;
  completePath: string;
  completeCode: string;
  mainCategoryId: string;
  metadataObj: MetadataObj[];
  labelsObjs: LabelsObjs[];
  variantObjs: VariantObjs[];
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
export interface LabelsObjs {
  objectId: string;
  objectCode: string;
  objectName: string;
  color: string;
}
export interface VariantObjs {
  group1: string,
  group2: string,
  price: string,
  stock: number,
  sku: string,
  imgUrl :string
}
export interface ProductResDto {
  id: string;
  code: string;
  name: string;
  status: string;
  imageUrl: string;
  sortOrder: string;
  description: string | null;
  mainCategoryId: string;
  createdOnDate: string;
  lastModifiedOnDate: string;
}

export interface ProductDetailResDto extends ProductResDto {
  mediaObjs: string[];
  createdByUserId: string;
  lastModifiedByUserId: string;
  lastModifiedDate: string;
  publicOnDate: string;
  workFlowStates: string;
  completeName: string;
  completePath: string;
  completeCode: string;
  mainCategoryId: string;
  metadataObj: MetadataObj[];
  labelsObjs: LabelsObjs[];
  variantObjs: VariantObjs[];
}

export interface ProductFilterParams extends PaginationParams {
  tenSanPham?: string;
  maSanPham?: string;
  status?: string;
  description?: string;
}
