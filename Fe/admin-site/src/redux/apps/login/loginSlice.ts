import { createSlice, PayloadAction } from "@reduxjs/toolkit";
import { createAppThunk } from "@/utils/createThunk";
import loginService from "@/redux/api/loginApi";
import type { UserResDto } from "@/types/user/user";

export interface LoginState {
  loading: boolean;
  error: string | null;
  user: UserResDto | null;
  isAuthenticated: boolean;
}

const initialState: LoginState = {
  loading: false,
  error: null,
  user: null,
  isAuthenticated: false,
};

export const login = createAppThunk(
  "login/login",
  async (data: { userName: string; password: string }) => {
    const response = await loginService.login(data);
    return response;
  }
);

const loginSlice = createSlice({
  name: "login",
  initialState,
  reducers: {
    logout: (state) => {
      state.user = null;
      state.isAuthenticated = false;
      state.error = null;
    },
  },
  extraReducers: (builder) => {
    builder
      .addCase(login.pending, (state) => {
        state.loading = true;
        state.error = null;
      })
      .addCase(login.fulfilled, (state, action: PayloadAction<UserResDto | null>) => {
        state.loading = false;
        if (action.payload) {
          state.user = action.payload;
          state.isAuthenticated = true;
        } else {
          state.user = null;
          state.isAuthenticated = false;
          state.error = "Sai tài khoản hoặc mật khẩu!";
        }
      })
      .addCase(login.rejected, (state, action) => {
        state.loading = false;
        state.user = null;
        state.isAuthenticated = false;
        state.error = action.error.message || "Đăng nhập thất bại!";
      });
  },
});

export const { logout } = loginSlice.actions;
export default loginSlice.reducer;