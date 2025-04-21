export interface FileItem {
  id: string;
  fileName: string;
  filePath: string;
  completeFilePath: string;
  contentType: string;
  fileSize: number;
  uploadedBy: string;
  isdeleted: boolean;
  createdByUserId: string | null;
  lastModifiedByUserId: string | null;
  lastModifiedOnDate: string;
  createdOnDate: string;
}

export interface FileUploadResponse {
  data: string;
} 