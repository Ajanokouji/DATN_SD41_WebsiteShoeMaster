import { createSlice, PayloadAction } from "@reduxjs/toolkit";
import { createAppThunk } from "@/utils/createThunk";
import commentService from "@/redux/api/commentApi";
import CommentReqDto, { CommentFilterParams, CommentResDto } from "@/types/comment/comment";
import { addLoadingCases } from "@/utils/redux.utils";

export interface CommentState {
  loading: boolean;
  error: string | null;
  comment: CommentResDto | null;
  comments: CommentResDto[];
  pagination: {
    currentPage: number;
    totalPages: number;
    pageSize: number;
    totalRecords: number;
  };
}

const initialState: CommentState = {
  loading: false,
  error: null,
  comment: null,
  comments: [],
  pagination: {
    currentPage: 1,
    totalPages: 0,
    pageSize: 20,
    totalRecords: 0,
  },
};

export const fetchComments = createAppThunk(
  "comments/fetch",
  async (params: CommentFilterParams) => {
    const response = await commentService.getComments(params);
    return response;
  }
);

export const fetchCommentById = createAppThunk(
  "comment/fetch",
  async (id: string) => {
    const response = await commentService.getCommentById(id);
    return response;
  }
);

export const createComments = createAppThunk(
  "comment/create",
  async (entities: CommentReqDto[]) => {
    const response = await commentService.createComments(entities);
    return response;
  }
);

export const deleteComment = createAppThunk(
  "comment/delete",
  async (id: string) => {
    await commentService.deleteComment(id);
    return id;
  }
);

export const deleteComments = createAppThunk(
  "comment/deleteMany",
  async (ids: string[]) => {
    await commentService.deleteComments(ids);
    return ids;
  }
);

export const updateComment = createAppThunk(
  "comment/update",
  async ({ id, data }: { id: string; data: Partial<CommentResDto> }) => {
    const response = await commentService.updateComment(id, data);
    return response;
  }
);

const commentSlice = createSlice({
  name: "comment",
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
    // Fetch users
      addLoadingCases(builder, fetchComments, {
        onFulfilled: (state, action) => {
          state.loading = false;
          state.comments = action.payload.data.content;
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
          state.error = (action.payload as string) || "Lỗi lấy danh sách đánh giá";
        },
      });

      // Fetch user by ID
      addLoadingCases(builder, fetchCommentById, {
        onFulfilled: (state, action) => {
          state.loading = false;
          state.comment = action.payload;
          state.error = null;
        },
        onRejected: (state, action) => {
          state.loading = false;
          state.error = (action.payload as string) || "Lỗi lấy thông tin đánh giá";
        },
      });

      // Create user
      addLoadingCases(builder, createComments, {
        onFulfilled: (state, action) => {
          state.loading = false;
          state.comments = [...action.payload, ...state.comments];
          state.error = null;
        },
        onRejected: (state, action) => {
          state.loading = false;
          state.error = (action.payload as string) || "Lỗi tạo đánh giá";
        },
      });

      // Update user
      addLoadingCases(builder, updateComment, {
        onFulfilled: (state, action) => {
          state.loading = false;
          state.comments = state.comments.map((comment) =>
            comment.id === action.payload.id ? action.payload : comment
          );
          state.error = null;
        },
        onRejected: (state, action) => {
          state.loading = false;
          state.error = (action.payload as string) || "Lỗi cập nhật đánh giá";
        },
      });

      // Delete user
      addLoadingCases(builder, deleteComment, {
        onFulfilled: (state, action) => {
          state.loading = false;
          state.comments = state.comments.filter((comment) => comment.id !== action.payload);
          state.error = null;
        },
        onRejected: (state, action) => {
          state.loading = false;
          state.error = (action.payload as string) || "Lỗi xóa đánh giá";
        },
      });
  },
});

export const { setPage, setPageSize } = commentSlice.actions;
export default commentSlice.reducer;