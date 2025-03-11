import { createSlice, PayloadAction } from "@reduxjs/toolkit";
import { addLoadingCases } from "@/utils/redux.utils";
import { createAppThunk } from "@/utils/createThunk";
import { CATEGORY_MESSAGES } from "@/constants/category.constants";
import CategoryReqDto, { Product } from "@/types/category/category";
import categoryService from "@/redux/api/categoryApi";
import { PaginationParams } from "@/types/common/pagination";

export interface InitState {
  loading: boolean;
  error: string | null;
  category: CategoryReqDto | null;
  products: Product[];
  pagination: {
    currentPage: number;
    totalPages: number;
    pageSize: number;
    totalRecords: number;
  };
}

const initialState: InitState = {
  loading: false,
  error: null,
  category: null,
  products: [],
  pagination: {
    currentPage: 1,
    totalPages: 0,
    pageSize: 20,
    totalRecords: 0,
  },
};

export const createCategory = createAppThunk(
  "category/create",
  categoryService.createCategoryReq,
  {
    successMessage: CATEGORY_MESSAGES.CREATE_CATEGORY.SUCCESS,
    errorMessage: CATEGORY_MESSAGES.CREATE_CATEGORY.ERROR,
  }
);

export const fetchProducts = createAppThunk(
  "products/fetch",
  async (params: PaginationParams) => {
    const response = await categoryService.getProducts(params);
    return response;
  }
);


const categorySlice = createSlice({
  name: "category",
  initialState,
  reducers: {
    setPage: (state, action: PayloadAction<number>) => {
      state.pagination.currentPage = action.payload;
    },
    setPageSize: (state, action: PayloadAction<number>) => {
      state.pagination.pageSize = action.payload;
    }
  },
  extraReducers: (builder) => {
    addLoadingCases(builder, fetchProducts, {
      onFulfilled: (state, action) => {
        state.loading = false;
        state.products = action.payload.data.content;
        state.pagination = {
          currentPage: action.payload.data.currentPage,
          totalPages: action.payload.data.totalPages,
          pageSize: action.payload.data.pageSize,
          totalRecords: action.payload.data.totalRecords,
        };
      },
    });
    
    // Create category
    addLoadingCases(builder, createCategory, {
      onFulfilled: (state, action) => {
        state.loading = false;
        state.category = action?.payload ?? null;
      },
    });
    
  },
});

export default categorySlice.reducer;
