import { RootState } from "@/redux/store";

export const selectCategories = (state: RootState) => state.category.categories;
export const selectPagination = (state: RootState) => state.category.pagination;

export const selectCategory = (state: RootState) => state.category.category;
