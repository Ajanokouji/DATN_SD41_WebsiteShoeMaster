import { RootState } from "@/redux/store";

export const selectComments = (state: RootState) => state.comment.comments;
export const selectComment = (state: RootState) => state.comment.comment;
export const selectCommentLoading = (state: RootState) => state.comment.loading;
export const selectCommentError = (state: RootState) => state.comment.error;
export const selectCommentPagination = (state: RootState) => state.comment.pagination;