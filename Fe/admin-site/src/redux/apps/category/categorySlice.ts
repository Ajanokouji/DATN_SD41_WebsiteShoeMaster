import { createSlice, PayloadAction } from "@reduxjs/toolkit";
import { addLoadingCases } from "@/utils/redux.utils";
import { createAppThunk } from "@/utils/createThunk";
import { CATEGORY_MESSAGES } from "@/constants/category.constants";
import CategoryReqDto, {
  CategoryDetailResDto,
  CategoryFilterParams,
  CategoryResDto,
} from "@/types/category/category";
import categoryService from "@/redux/api/categoryApi";

export interface InitState {
  loading: boolean;
  error: string | null;
  category: CategoryDetailResDto | null;
  categories: CategoryResDto[];
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
  categories: [],
  pagination: {
    currentPage: 1,
    totalPages: 0,
    pageSize: 20,
    totalRecords: 0,
  },
};

export const fetchCategories = createAppThunk(
  "categories/fetch",
  async (params: CategoryFilterParams) => {
    const response = await categoryService.getCategories(params);
    return response;
  }
);

export const fetchCategoryById = createAppThunk(
  "category/fetch",
  async (id: string) => {
    const response = await categoryService.getCategoryById(id);
    return response;
  }
);

export const createCategory = createAppThunk(
  "category/create",
  categoryService.createCategoryReq,
  {
    successMessage: CATEGORY_MESSAGES.CREATE_CATEGORY.SUCCESS,
    errorMessage: CATEGORY_MESSAGES.CREATE_CATEGORY.ERROR,
  }
);

export const updateCategory = createAppThunk(
  "category/update",
  async ({ id, data }: { id: string; data: Partial<CategoryReqDto> }) => {
    const response = await categoryService.updateCategoryReq(id, data);
    return response;
  },
  {
    successMessage: CATEGORY_MESSAGES.UPDATE_CATEGORY.SUCCESS,
    errorMessage: CATEGORY_MESSAGES.UPDATE_CATEGORY.ERROR,
  }
);

export const deleteCategory = createAppThunk(
  "category/delete",
  async (id: string) => {
    await categoryService.deleteCategoryReq(id);
    return id;
  },
  {
    successMessage: CATEGORY_MESSAGES.DELETE_CATEGORY.SUCCESS,
    errorMessage: CATEGORY_MESSAGES.DELETE_CATEGORY.ERROR,
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
    },
  },
  extraReducers: (builder) => {
    addLoadingCases(builder, fetchCategories, {
      onFulfilled: (state, action) => {
        state.loading = false;
        state.categories = action.payload.data.content;
        state.pagination = {
          currentPage: action.payload.data.currentPage,
          totalPages: action.payload.data.totalPages,
          pageSize: action.payload.data.pageSize,
          totalRecords: action.payload.data.totalRecords,
        };
      },
    });

    // Fetch by ID
    addLoadingCases(builder, fetchCategoryById, {
      onFulfilled: (state, action) => {
        state.loading = false;
        state.category = action.payload;
      },
    });

    // Create category
    addLoadingCases(builder, createCategory, {
      onFulfilled: (state, action) => {
        state.loading = false;
        // state.category = action?.payload ?? null;
        state.categories = [action.payload, ...state.categories];
      },
    });

    // Update category
    addLoadingCases(builder, updateCategory, {
      onFulfilled: (state, action) => {
        state.loading = false;
        state.categories = state.categories.map((category) =>
          category.id === action.payload.id ? action.payload : category
        );
      },
    });

    // Delete category
    addLoadingCases(builder, deleteCategory, {
      onFulfilled: (state, action) => {
        state.loading = false;
        state.categories = state.categories.filter(
          (category) => category.id !== action.payload
        );
      },
    });
  },
});

export const { setPage, setPageSize } = categorySlice.actions;
export default categorySlice.reducer;
