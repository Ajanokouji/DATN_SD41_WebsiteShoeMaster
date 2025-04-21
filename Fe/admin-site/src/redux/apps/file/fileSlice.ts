import { createSlice } from "@reduxjs/toolkit";
import { createAppThunk } from "@/utils/createThunk";
import { addLoadingCases } from "@/utils/redux.utils";
import fileApi from "@/redux/api/fileApi";
import type { FileItem } from "@/types/file";

// Define the state interface
interface FileState {
  loading: boolean;
  error: string | null;
  files: FileItem[];
  totalRecords: number;
  currentPage: number;
  pageSize: number;
}

// Initial state
const initialState: FileState = {
  loading: false,
  error: null,
  files: [],
  totalRecords: 0,
  currentPage: 1,
  pageSize: 20,
};

// Thunks
export const fetchFiles = createAppThunk(
  "file/fetchFiles",
  async (params?: any) => {
    const response = await fileApi.getFiles(params);
    return response;
  }
);

export const uploadFile = createAppThunk(
  "file/upload",
  async (file: File) => {
    const response = await fileApi.uploadFile(file);
    return response;
  }
);

export const deleteFile = createAppThunk(
  "file/delete",
  async (id: string) => {
    await fileApi.deleteFile(id);
    return id;
  }
);

export const renameFile = createAppThunk(
  "file/rename",
  async ({ id, fileName }: { id: string; fileName: string }) => {
    await fileApi.renameFile(id, fileName);
    return { id, fileName };
  }
);

// Slice
const fileSlice = createSlice({
  name: "file",
  initialState,
  reducers: {
    clearFiles: (state) => {
      state.files = [];
      state.totalRecords = 0;
      state.currentPage = 1;
    },
  },
  extraReducers: (builder) => {
    // Fetch files
    addLoadingCases(builder, fetchFiles, {
      onFulfilled: (state, action) => {
        state.files = action.payload.content;
        state.totalRecords = action.payload.totalRecords;
        state.currentPage = action.payload.currentPage;
        state.pageSize = action.payload.pageSize;
      },
    });

    // Upload file
    addLoadingCases(builder, uploadFile, {
      onFulfilled: (state) => {
        state.loading = false;
      },
    });

    // Delete file
    addLoadingCases(builder, deleteFile, {
      onFulfilled: (state, action) => {
        state.files = state.files.filter(file => file.id !== action.payload);
        state.totalRecords--;
      },
    });

    // Rename file
    addLoadingCases(builder, renameFile, {
      onFulfilled: (state, action) => {
        const file = state.files.find(f => f.id === action.payload.id);
        if (file) {
          file.fileName = action.payload.fileName;
        }
      },
    });
  },
});

export const { clearFiles } = fileSlice.actions;
export default fileSlice.reducer; 