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

export interface PaginationProps {
  currentPage: number;
  totalPages: number;
  pageSize: number;
  totalRecords: number;
  onPageChange: (page: number) => void;
  onPageSizeChange: (size: number) => void;
}

export interface FilterItem {
  key: string;
  value: string;
  label: string;
}
