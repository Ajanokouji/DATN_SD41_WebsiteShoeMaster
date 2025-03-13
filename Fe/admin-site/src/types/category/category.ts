export default interface CategoryReqDto {
  code: string;
  name: string;
  description: string;
  metadataObj: MetadataObj[];
  createdByUserId: string;
  lastModifiedByUserId: string;
  lastModifiedDate: string;
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
  key: number;
  code: string;
  value: string;
  order: number;
}

export interface CategoryResDto {
  id: string;
  code: string;
  name: string;
  sortOrder: number | null;
  description: string | null;
  mainCategoryId: string;
  metadataObj: any[];
  createdOnDate: string;
  lastModifiedOnDate: string;
  isdeleted: boolean;
}
