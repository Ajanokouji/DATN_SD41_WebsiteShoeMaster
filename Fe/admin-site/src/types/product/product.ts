export interface ProductResDto {
  id: string;
  code: string;
  name: string;
  status: string;
  imageUrl: string;
  sortOrder: number | null;
  description: string | null;
  mainCategoryId: string;
  metadataObj: any[];
  createdOnDate: string;
  lastModifiedOnDate: string;
  isdeleted: boolean;
}

export default interface ProductReqDto {
  code: string;
  name: string;
  description: string;
  metadataObj: any[];
  createdByUserId: string;
  lastModifiedByUserId: string;
  lastModifiedDate: string;
  createdOnDate: string;
  sortOrder: number;
}
