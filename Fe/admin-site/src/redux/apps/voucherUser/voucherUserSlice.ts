import { createSlice } from "@reduxjs/toolkit";
import { createAppThunk } from "@/utils/createThunk";
import VoucherUserReqDto, { VoucherUserFilterParams, VoucherUserResDto, UserFilterParamsForSearch, UserResDtoForSearch } from "@/types/voucherUser/voucherUser";
import voucherUserService from "@/redux/api/voucherUserApi";
import { addLoadingCases } from "@/utils/redux.utils";

export interface InitState {
  loading: boolean;
  error: string | null;
  voucherUser: VoucherUserResDto | null;
  voucherUsers: VoucherUserResDto[];
  users: UserResDtoForSearch[] | null;
userPagination: {
    currentPage: number;
    totalPages: number;
    pageSize: number;
    totalRecords: number;
    };
voucherUserPagination: {
    currentPage: number;
    totalPages: number;
    pageSize: number;
    totalRecords: number;
    };
}

const initialState: InitState = {
  loading: false,
  error: null,
  voucherUser: null,
  voucherUsers: [],
  users: [],
  userPagination: {
    currentPage: 1,
    totalPages: 0,
    pageSize: 20,
    totalRecords: 0,
  },
  voucherUserPagination: {
    currentPage: 1,
    totalPages: 0,
    pageSize: 20,
    totalRecords: 0,
  },
};

export const fetchVoucherUsers = createAppThunk(
  "voucherUsers/fetch",
  async (queryModel: VoucherUserFilterParams) => {
    const response = await voucherUserService.getVoucherUsers(queryModel);
    return response;
  }
);

export const fetchVoucherUserById = createAppThunk(
  "voucherUser/fetchById",
  async (id: string) => {
    const response = await voucherUserService.getVoucherUserById(id);
    return response;
  }
);

export const createVoucherUsers = createAppThunk(
  "voucherUser/create",
  async (voucherProducts: VoucherUserReqDto[]) => {
    const response = await voucherUserService.createVoucherUsers(voucherProducts);
    return response;
  },
  {
    successMessage: "Create VoucherUser success!",
    errorMessage: "Create VoucherUser fail. Please try again.",
  }
);

export const updateVoucherUser = createAppThunk(
  "voucherUser/update",
  async ({ id, data }: { id: string; data: Partial<VoucherUserReqDto> }) => {
    const response = await voucherUserService.updateVoucherUser(id, data);
    return response;
  },
  {
    successMessage: "Update VoucherUser success!",
    errorMessage: "Update VoucherUser fail. Please try again.",
  }
);

export const deleteVoucherUser = createAppThunk(
  "voucherUser/delete",
  async (id: string) => {
    await voucherUserService.deleteVoucherUser(id);
    return id;
  },
  {
    successMessage: "Delete VoucherUser success!",
    errorMessage: "Delete VoucherUser fail. Please try again.",
  }
);

export const searchUser = createAppThunk(
  "voucherUser/search",
  async (queryModel: UserFilterParamsForSearch) => {
    const response = await voucherUserService.searchUser(queryModel);
    return response;
  },
  {
    errorMessage: "Search user failed",
  }
);

export const fetchUsersByIds = createAppThunk(
  "voucherUser/fetchUsersByIds",
  async (ids: string[]) => {
    const response = await voucherUserService.getUsersByIds(ids);
    return response;
  },
  {
    errorMessage: "Fech users by IDs failed",
  }
);

export const findVoucherUsersByVidUid = createAppThunk(
  "voucherUser/findByVidUid",
  async (voucherUsers: VoucherUserReqDto[]) => {
    const response = await voucherUserService.findVoucherUsersByVidUid(voucherUsers);
    return response;
  },
  {
    errorMessage: "Find voucher users by VidUid failed",
  }
);

const voucherUserSlice = createSlice({
  name: "voucherUser",
  initialState,
  reducers: {
    setUserPage(state, action) {
      state.userPagination.currentPage = action.payload;
    },
    setUserPageSize(state, action) {
      state.userPagination.pageSize = action.payload;
    },
    setVoucherUserPage(state, action) {
      state.voucherUserPagination.currentPage = action.payload;
    },
    setVoucherUserPageSize(state, action) {
      state.voucherUserPagination.pageSize = action.payload;
    },
    clearUsers(state) {
      state.users = null; // Đặt users thành null
    }
  },
  extraReducers: (builder) => {
    addLoadingCases(builder, fetchVoucherUsers, {
        onFulfilled: (state, action) => {
        state.loading = false;
        state.voucherUsers = action.payload.data.content;
        state.voucherUserPagination = {
            currentPage: action.payload.data.currentPage,
            totalPages: action.payload.data.totalPages,
            pageSize: action.payload.data.pageSize,
            totalRecords: action.payload.data.totalRecords,
        };
        },
    });

    addLoadingCases(builder, fetchVoucherUserById, {
    onFulfilled: (state, action) => {
        state.loading = false;
        state.voucherUser = action.payload;
    },
    });

    addLoadingCases(builder, createVoucherUsers, {
    onFulfilled: (state, action) => {
        state.loading = false;
        state.voucherUsers = [...action.payload, ...state.voucherUsers]; // Thêm danh sách mới vào đầu mảng
    },
    });

    addLoadingCases(builder, updateVoucherUser, {
    onFulfilled: (state, action) => {
        state.loading = false;
        state.voucherUsers = state.voucherUsers.map((voucherUser) =>
        voucherUser.id === action.payload.id ? action.payload : voucherUser
        );
    },
    });

    addLoadingCases(builder, deleteVoucherUser, {
    onFulfilled: (state, action) => {
        state.loading = false;
        state.voucherUsers = state.voucherUsers.filter(
        (voucherUser) => voucherUser.id !== action.payload
        );
    },
    });

    addLoadingCases(builder, searchUser, {
    onFulfilled: (state, action) => {
        state.loading = false;
        state.users = action.payload.data.content; // Cập nhật danh sách user
        state.userPagination = {
        currentPage: action.payload.data.currentPage,
        totalPages: action.payload.data.totalPages,
        pageSize: action.payload.data.pageSize,
        totalRecords: action.payload.data.totalRecords,
        };
    },
    });
},
});

export const {
    setUserPage,
    setUserPageSize,
    clearUsers,
    setVoucherUserPage,
    setVoucherUserPageSize,
    } = voucherUserSlice.actions;
export default voucherUserSlice.reducer;