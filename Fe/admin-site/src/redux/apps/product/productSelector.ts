import { RootState } from "@/redux/store";

export const selectProducts = (state: RootState) => state.product.products;
export const selectPagination = (state: RootState) => state.product.pagination;

export const selectProduct = (state: RootState) => state.product.product;
