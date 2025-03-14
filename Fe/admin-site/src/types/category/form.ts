export interface FieldSelectionValue {
  key: string;
  code: string;
  value: string;
  order: number;
  fieldName?: string;
}

export interface FormData {
  name: string;
  description: string;
  parentId: string;
  sortOrder: number;
  type: string;
  code: string;
  completeCode: string;
  completeName: string;
  completePath: string;
  // eslint-disable-next-line @typescript-eslint/no-explicit-any
  [key: string]: any;
}
