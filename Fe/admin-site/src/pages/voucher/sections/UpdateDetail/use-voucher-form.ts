import { useEffect, useState } from "react";
import { useForm } from "react-hook-form";
import { useAppDispatch } from "@/hooks/use-app-dispatch";
import {
  fetchVoucherById,
  updateVoucher,
} from "@/redux/apps/voucher/voucherSlice";

interface FormData {
  voucherName: string;
  startDate: string;
  endDate: string;
  // Add other fields as needed
}

export const useVoucherForm = (
  voucherId: string,
  // eslint-disable-next-line @typescript-eslint/no-explicit-any
  voucher: any,
  onClose: () => void
) => {
  const dispatch = useAppDispatch();
  const [isEditing, setIsEditing] = useState(false);
  const [isLoading, setIsLoading] = useState(false);

  // Initialize form with empty values
  const methods = useForm<FormData>({
    defaultValues: {
      voucherName: "",
      startDate: "",
      endDate: "",
    }
  });

  // Reset form with voucher data when it's available
  useEffect(() => {
    if (voucher) {
      methods.reset({
        voucherName: voucher.voucherName || "",
        startDate: voucher.startDate || "",
        endDate: voucher.endDate || "",
        // Add other fields as needed
      });
    }
  }, [voucher, methods]);

  // Fetch voucher data when voucherId changes
  useEffect(() => {
    if (voucherId) {
      setIsLoading(true);
      dispatch(fetchVoucherById(voucherId))
        .finally(() => setIsLoading(false));
    }
  }, [dispatch, voucherId]);

  const handleSubmit = (value: FormData) => {
    const updatedVoucher = {
      ...voucher,
      ...value,
    };

    dispatch(updateVoucher({ id: voucherId, data: updatedVoucher }));
    setIsEditing(false);
    onClose();
  };

  return {
    isEditing,
    setIsEditing,
    isLoading,
    methods,
    handleSubmit,
  };
};