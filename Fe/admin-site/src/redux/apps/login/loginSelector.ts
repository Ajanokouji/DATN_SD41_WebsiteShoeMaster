import { RootState } from "@/redux/store";

export const selectLoginUser = (state: RootState) => state.login.user;
export const selectLoginLoading = (state: RootState) => state.login.loading;
export const selectLoginError = (state: RootState) => state.login.error;
export const selectIsAuthenticated = (state: RootState) => state.login.isAuthenticated;