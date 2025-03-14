import { createSlice, PayloadAction } from "@reduxjs/toolkit";
import { addLoadingCases } from "@/utils/redux.utils";
import { createAppThunk } from "@/utils/createThunk";
import ContactReqDto, { ContactFilterParams, ContactResDto } from "@/types/contact/contact";
import contactService from "@/redux/api/contactApi";
import { CONTACT_MESSAGES } from "@/constants/contact.constants";


export interface InitState {
  loading: boolean;
  error: string | null;
  contact: ContactResDto | null;
  contacts: ContactResDto[];
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
  contact: null,
  contacts: [],
  pagination: {
    currentPage: 1,
    totalPages: 0,
    pageSize: 20,
    totalRecords: 0,
  },
};

export const fetchContacts = createAppThunk(
  "contacts/fetch",
  async (params: ContactFilterParams) => {
    const response = await contactService.getContacts(params);
    return response;
  }
);

export const fetchContactById = createAppThunk(
  "contact/fetch",
  async (id: string) => {
    const response = await contactService.getContactById(id);
    return response;
  }
);

export const createContact = createAppThunk(
  "contact/create",
  contactService.createContactReq,
  {
    successMessage: CONTACT_MESSAGES.CREATE_CONTACT.SUCCESS,
    errorMessage: CONTACT_MESSAGES.CREATE_CONTACT.ERROR,
  }
);

export const updateContact = createAppThunk(
  "contact/update",
  async ({ id, data }: { id: string; data: Partial<ContactReqDto> }) => {
    const response = await contactService.updateContactReq(id, data);
    return response;
  },
  {
    successMessage: CONTACT_MESSAGES.UPDATE_CONTACT.SUCCESS,
    errorMessage: CONTACT_MESSAGES.UPDATE_CONTACT.ERROR,
  }
);

export const deleteContact = createAppThunk(
  "contact/delete",
  async (id: string) => {
    await contactService.deleteContactReq(id);
    return id;
  },
  {
    successMessage: CONTACT_MESSAGES.DELETE_CONTACT.SUCCESS,
    errorMessage: CONTACT_MESSAGES.DELETE_CONTACT.ERROR,
  }
);

const contactSlice = createSlice({
  name: "contact",
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
    addLoadingCases(builder, fetchContacts, {
      onFulfilled: (state, action) => {
        state.loading = false;
        state.contacts = action.payload.data.content;
        state.pagination = {
          currentPage: action.payload.data.currentPage,
          totalPages: action.payload.data.totalPages,
          pageSize: action.payload.data.pageSize,
          totalRecords: action.payload.data.totalRecords,
        };
      },
    });

    // Fetch by ID
    addLoadingCases(builder, fetchContactById, {
      onFulfilled: (state, action) => {
        state.loading = false;
        state.contact = action.payload;
      },
    });

    // Create contact
    addLoadingCases(builder, createContact, {
      onFulfilled: (state, action) => {
        state.loading = false;
        // state.contact = action?.payload ?? null;
        state.contacts = [action.payload, ...state.contacts];
      },
    });

    // Update contact
    addLoadingCases(builder, updateContact, {
      onFulfilled: (state, action) => {
        state.loading = false;
        state.contacts = state.contacts.map((contact) =>
          contact.id === action.payload.id ? action.payload : contact
        );
      },
    });

    // Delete contact
    addLoadingCases(builder, deleteContact, {
      onFulfilled: (state, action) => {
        state.loading = false;
        state.contacts = state.contacts.filter(
          (contact) => contact.id !== action.payload
        );
      },
    });
  },
});

export const { setPage, setPageSize } = contactSlice.actions;
export default contactSlice.reducer;
