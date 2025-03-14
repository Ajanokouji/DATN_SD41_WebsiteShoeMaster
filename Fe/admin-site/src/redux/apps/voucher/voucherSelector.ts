import { RootState } from "@/redux/store";

export const selectVouchers = (state: RootState) => state.voucher.vouchers;
export const selectPagination = (state: RootState) => state.voucher.pagination;

export const selectVoucher = (state: RootState) => state.voucher.voucher;
