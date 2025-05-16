import { createSlice, PayloadAction } from "@reduxjs/toolkit";
import { addLoadingCases } from "@/utils/redux.utils";
import { createAppThunk } from "@/utils/createThunk";
import contentBaseService from "@/redux/api/contentBase";
import { ContentBaseReqDto, ContentBaseResDto, ContentBaseFilterParams } from "@/types/contentBase/contentBase";
import { CONTENTBASE_MESSAGES } from "@/constants/contentBase.constants";

export interface InitState {
  loading: boolean;
  error: string | null;
  contentBase: ContentBaseResDto | null;
  contentBases: ContentBaseResDto[];
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
  contentBase: null,
  contentBases: [],
  pagination: {
    currentPage: 1,
    totalPages: 0,
    pageSize: 20,
    totalRecords: 0,
  },
};

export const fetchContentBases = createAppThunk(
  "contentBases/fetch",
  async (params: ContentBaseFilterParams) => {
    const response = await contentBaseService.getContentBases(params);
    return response;
  }
);

export const fetchContentBaseById = createAppThunk(
  "contentBase/fetch",
  async (id: string) => {
    const response = await contentBaseService.getContentBaseById(id);
    return response;
  }
);

export const createContentBase = createAppThunk(
  "contentBase/create",
  contentBaseService.createContentBaseReq,
    {
      successMessage: CONTENTBASE_MESSAGES.CREATE_CONTENTBASE.SUCCESS,
      errorMessage: CONTENTBASE_MESSAGES.CREATE_CONTENTBASE.ERROR,
    }
);

export const updateContentBase = createAppThunk(
  "contentBase/update",
  async ({ id, data }: { id: string; data: Partial<ContentBaseReqDto> }) => {
    const response = await contentBaseService.updateContentBaseReq(id, data);
    return response;
  },
  {
    successMessage: CONTENTBASE_MESSAGES.UPDATE_CONTENTBASE.SUCCESS,
    errorMessage: CONTENTBASE_MESSAGES.UPDATE_CONTENTBASE.ERROR,
  }
);

export const deleteContentBase = createAppThunk(
  "contentBase/delete",
  async (id: string) => {
    await contentBaseService.deleteContentBaseReq(id);
    return id;
  },
  {
    successMessage: CONTENTBASE_MESSAGES.DELETE_CONTENTBASE.SUCCESS,
    errorMessage: CONTENTBASE_MESSAGES.DELETE_CONTENTBASE.ERROR,
  }
);

const contentBaseSlice = createSlice({
  name: "contentBase",
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
    addLoadingCases(builder, fetchContentBases, {
      onFulfilled: (state, action) => {
        state.loading = false;
        state.contentBases = action.payload.data.content;
        state.pagination = {
          currentPage: action.payload.data.currentPage,
          totalPages: action.payload.data.totalPages,
          pageSize: action.payload.data.pageSize,
          totalRecords: action.payload.data.totalRecords,
        };
      },
    });

    addLoadingCases(builder, fetchContentBaseById, {
      onFulfilled: (state, action) => {
        state.loading = false;
        state.contentBase = action.payload;
      },
    });

    addLoadingCases(builder, createContentBase, {
      onFulfilled: (state, action) => {
        state.loading = false;
        state.contentBases = [action.payload, ...state.contentBases];
      },
    });

    addLoadingCases(builder, updateContentBase, {
      onFulfilled: (state, action) => {
        state.loading = false;
        state.contentBases = state.contentBases.map((contentBase) =>
          contentBase.id === action.payload.id ? action.payload : contentBase
        );
      },
    });

    addLoadingCases(builder, deleteContentBase, {
      onFulfilled: (state, action) => {
        state.loading = false;
        state.contentBases = state.contentBases.filter(
          (contentBase) => contentBase.id !== action.payload
        );
      },
    });
  },
});

export const { setPage, setPageSize } = contentBaseSlice.actions;
export default contentBaseSlice.reducer;