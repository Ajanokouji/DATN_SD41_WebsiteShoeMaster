import httpClient from "./agent";
import {FileItem , FileUploadResponse } from '@/types/file';
import { PaginatedResponse } from "@/types/common/pagination";
import { FileFilterRequest } from "@/types/file/file";
class FileApi {
  private static instance: FileApi;
  private readonly baseURL: string = "/files";

  private constructor() {}

  public static getInstance(): FileApi {
    if (!FileApi.instance) {
      FileApi.instance = new FileApi();
    }
    return FileApi.instance;
  }

  private handleApiError(error: any): Error {
    if (error.response) {
      
      // Server trả về response với status code nằm ngoài range 2xx
      const serverError = error.response?.message || 'Lỗi từ server';
      return new Error(serverError);
    } else if (error.request) {
      // Request được gửi nhưng không nhận được response
      return new Error('Không thể kết nối đến server');
    } else {
      // Có lỗi khi setting up request
      return new Error('Có lỗi xảy ra, vui lòng thử lại');
    }
  }

  // Get files with filter
  async getFiles(params?: FileFilterRequest): Promise<PaginatedResponse<FileItem>> {
    try {
      const defaultParams: FileFilterRequest = {
        currentPage: 1,
        pageSize: 20,
        sort: 'createdOnDate desc',
        searchAllApp: true
      };

      const requestParams = {
        ...defaultParams,
        ...params
      };

      const response = await httpClient.post<FileResponse>(
        `${this.baseURL}/filter`,
        requestParams
      );

      if (!response) {
        throw new Error('Không nhận được dữ liệu từ server');
      }

      return response;
    } catch (error) {
      throw this.handleApiError(error);
    }
  }

  // Upload file
  async uploadFile(file: File): Promise<string> {
    try {
      const formData = new FormData();
      formData.append('File', file);
      formData.append('FileName', file.name);

      const response = await httpClient.post<FileUploadResponse>(
        `${this.baseURL}/upload`,
        formData,
        {
          headers: {
            'Content-Type': 'multipart/form-data',
          },
        }
      );

      if (!response.data) {
        throw new Error('Không nhận được phản hồi khi tải file');
      }

      return response.data.data;
    } catch (error) {
      throw this.handleApiError(error);
    }
  }

  // Delete file
  async deleteFile(id: string): Promise<void> {
    try {
      await httpClient.delete(`${this.baseURL}/${id}`);
    } catch (error) {
      console.error('Delete file error:', error);
      throw error;
    }
  }

  // Rename file
  async renameFile(id: string, fileName: string): Promise<void> {
    try {
      await httpClient.put(`${this.baseURL}/${id}/rename`, { fileName });
    } catch (error) {
      console.error('Rename file error:', error);
      throw error;
    }
  }

  // Update file metadata
  async updateMetadata(id: string, metadata: any): Promise<void> {
    try {
      await httpClient.put(`${this.baseURL}/${id}/metadata`, metadata);
    } catch (error) {
      console.error('Update metadata error:', error);
      throw error;
    }
  }

  // Move file
  async moveFile(id: string, newPath: string): Promise<void> {
    try {
      await httpClient.put(`${this.baseURL}/${id}/move`, { newPath });
    } catch (error) {
      console.error('Move file error:', error);
      throw error;
    }
  }

  // Get file URL
  async getFileUrl(id: string): Promise<string> {
    try {
      const response = await httpClient.get(`${this.baseURL}/${id}/url`);
      return response.data || '';
    } catch (error) {
      console.error('Get file URL error:', error);
      throw error;
    }
  }
}

const fileApi = FileApi.getInstance();
export default fileApi; 