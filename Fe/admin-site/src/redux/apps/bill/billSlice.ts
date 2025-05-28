import { createSlice, PayloadAction } from "@reduxjs/toolkit";
import { createAppThunk } from "@/utils/createThunk";
import { addLoadingCases } from "@/utils/redux.utils";
import { BILL_MESSAGES } from "@/constants/bill.constants";
import { BillDetailResDto, BillFilterParams, BillResDto } from "@/types/bill/bill";
import billService from "@/redux/api/billApi";

// Định nghĩa state ban đầu
interface BillState {
  loading: boolean;
  error: string | null;
  bill: BillDetailResDto | null;
  bills: BillResDto[];
  pagination: {
    currentPage: number;
    totalPages: number;
    pageSize: number;
    totalRecords: number;
  };
}

const initialState: BillState = {
  loading: false,
  error: null,
  bill: null,
  bills: [],
  pagination: {
    currentPage: 1,
    totalPages: 0,
    pageSize: 20,
    totalRecords: 0,
  },
};

// Fetch danh sách hóa đơn có filter
export const fetchBills = createAppThunk(
  "bill/fetchAll",
  async (params: BillFilterParams) => {
    const response = await billService.getBills(params);
    return response;
  }
);

// Fetch chi tiết một hóa đơn theo ID
export const fetchBillById = createAppThunk(
  "bill/fetchById",
  async (id: string) => {
    const response = await billService.getBillById(id);
    return response;
  }
);

// Tạo mới hóa đơn
export const createBill = createAppThunk(
  "bill/create",
  billService.createBillReq,
  {
    successMessage: BILL_MESSAGES.CREATE_BILL.SUCCESS,
    errorMessage: BILL_MESSAGES.CREATE_BILL.ERROR,
  }
);

// Cập nhật hóa đơn
export const updateBill = createAppThunk(
  "bill/update",
  async ({ id, data }: { id: string; data: Partial<BillResDto> }) => {
    const response = await billService.updateBillReq(id, data);
    return response;
  },
  {
    successMessage: BILL_MESSAGES.UPDATE_BILL.SUCCESS,
    errorMessage: BILL_MESSAGES.UPDATE_BILL.ERROR,
  }
);

// Cập nhật hóa đơn theo trường gửi lên
export const patchBill = createAppThunk(
  "bill/patch",
  async ({ id, data }: { id: string; data: Partial<BillResDto> }) => {
    const response = await billService.patchBillReq(id, data);
    return response;
  },
  {
    successMessage: BILL_MESSAGES.UPDATE_BILL.SUCCESS,
    errorMessage: BILL_MESSAGES.UPDATE_BILL.ERROR,
  }
);


// Xóa hóa đơn
export const deleteBill = createAppThunk(
  "bill/delete",
  async (id: string) => {
    await billService.deleteBillReq(id);
    return id;
  },
  {
    successMessage: BILL_MESSAGES.DELETE_BILL.SUCCESS,
    errorMessage: BILL_MESSAGES.DELETE_BILL.ERROR,
  }
);

// Kiểm tra số lượng trong kho
export const checkInventory = createAppThunk(
  "bill/checkInventory",
  async (id: string) => {
    const response = await billService.checkInventory(id);
    console.log(response)
    return response;
  }
);

// Slice quản lý trạng thái của bill
const billSlice = createSlice({
  name: "bill",
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
    addLoadingCases(builder, fetchBills, {
      onFulfilled: (state, action) => {
        state.loading = false;
        state.bills = action.payload.data.content;
        state.pagination = {
          currentPage: action.payload.data.currentPage,
          totalPages: action.payload.data.totalPages,
          pageSize: action.payload.data.pageSize,
          totalRecords: action.payload.data.totalRecords,
        };
      },
    });

    addLoadingCases(builder, fetchBillById, {
      onFulfilled: (state, action) => {
        state.loading = false;
        state.bill = action.payload;
      },
    });

    addLoadingCases(builder, createBill, {
      onFulfilled: (state, action) => {
        state.loading = false;
        state.bills = [action.payload, ...state.bills];
      },
    });

    addLoadingCases(builder, updateBill, {
      onFulfilled: (state, action) => {
        state.loading = false;
        state.bills = state.bills.map((bill) =>
          bill.id === action.payload.id ? action.payload : bill
        );
      },
    });

    addLoadingCases(builder, deleteBill, {
      onFulfilled: (state, action) => {
        state.loading = false;
        state.bills = state.bills.filter((bill) => bill.id !== action.payload);
      },
    });
  },
});

export const { setPage, setPageSize } = billSlice.actions;
export default billSlice.reducer;
