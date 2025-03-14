export default interface CategoryReqDto {
  code: string;
  name: string;
  description: string;
  type: string;
  completeCode: string;
  completeName:string;
  completePath:string;
  parentPath:string;
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

export interface CategoryResDto {
  id: string;
  code: string;
  name: string;
  sortOrder: number | null;
  description: string | null;
  mainCategoryId: string;
  createdOnDate: string;
  lastModifiedOnDate: string;
  isdeleted: boolean;
}
