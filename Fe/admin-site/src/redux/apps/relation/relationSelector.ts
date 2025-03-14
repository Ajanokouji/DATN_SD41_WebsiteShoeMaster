import { RootState } from "@/redux/store";

export const selectRelations = (state: RootState) => state.relation.relations;
export const selectPagination = (state: RootState) => state.relation.pagination;

export const selectRelation = (state: RootState) => state.relation.relation;
