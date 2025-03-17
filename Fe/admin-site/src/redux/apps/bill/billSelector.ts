import { RootState } from "@/redux/store";

export const selectBills = (state: RootState) => state.bill.bills;
export const selectPagination = (state: RootState) => state.bill.pagination;

export const selectBill = (state: RootState) => state.bill.bill;
