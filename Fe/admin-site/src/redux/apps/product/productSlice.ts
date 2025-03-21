import { createSlice, PayloadAction } from "@reduxjs/toolkit";
import { addLoadingCases } from "@/utils/redux.utils";
import { createAppThunk } from "@/utils/createThunk";
import ProductReqDto, {
  ProductDetailResDto,
  ProductFilterParams,
  ProductResDto,
} from "@/types/product/product";
import productService from "@/redux/api/productApi";
import { PRODUCT_MESSAGES } from "@/constants/product.constants";

export interface InitState {
  loading: boolean;
  error: string | null;
  product: ProductDetailResDto | null;
  products: ProductResDto[];
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
  product: null,
  products: [],
  pagination: {
    currentPage: 1,
    totalPages: 0,
    pageSize: 20,
    totalRecords: 0,
  },
};

export const fetchProducts = createAppThunk(
  "products/fetch",
  async (params: ProductFilterParams) => {
    const response = await productService.getProducts(params);
    return response;
  }
);
export const fetchProductById = createAppThunk(
  "product/fetch",
  async (id: string) => {
    const response = await productService.getProductById(id);
    return response;
  }
);

export const createProduct = createAppThunk(
  "product/create",
  productService.createProductReq,
  {
    successMessage: PRODUCT_MESSAGES.CREATE_PRODUCT.SUCCESS,
    errorMessage: PRODUCT_MESSAGES.CREATE_PRODUCT.ERROR,
  }
);

export const updateProduct = createAppThunk(
  "product/update",
  async ({ id, data }: { id: string; data: Partial<ProductReqDto> }) => {
    const response = await productService.updateProductReq(id, data);
    return response;
  },
  {
    successMessage: PRODUCT_MESSAGES.UPDATE_PRODUCT.SUCCESS,
    errorMessage: PRODUCT_MESSAGES.UPDATE_PRODUCT.ERROR,
  }
);

export const deleteProduct = createAppThunk(
  "product/delete",
  async (id: string) => {
    await productService.deleteProductReq(id);
    return id;
  },
  {
    successMessage: PRODUCT_MESSAGES.DELETE_PRODUCT.SUCCESS,
    errorMessage: PRODUCT_MESSAGES.DELETE_PRODUCT.ERROR,
  }
);

const productSlice = createSlice({
  name: "product",
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
    addLoadingCases(builder, fetchProducts, {
      onFulfilled: (state, action) => {
        state.loading = false;
        state.products = action.payload.data.content;
        state.pagination = {
          currentPage: action.payload.data.currentPage,
          totalPages: action.payload.data.totalPages,
          pageSize: action.payload.data.pageSize,
          totalRecords: action.payload.data.totalRecords,
        };
      },
    });

    // Fetch by ID
    addLoadingCases(builder, fetchProductById, {
      onFulfilled: (state, action) => {
        state.loading = false;
        state.product = action.payload;
      },
    });

    // Create product
    addLoadingCases(builder, createProduct, {
      onFulfilled: (state, action) => {
        state.loading = false;
        // state.product = action?.payload ?? null;
        state.products = [action.payload, ...state.products];
      },
    });

    // Update
    addLoadingCases(builder, updateProduct, {
      onFulfilled: (state, action) => {
        state.loading = false;
        state.products = state.products.map((product) =>
          product.id === action.payload.id ? action.payload : product
        );
      },
    });

    // Delete product
    addLoadingCases(builder, deleteProduct, {
      onFulfilled: (state, action) => {
        state.loading = false;
        state.products = state.products.filter(
          (product) => product.id !== action.payload
        );
      },
    });
  },
});

export const { setPage, setPageSize } = productSlice.actions;
export default productSlice.reducer;
