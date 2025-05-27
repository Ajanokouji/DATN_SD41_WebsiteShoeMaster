export interface PaginatedResponse<T> {
  content: T[];
  numberOfRecords: number;
  totalPages: number;
  currentPage: number;
  pageSize: number;
} 