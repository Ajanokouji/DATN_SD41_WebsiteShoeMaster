import { RootState } from "@/redux/store";

export const selectVoucherUsers = (state: RootState) => state.voucherUser.voucherUsers;
export const selectVoucherUserPagination = (state: RootState) => state.voucherUser.voucherUserPagination;
export const selectVoucherUser = (state: RootState) => state.voucherUser.voucherUser;

export const selectUsers = (state: RootState) => state.voucherUser.users;
export const selectUserPagination = (state: RootState) => state.voucherUser.userPagination;