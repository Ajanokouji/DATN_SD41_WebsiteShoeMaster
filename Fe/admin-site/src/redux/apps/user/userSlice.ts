import { createSlice, PayloadAction } from "@reduxjs/toolkit";
import { addLoadingCases } from "@/utils/redux.utils";
import { createAppThunk } from "@/utils/createThunk";
import userService from "@/redux/api/userService";
import UserReqDto, { UserResDto, UserFilterParams } from "@/types/user/user";

interface UserState {
  loading: boolean;
  error: string | null;
  user: UserResDto | null;
  users: UserResDto[];
  pagination: {
    currentPage: number;
    totalPages: number;
    pageSize: number;
    totalRecords: number;
  };
}

const initialState: UserState = {
  loading: false,
  error: null,
  user: null,
  users: [],
  pagination: {
    currentPage: 1,
    totalPages: 0,
    pageSize: 20,
    totalRecords: 0,
  },
};

export const fetchUsers = createAppThunk(
  "user/fetchUsers",
  async (params: UserFilterParams) => {
    const response = await userService.getUsers(params);
    return response;
  }
);

export const fetchUserById = createAppThunk(
  "user/fetchUserById",
  async (id: string) => {
    const response = await userService.getUserById(id);
    return response;
  }
);

export const createUser = createAppThunk(
  "user/createUser",
  userService.createUserReq,
  {
    successMessage: "Tạo tài khoản thành công!",
    errorMessage: "Tạo tài khoản thất bại!",
  }
);

export const updateUser = createAppThunk(
  "user/updateUser",
  async ({ id, userData }: { id: string; userData: Partial<UserReqDto> }) => {
    const response = await userService.updateUserReq(id, userData);
    return response;
  },
  {
    successMessage: "Cập nhật tài khoản thành công!",
    errorMessage: "Cập nhật tài khoản thất bại!",
  }
);

export const deleteUser = createAppThunk(
  "user/deleteUser",
  async (id: string) => {
    await userService.deleteUserReq(id);
    return id;
  },
  {
    successMessage: "Xóa tài khoản thành công!",
    errorMessage: "Xóa tài khoản thất bại!",
  }
);

// Thunk kiểm tra trùng username
export const checkTrungCodeUser = createAppThunk(
  "user/checkTrungCodeUser",
  async ({ username, id }: { username: string; id?: string }) => {
    const response = await userService.checkTrungCodeUser(username, id);
    return response;
  }
);

const userSlice = createSlice({
  name: "user",
  initialState,
  reducers: {
    setUserPage: (state, action: PayloadAction<number>) => {
      state.pagination.currentPage = action.payload;
    },
    setUserPageSize: (state, action: PayloadAction<number>) => {
      state.pagination.pageSize = action.payload;
    },
    clearUserError: (state) => {
      state.error = null;
    },
  },
  extraReducers: (builder) => {
    // Fetch users
    addLoadingCases(builder, fetchUsers, {
      onFulfilled: (state, action) => {
        state.loading = false;
        state.users = action.payload.data.content;
        state.pagination = {
          currentPage: action.payload.data.currentPage,
          totalPages: action.payload.data.totalPages,
          pageSize: action.payload.data.pageSize,
          totalRecords: action.payload.data.totalRecords,
        };
        state.error = null;
      },
      onRejected: (state, action) => {
        state.loading = false;
        state.error = (action.payload as string) || "Lỗi lấy danh sách tài khoản";
      },
    });

    // Fetch user by ID
    addLoadingCases(builder, fetchUserById, {
      onFulfilled: (state, action) => {
        state.loading = false;
        state.user = action.payload;
        state.error = null;
      },
      onRejected: (state, action) => {
        state.loading = false;
        state.error = (action.payload as string) || "Lỗi lấy thông tin tài khoản";
      },
    });

    // Create user
    addLoadingCases(builder, createUser, {
      onFulfilled: (state, action) => {
        state.loading = false;
        state.users = [action.payload, ...state.users];
        state.error = null;
      },
      onRejected: (state, action) => {
        state.loading = false;
        state.error = (action.payload as string) || "Lỗi tạo tài khoản";
      },
    });

    // Update user
    addLoadingCases(builder, updateUser, {
      onFulfilled: (state, action) => {
        state.loading = false;
        state.users = state.users.map((user) =>
          user.id === action.payload.id ? action.payload : user
        );
        state.error = null;
      },
      onRejected: (state, action) => {
        state.loading = false;
        state.error = (action.payload as string) || "Lỗi cập nhật tài khoản";
      },
    });

    // Delete user
    addLoadingCases(builder, deleteUser, {
      onFulfilled: (state, action) => {
        state.loading = false;
        state.users = state.users.filter((user) => user.id !== action.payload);
        state.error = null;
      },
      onRejected: (state, action) => {
        state.loading = false;
        state.error = (action.payload as string) || "Lỗi xóa tài khoản";
      },
    });
  },
});

export const { setUserPage, setUserPageSize, clearUserError } = userSlice.actions;
export default userSlice.reducer;