import { useEffect, useState } from "react";
import { useForm } from "react-hook-form";
import { useAppDispatch } from "@/hooks/use-app-dispatch";
import {
  fetchVoucherById,
  updateVoucher,
  checkVoucherCodeExist,
} from "@/redux/apps/voucher/voucherSlice";
import { zodResolver } from "@hookform/resolvers/zod";
import { VoucherFormSchema, voucherFormSchema } from "./FormSchema";

// Hàm chuyển đổi UTC thành giờ địa phương
const convertUtcToLocal = (utcString: string): string => {
  // Convert 'YYYY-MM-DD HH:mm:ss' => 'YYYY-MM-DDTHH:mm:ssZ' (UTC format)
  const isoString = utcString.replace(" ", "T") + "Z";
  const utcDate = new Date(isoString); // Parse as UTC

  // Convert to local date string for <input type="datetime-local" />
  const localDate = new Date(utcDate.getTime() - utcDate.getTimezoneOffset() * 60000);
  return localDate.toISOString().slice(0, 19); // 'YYYY-MM-DDTHH:mm:ss'
};

export const useVoucherForm = (
  voucherId: string,
  voucher: any,
  onClose: () => void
) => {
  const dispatch = useAppDispatch();
  const [isEditing, setIsEditing] = useState(false);
  const [isLoading, setIsLoading] = useState(false);

  // Initialize form with empty values
  const methods = useForm<VoucherFormSchema>({
    resolver: zodResolver(voucherFormSchema),
    mode: "onChange",
    reValidateMode: "onChange",
    defaultValues: {
      voucherName: "",
      startDate: "",
      endDate: "",
      status: 0,
      voucherType: 1,
      isdeleted: false,
      createdByUserId: "3fa85f64-5717-4562-b3fc-2c963f66afa6",
      lastModifiedByUserId: "3fa85f64-5717-4562-b3fc-2c963f66afa6",
      lastModifiedOnDate: new Date().toISOString(),
      createdOnDate: new Date().toISOString(),
      code: "",
      discountAmount: undefined,
      discountPercentage: undefined,
      description: "",
      minimumOrderAmount: undefined,
      discountType: "VNĐ",
      totalMaxUsage: undefined,
      maxUsagePerCustomer: undefined,
      maxDiscountAmount: undefined,
    }
  });

  // Reset form with voucher data when it's available
  useEffect(() => {
    if (voucher) {
      // Chuyển đổi thời gian UTC thành thời gian địa phương
      const startDateLocal = voucher.startDate ? convertUtcToLocal(voucher.startDate) : "";
      const endDateLocal = voucher.endDate ? convertUtcToLocal(voucher.endDate) : "";

      methods.reset({
        voucherName: voucher.voucherName || "",
        startDate: startDateLocal,
        endDate: endDateLocal,
        code: voucher.code || "",
        discountType: voucher.discountAmount ? "VNĐ" : "%",
        discountAmount: voucher.discountAmount || undefined,
        discountPercentage: voucher.discountPercentage || undefined,
        minimumOrderAmount: voucher.minimumOrderAmount || undefined,
        status: voucher.status || 0,
        description: voucher.description || "",
        voucherType: voucher.voucherType || 1,
        isdeleted: voucher.isdeleted || false,
        totalMaxUsage: voucher.totalMaxUsage || undefined,
        maxUsagePerCustomer: voucher.maxUsagePerCustomer || undefined,
        maxDiscountAmount: voucher.maxDiscountAmount || undefined,
        createdByUserId: voucher.createdByUserId || "3fa85f64-5717-4562-b3fc-2c963f66afa6",
        lastModifiedByUserId: voucher.lastModifiedByUserId || "3fa85f64-5717-4562-b3fc-2c963f66afa6",
        lastModifiedOnDate: voucher.lastModifiedOnDate || new Date().toISOString(),
        createdOnDate: voucher.createdOnDate || new Date().toISOString(),
        imageUrl: voucher.imageUrl || "",
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

  const [errorMessage, setErrorMessage] = useState<string | null>(null);
  const handleSubmit = async (value: VoucherFormSchema) => {
    try {
      //Kiểm tra voucher có tồn tại không
      const resultAction0 = await dispatch(fetchVoucherById(voucherId.trim()));
      if (fetchVoucherById.fulfilled.match(resultAction0)) {
        const voucherFound = resultAction0.payload;

        if (!voucherFound || (voucherFound && voucherFound.isDeleted)) {
          setErrorMessage("Voucher không tồn tại! Vui lòng kiểm tra lại!");
          return;
        }
      }
      else{
        console.error("Lỗi trong quá trình kiểm tra mã voucher:", resultAction0.error.message);
        return;
      }
      setErrorMessage(null);

      //Check code đã tồn tại chưa
      const resultAction = await dispatch(checkVoucherCodeExist({ code: value.code.trim(), voucherId: voucherId.trim() }));
      if (checkVoucherCodeExist.fulfilled.match(resultAction)) {
        const isExist = resultAction.payload;

        if (isExist) {
          methods.setError("code", {
            type: "manual",
            message: "Mã voucher đã tồn tại",
          });
          return;
        }
      }
      else{
        console.error("Lỗi trong quá trình kiểm tra mã voucher:", resultAction.error.message);
        return;
      }

      //Xác định chỉ có một trong hai discountAmount hoặc discountPercentage được nhập
      if (value.discountType === "VNĐ") {
        value.discountPercentage = undefined;
      } else {
        value.discountAmount = undefined;
      }

      console.log("Giá trị voucher sau khi sửa:", value);

      //Trim chuỗi
      value.voucherName = value.voucherName.trim();
      value.code = value.code.trim();
      value.description = value.description?.trim() || undefined;

      //Chuyển thành giờ UTC
      value.startDate = new Date(value.startDate).toISOString();
      value.endDate = new Date(value.endDate).toISOString();

      const updatedVoucher = {
        ...voucher,
        ...value,
      };

      var editRs = await dispatch(updateVoucher({ id: voucherId, data: updatedVoucher }));
      if (updateVoucher.fulfilled.match(editRs)) {
        onClose();
      }
    } catch (error) {
      console.error("Lỗi trong quá trình sửa voucher:", error);
    }
  };

  return {
    isEditing,
    setIsEditing,
    isLoading,
    methods,
    handleSubmit,
    errorMessage,
  };
};