import { RootState } from "@/redux/store";

export const selectContentBases = (state: RootState) => state.contentBase.contentBases;
export const selectPagination = (state: RootState) => state.contentBase.pagination;
export const selectContentBase = (state: RootState) => state.contentBase.contentBase;