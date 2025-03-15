import { RootState } from "@/redux/store";

export const selectContacts = (state: RootState) => state.contact.contacts;
export const selectPagination = (state: RootState) => state.contact.pagination;

export const selectContact = (state: RootState) => state.contact.contact;
