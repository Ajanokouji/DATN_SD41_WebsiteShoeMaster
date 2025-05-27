import { createAsyncThunk } from "@reduxjs/toolkit";
import { handleAxiosError } from "@/utils/error.utils";
import { showNotification } from "@/redux/apps/message/messageSlice";
import type { RootState, AppDispatch } from "@/redux/store";

interface ThunkOptions {
  successMessage?: string;
  errorMessage?: string;
  onSuccess?: (dispatch: AppDispatch) => void;
}

export const createAppThunk = <Returned, ThunkArg = void>(
  typePrefix: string,
  payloadCreator: (arg: ThunkArg) => Promise<Returned>,
  options: ThunkOptions = {}
) => {
  return createAsyncThunk<Returned, ThunkArg, { state: RootState; dispatch: AppDispatch; rejectValue: string }>(
    typePrefix,
    async (arg, { dispatch, rejectWithValue }) => {
      try {
        const response = await payloadCreator(arg);
        
        if (options.successMessage) {
          dispatch(showNotification({ message: options.successMessage, type: "success" }));
        }

        if (options.onSuccess) {
          options.onSuccess(dispatch);
        }

        return response;
      } catch (error) {
        const errorMessage = options.errorMessage || handleAxiosError(error);
        dispatch(showNotification({ message: errorMessage, type: "error" }));
        return rejectWithValue(errorMessage);
      }
    }
  );
};
