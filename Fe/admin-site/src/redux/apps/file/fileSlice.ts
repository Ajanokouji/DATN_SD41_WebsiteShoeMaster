import { createSlice } from "@reduxjs/toolkit";
import { createAppThunk } from "@/utils/createThunk";
import { addLoadingCases } from "@/utils/redux.utils";
import fileService from "@/redux/api/fileApi";

interface FileState {
  loading: boolean;
  error: string | null;
  uploadedFiles: string[];
}

const initialState: FileState = {
  loading: false,
  error: null,
  uploadedFiles: [],
};

export const uploadFile = createAppThunk(
  "file/upload",
  async (file: File) => {
    const response = await fileService.uploadFile(file);
    return response;
  },
  {
    successMessage: "Upload file thành công!",
    errorMessage: "Upload file thất bại. Vui lòng thử lại.",
  }
);

const fileSlice = createSlice({
  name: "file",
  initialState,
  reducers: {
    clearUploadedFiles: (state) => {
      state.uploadedFiles = [];
    },
  },
  extraReducers: (builder) => {
    addLoadingCases(builder, uploadFile, {
      onFulfilled: (state, action) => {
        state.loading = false;
        state.uploadedFiles.push(action.payload);
      },
    });
  },
});

export const { clearUploadedFiles } = fileSlice.actions;
export default fileSlice.reducer; 