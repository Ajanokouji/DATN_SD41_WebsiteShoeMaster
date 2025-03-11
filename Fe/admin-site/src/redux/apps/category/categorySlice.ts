import { createSlice } from "@reduxjs/toolkit";
import { addLoadingCases } from "@/utils/redux.utils";
import { createAppThunk } from "@/utils/createThunk";
import { CATEGORY_MESSAGES } from "@/constants/category.constants";
import CategoryReqDto from "@/types/category/category";
import categoryService from "@/redux/api/categoryApi";

export interface InitState {
  loading: boolean;
  error: string | null;
  category: CategoryReqDto | null;
}

const initialState: InitState = {
  loading: false,
  error: null,
  category: null,
};

export const createCategory = createAppThunk(
  "category/create",
  categoryService.createCategoryReq,
  {
    successMessage: CATEGORY_MESSAGES.CREATE_CATEGORY.SUCCESS,
    errorMessage: CATEGORY_MESSAGES.CREATE_CATEGORY.ERROR,
  }
);


const categorySlice = createSlice({
  name: "category",
  initialState,
  reducers: {},
  extraReducers: (builder) => {
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
