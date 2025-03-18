import { RootState } from "@/redux/store";

export const selectCustomers = (state: RootState) => state.customer.customers;
export const selectPagination = (state: RootState) => state.customer.pagination;

export const selectCustomer = (state: RootState) => state.customer.customer;
