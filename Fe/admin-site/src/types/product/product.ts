export default interface ProductReqDto {
  code: string;
  name: string;
  description: string;
  status: string;
  imageUrl: string;
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
export interface ProductResDto {
  id: string;
  code: string;
  name: string;
  status: string;
  imageUrl: string;
  sortOrder: number | null;
  description: string | null;
  mainCategoryId: string;
  createdOnDate: string;
  lastModifiedOnDate: string;
  isdeleted: boolean;
}
