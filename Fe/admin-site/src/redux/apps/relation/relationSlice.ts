import { createSlice, PayloadAction } from "@reduxjs/toolkit";
import { addLoadingCases } from "@/utils/redux.utils";
import { createAppThunk } from "@/utils/createThunk";
import RelationReqDto, { RelationFilterParams, RelationResDto } from "@/types/category/relation";
import { RELATION_MESSAGES } from "@/constants/relation.constants";
import relationService from "@/redux/api/relationApi";

export interface InitState {
  loading: boolean;
  error: string | null;
  relation: RelationResDto | null;
  relations: RelationResDto[];
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
  relation: null,
  relations: [],
  pagination: {
    currentPage: 1,
    totalPages: 0,
    pageSize: 20,
    totalRecords: 0,
  },
};

export const fetchRelations = createAppThunk(
  "relations/fetch",
  async (params: RelationFilterParams) => {
    const response = await relationService.getRelations(params);
    return response;
  }
);

export const fetchRelationById = createAppThunk(
  "relation/fetch",
  async (id: string) => {
    const response = await relationService.getRelationById(id);
    return response;
  }
);

export const createRelation = createAppThunk(
  "relation/create",
  relationService.createRelationReq,
  {
    successMessage: RELATION_MESSAGES.CREATE_RELATION.SUCCESS,
    errorMessage: RELATION_MESSAGES.CREATE_RELATION.ERROR,
  }
);

export const updateRelation = createAppThunk(
  "relation/update",
  async ({ id, data }: { id: string; data: Partial<RelationReqDto> }) => {
    const response = await relationService.updateRelationReq(id, data);
    return response;
  },
  {
    successMessage: RELATION_MESSAGES.UPDATE_RELATION.SUCCESS,
    errorMessage: RELATION_MESSAGES.UPDATE_RELATION.ERROR,
  }
);

export const deleteRelation = createAppThunk(
  "relation/delete",
  async (id: string) => {
    await relationService.deleteRelationReq(id);
    return id;
  },
  {
    successMessage: RELATION_MESSAGES.DELETE_RELATION.SUCCESS,
    errorMessage: RELATION_MESSAGES.DELETE_RELATION.ERROR,
  }
);

const relationSlice = createSlice({
  name: "relation",
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
    addLoadingCases(builder, fetchRelations, {
      onFulfilled: (state, action) => {
        state.loading = false;
        state.relations = action.payload.data.content;
        state.pagination = {
          currentPage: action.payload.data.currentPage,
          totalPages: action.payload.data.totalPages,
          pageSize: action.payload.data.pageSize,
          totalRecords: action.payload.data.totalRecords,
        };
      },
    });

    // Fetch by ID
    addLoadingCases(builder, fetchRelationById, {
      onFulfilled: (state, action) => {
        state.loading = false;
        state.relation = action.payload;
      },
    });

    // Create relation
    addLoadingCases(builder, createRelation, {
      onFulfilled: (state, action) => {
        state.loading = false;
        // state.relation = action?.payload ?? null;
        state.relations = [action.payload, ...state.relations];
      },
    });

    // Update relation
    addLoadingCases(builder, updateRelation, {
      onFulfilled: (state, action) => {
        state.loading = false;
        state.relations = state.relations.map((relation) =>
          relation.id === action.payload.id ? action.payload : relation
        );
      },
    });

    // Delete relation
    addLoadingCases(builder, deleteRelation, {
      onFulfilled: (state, action) => {
        state.loading = false;
        state.relations = state.relations.filter(
          (relation) => relation.id !== action.payload
        );
      },
    });
  },
});

export const { setPage, setPageSize } = relationSlice.actions;
export default relationSlice.reducer;
