import { RootState } from "@/redux/store";

export const selectVoucherProducts = (state: RootState) => state.voucherProduct.voucherProducts;
export const selectVoucherProductPagination = (state: RootState) => state.voucherProduct.voucherProductPagination;
export const selectVoucherProduct = (state: RootState) => state.voucherProduct.voucherProduct;

export const selectProducts = (state: RootState) => state.voucherProduct.products;
export const selectProductPagination = (state: RootState) => state.voucherProduct.productPagination;