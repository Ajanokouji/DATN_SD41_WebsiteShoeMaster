import { createAsyncThunk } from "@reduxjs/toolkit";
import { handleAxiosError } from "@/utils/error.utils";
import { showNotification } from "@/redux/apps/message/messageSlice";
import type { RootState, AppDispatch } from "@/redux/store";

interface ThunkOptions {
  successMessage?: string;
  errorMessage?: string;
  onSuccess?: (dispatch: AppDispatch) => void;
}

export const createAppThunk = createAsyncThunk.withTypes<{
  state: RootState;
  dispatch: AppDispatch;
  rejectValue: string;
}>();
