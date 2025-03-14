import { useEffect, useState } from "react";
import { useForm } from "react-hook-form";
import { useAppDispatch } from "@/hooks/use-app-dispatch";
import {
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

  const methods = useForm<FormData>({
    defaultValues: {
      voucherName: "",
      startDate: "",
      endDate: "",
    }
  });

  const { reset } = methods;

  // Update form values when voucher data changes
  useEffect(() => {
    if (voucher) {
      reset({
        voucherName: voucher.voucherName || "",
        startDate: voucher.startDate || "",
        endDate: voucher.endDate || "",
        // Add other fields as needed
      });
    }
  }, [voucher, reset]);

  const handleSubmit = (formData: FormData) => {
    const updatedVoucher = {
      ...voucher,
      ...formData,
    };

    dispatch(updateVoucher({ id: voucherId, data: updatedVoucher }));
    setIsEditing(false);
    onClose();
  };

  const resetForm = () => {
    if (voucher) {
      reset({
        voucherName: voucher.voucherName || "",
        startDate: voucher.startDate || "",
        endDate: voucher.endDate || "",
        // Add other fields as needed
      });
    }
  };

  return {
    isEditing,
    setIsEditing,
    methods,
    handleSubmit,
    resetForm,
  };
};