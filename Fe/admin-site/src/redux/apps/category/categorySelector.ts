import { RootState } from "@/redux/store";

export const selectProducts = (state: RootState) => state.category.products;
export const selectPagination = (state: RootState) => state.category.pagination;
