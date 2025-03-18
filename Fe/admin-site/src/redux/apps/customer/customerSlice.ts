import { createSlice, PayloadAction } from "@reduxjs/toolkit";
import { addLoadingCases } from "@/utils/redux.utils";
import { createAppThunk } from "@/utils/createThunk";
import { CustomerFilterParams, CustomerReqDto, CustomerResDto } from "@/types/customer/customer";
import customerService from "@/redux/api/customerApi";
import { CUSTOMER_MESSAGES } from "@/constants/customer.constants";



export interface InitState {
  loading: boolean;
  error: string | null;
  customer: CustomerResDto | null;
  customers: CustomerResDto[];
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
  customer: null,
  customers: [],
  pagination: {
    currentPage: 1,
    totalPages: 0,
    pageSize: 20,
    totalRecords: 0,
  },
};

export const fetchCustomers = createAppThunk(
  "customers/fetch",
  async (params: CustomerFilterParams) => {
    const response = await customerService.getCustomers(params);
    return response;
  }
);

export const fetchCustomerById = createAppThunk(
  "customer/fetch",
  async (id: string) => {
    const response = await customerService.getCustomerById(id);
    return response;
  }
);

export const createCustomer = createAppThunk(
  "customer/create",
  customerService.createCustomerReq,
  {
    successMessage: CUSTOMER_MESSAGES.CREATE_CUSTOMER.SUCCESS,
    errorMessage: CUSTOMER_MESSAGES.CREATE_CUSTOMER.ERROR,
  }
);

export const updateCustomer = createAppThunk(
  "customer/update",
  async ({ id, data }: { id: string; data: Partial<CustomerReqDto> }) => {
    const response = await customerService.updateCustomerReq(id, data);
    return response;
  },
  {
    successMessage: CUSTOMER_MESSAGES.UPDATE_CUSTOMER.SUCCESS,
    errorMessage: CUSTOMER_MESSAGES.UPDATE_CUSTOMER.ERROR,
  }
);

export const deleteCustomer = createAppThunk(
  "customer/delete",
  async (id: string) => {
    await customerService.deleteCustomerReq(id);
    return id;
  },
  {
    successMessage: CUSTOMER_MESSAGES.DELETE_CUSTOMER.SUCCESS,
    errorMessage: CUSTOMER_MESSAGES.DELETE_CUSTOMER.ERROR,
  }
);

const customerSlice = createSlice({
  name: "customer",
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
    addLoadingCases(builder, fetchCustomers, {
      onFulfilled: (state, action) => {
        state.loading = false;
        state.customers = action.payload.data.content;
        state.pagination = {
          currentPage: action.payload.data.currentPage,
          totalPages: action.payload.data.totalPages,
          pageSize: action.payload.data.pageSize,
          totalRecords: action.payload.data.totalRecords,
        };
      },
    });

    // Fetch by ID
    addLoadingCases(builder, fetchCustomerById, {
      onFulfilled: (state, action) => {
        state.loading = false;
        state.customer = action.payload;
      },
    });

    // Create customer
    addLoadingCases(builder, createCustomer, {
      onFulfilled: (state, action) => {
        state.loading = false;
        // state.customer = action?.payload ?? null;
        state.customers = [action.payload, ...state.customers];
      },
    });

    // Update customer
    addLoadingCases(builder, updateCustomer, {
      onFulfilled: (state, action) => {
        state.loading = false;
        state.customers = state.customers.map((customer) =>
          customer.id === action.payload.id ? action.payload : customer
        );
      },
    });

    // Delete customer
    addLoadingCases(builder, deleteCustomer, {
      onFulfilled: (state, action) => {
        state.loading = false;
        state.customers = state.customers.filter(
          (customer) => customer.id !== action.payload
        );
      },
    });
  },
});

export const { setPage, setPageSize } = customerSlice.actions;
export default customerSlice.reducer;
