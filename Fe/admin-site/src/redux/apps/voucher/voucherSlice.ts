import { createSlice, PayloadAction } from "@reduxjs/toolkit";
import { addLoadingCases } from "@/utils/redux.utils";
import { createAppThunk } from "@/utils/createThunk";
import VoucherReqDto, { VoucherFilterParams, VoucherResDto } from "@/types/voucher/voucher";
import voucherService from "@/redux/api/voucher";
import { VOUCHER_MESSAGES } from "@/constants/voucher.constants";


export interface InitState {
  loading: boolean;
  error: string | null;
  voucher: VoucherResDto | null;
  vouchers: VoucherResDto[];
  pagination: {
    currentPage: number;
    totalPages: number;
    pageSize: number;
    totalRecords: number;
  };
}

const initialState: InitState = {
  loading: false,
  error: null,
  voucher: null,
  vouchers: [],
  pagination: {
    currentPage: 1,
    totalPages: 0,
    pageSize: 20,
    totalRecords: 0,
  },
};

export const fetchVouchers = createAppThunk(
  "vouchers/fetch",
  async (params: VoucherFilterParams) => {
    const response = await voucherService.getVouchers(params);
    return response;
  }
);

export const fetchVoucherById = createAppThunk(
  "voucher/fetch",
  async (id: string) => {
    const response = await voucherService.getVoucherById(id);
    return response;
  }
);

export const createVoucher = createAppThunk(
  "voucher/create",
  voucherService.createVoucherReq,
  {
    successMessage: VOUCHER_MESSAGES.CREATE_VOUCHER.SUCCESS,
    errorMessage: VOUCHER_MESSAGES.CREATE_VOUCHER.ERROR,
  }
);

export const updateVoucher = createAppThunk(
  "voucher/update",
  async ({ id, data }: { id: string; data: Partial<VoucherReqDto> }) => {
    const response = await voucherService.updateVoucherReq(id, data);
    return response;
  },
  {
    successMessage: VOUCHER_MESSAGES.UPDATE_VOUCHER.SUCCESS,
    errorMessage: VOUCHER_MESSAGES.UPDATE_VOUCHER.ERROR,
  }
);

export const deleteVoucher = createAppThunk(
  "voucher/delete",
  async (id: string) => {
    await voucherService.deleteVoucherReq(id);
    return id;
  },
  {
    successMessage: VOUCHER_MESSAGES.DELETE_VOUCHER.SUCCESS,
    errorMessage: VOUCHER_MESSAGES.DELETE_VOUCHER.ERROR,
  }
);

export const checkVoucherCodeExist = createAppThunk(
  "voucher/checkCodeExist",
  async ({ code, voucherId }: { code: string; voucherId?: string }) => {
    const isExist = await voucherService.checkVoucherCodeExist(code, voucherId);
    return isExist;
  }
);

const voucherSlice = createSlice({
  name: "voucher",
  initialState,
  reducers: {
    setPage: (state, action: PayloadAction<number>) => {
      state.pagination.currentPage = action.payload;
    },
    setPageSize: (state, action: PayloadAction<number>) => {
      state.pagination.pageSize = action.payload;
    },
  },
  extraReducers: (builder) => {
    addLoadingCases(builder, fetchVouchers, {
      onFulfilled: (state, action) => {
        state.loading = false;
        state.vouchers = action.payload.data.content;
        state.pagination = {
          currentPage: action.payload.data.currentPage,
          totalPages: action.payload.data.totalPages,
          pageSize: action.payload.data.pageSize,
          totalRecords: action.payload.data.totalRecords,
        };
      },
    });

    // Fetch by ID
    addLoadingCases(builder, fetchVoucherById, {
      onFulfilled: (state, action) => {
        state.loading = false;
        state.voucher = action.payload;
      },
    });

    // Create voucher
    addLoadingCases(builder, createVoucher, {
      onFulfilled: (state, action) => {
        state.loading = false;
        // state.voucher = action?.payload ?? null;
        state.vouchers = [action.payload, ...state.vouchers];
      },
    });

    // Update voucher
    addLoadingCases(builder, updateVoucher, {
      onFulfilled: (state, action) => {
        state.loading = false;
        state.vouchers = state.vouchers.map((voucher) =>
          voucher.id === action.payload.id ? action.payload : voucher
        );
      },
    });

    // Delete voucher
    addLoadingCases(builder, deleteVoucher, {
      onFulfilled: (state, action) => {
        state.loading = false;
        state.vouchers = state.vouchers.filter(
          (voucher) => voucher.id !== action.payload
        );
      },
    });
  },
});

export const { setPage, setPageSize } = voucherSlice.actions;
export default voucherSlice.reducer;
