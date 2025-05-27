import { RootState } from "@/redux/store";

export const selectUsers = (state: RootState) => state.user.users;
export const selectPagination = (state: RootState) => state.user.pagination;
export const selectUser = (state: RootState) => state.user.user;
