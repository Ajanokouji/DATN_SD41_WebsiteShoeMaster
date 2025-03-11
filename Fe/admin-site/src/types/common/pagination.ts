export interface PaginatedResponse<T> {
  data: {
    currentPage: number;
    totalPages: number;
    pageSize: number;
    numberOfRecords: number;
    totalRecords: number;
    content: T[];
  };
}

export interface PaginationParams {
  CurrentPage: number;
  PageSize: number;
  search?: string;
}
