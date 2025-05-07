import { createSlice } from "@reduxjs/toolkit";
import { addLoadingCases } from "@/utils/redux.utils";
import { createAppThunk } from "@/utils/createThunk";
import VoucherProductReqDto, { VoucherProductFilterParams, VoucherProductResDto } from "@/types/voucherProduct/voucherProduct";
import voucherProductService from "@/redux/api/voucherProductApi";
import { VOUCHER_PRODUCT_MESSAGES } from "@/constants/voucherProduct.constants";
import { ProductDetailResDto, ProductFilterParams } from "@/types/product/product";

export interface InitState {
  loading: boolean;
  error: string | null;
  voucherProduct: VoucherProductResDto | null;
  voucherProducts: VoucherProductResDto[];
  products: ProductDetailResDto[] | null;
  productPagination: {
    currentPage: number;
    totalPages: number;
    pageSize: number;
    totalRecords: number;
  };
  voucherProductPagination: {
    currentPage: number;
    totalPages: number;
    pageSize: number;
    totalRecords: number;
  };
}

const initialState: InitState = {
  loading: false,
  error: null,
  voucherProduct: null,
  voucherProducts: [],
  products: [],
  productPagination: {
    currentPage: 1,
    totalPages: 0,
    pageSize: 20,
    totalRecords: 0,
  },
  voucherProductPagination: {
    currentPage: 1,
    totalPages: 0,
    pageSize: 20,
    totalRecords: 0,
  },
};

export const fetchVoucherProducts = createAppThunk(
  "voucherProducts/fetch",
  async (params: VoucherProductFilterParams) => {
    const response = await voucherProductService.getVoucherProducts(params);
    return response;
  }
);

export const fetchVoucherProductById = createAppThunk(
  "voucherProduct/fetch",
  async (id: string) => {
    const response = await voucherProductService.getVoucherProductById(id);
    return response;
  }
);

export const createVoucherProduct = createAppThunk(
  "voucherProduct/create",
  async (voucherProducts: VoucherProductReqDto[]) => {
    const response = await voucherProductService.createVoucherProductReq(voucherProducts);
    return response;
  },
  {
    successMessage: VOUCHER_PRODUCT_MESSAGES.CREATE_VOUCHER_PRODUCT.SUCCESS,
    errorMessage: VOUCHER_PRODUCT_MESSAGES.CREATE_VOUCHER_PRODUCT.ERROR,
  }
);

export const updateVoucherProduct = createAppThunk(
  "voucherProduct/update",
  async ({ id, data }: { id: string; data: Partial<VoucherProductReqDto> }) => {
    const response = await voucherProductService.updateVoucherProductReq(id, data);
    return response;
  },
  {
    successMessage: VOUCHER_PRODUCT_MESSAGES.UPDATE_VOUCHER_PRODUCT.SUCCESS,
    errorMessage: VOUCHER_PRODUCT_MESSAGES.UPDATE_VOUCHER_PRODUCT.ERROR,
  }
);

export const deleteVoucherProduct = createAppThunk(
  "voucherProduct/delete",
  async (id: string) => {
    await voucherProductService.deleteVoucherProductReq(id);
    return id;
  },
  {
    successMessage: VOUCHER_PRODUCT_MESSAGES.DELETE_VOUCHER_PRODUCT.SUCCESS,
    errorMessage: VOUCHER_PRODUCT_MESSAGES.DELETE_VOUCHER_PRODUCT.ERROR,
  }
);

export const fetchProductsByIds = createAppThunk(
  "voucherProduct/fetchProductsByIds",
  async (ids: string[]) => {
    const response = await voucherProductService.getProductsByIds(ids);
    return response;
  },
  {
    errorMessage: "Fech products by IDs failed",
  }
);

export const searchProduct = createAppThunk(
  "voucherProduct/searchProduct",
  async (queryModel: ProductFilterParams) => {
    const response = await voucherProductService.searchProduct(queryModel);
    return response;
  },
  {
    errorMessage: "Search product failed",
  }
);

export const findVoucherProductsByVcidPidVaid = createAppThunk(
  "voucherProduct/findByVcidPidVaid",
  async (voucherProducts: VoucherProductReqDto[]) => {
    const response = await voucherProductService.findVoucherProductsByVcidPidVaid(voucherProducts);
    return response;
  },
  {
    errorMessage: "Find voucher products by VcidPidVaid failed",
  }
);

const voucherProductSlice = createSlice({
  name: "voucherProduct",
  initialState,
  reducers: {
    setProductPage(state, action) {
      state.productPagination.currentPage = action.payload;
    },
    setProductPageSize(state, action) {
      state.productPagination.pageSize = action.payload;
    },
    setVoucherProductPage(state, action) {
      state.voucherProductPagination.currentPage = action.payload;
    },
    setVoucherProductPageSize(state, action) {
      state.voucherProductPagination.pageSize = action.payload;
    },
    clearProducts(state) {
      state.products = null; // Đặt products thành null
    }
  },
  extraReducers: (builder) => {
    addLoadingCases(builder, fetchVoucherProducts, {
      onFulfilled: (state, action) => {
        state.loading = false;
        state.voucherProducts = action.payload.data.content;
        state.voucherProductPagination = {
          currentPage: action.payload.data.currentPage,
          totalPages: action.payload.data.totalPages,
          pageSize: action.payload.data.pageSize,
          totalRecords: action.payload.data.totalRecords,
        };
      },
    });

    // Fetch by ID
    addLoadingCases(builder, fetchVoucherProductById, {
      onFulfilled: (state, action) => {
        state.loading = false;
        state.voucherProduct = action.payload;
      },
    });

    // Create voucher
    addLoadingCases(builder, createVoucherProduct, {
      onFulfilled: (state, action) => {
        state.loading = false;
        state.voucherProducts = [...action.payload, ...state.voucherProducts]; // Thêm danh sách mới vào đầu mảng
      },
    });

    // Update voucher
    addLoadingCases(builder, updateVoucherProduct, {
      onFulfilled: (state, action) => {
        state.loading = false;
        state.voucherProducts = state.voucherProducts.map((voucherProduct) =>
          voucherProduct.id === action.payload.id ? action.payload : voucherProduct
        );
      },
    });

    // Delete voucher
    addLoadingCases(builder, deleteVoucherProduct, {
      onFulfilled: (state, action) => {
        state.loading = false;
        state.voucherProducts = state.voucherProducts.filter(
          (voucherProduct) => voucherProduct.id !== action.payload
        );
      },
    });

    addLoadingCases(builder, searchProduct, {
      onFulfilled: (state, action) => {
        state.loading = false;
        state.products = action.payload.data.content; // Cập nhật danh sách sản phẩm
        state.productPagination = {
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
  setProductPage,
  setProductPageSize,
  clearProducts,
  setVoucherProductPage,
  setVoucherProductPageSize,
} = voucherProductSlice.actions;
export default voucherProductSlice.reducer;