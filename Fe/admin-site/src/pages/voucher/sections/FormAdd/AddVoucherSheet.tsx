import { Button } from "@/components/ui/button";
import { Form } from "@/components/ui/form";
import React, { useState } from "react";
import { useAppDispatch } from "@/hooks/use-app-dispatch";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import {
  Sheet,
  SheetContent,
  SheetDescription,
  SheetHeader,
  SheetTitle,
} from "@/components/ui/sheet";
import { createVoucher, checkVoucherCodeExist } from "@/redux/apps/voucher/voucherSlice";
import { voucherFormSchema, VoucherFormSchema } from "./FormSchema";
import VoucherReqDto from "@/types/voucher/voucher";
import { BasicInfoFields } from "./BasicFields";

interface AddVoucherSheetProps {
  isOpen: boolean;
  onClose: () => void;
  voucherType: number;
}

const AddVoucherSheet: React.FC<AddVoucherSheetProps> = ({
  isOpen,
  onClose,
  voucherType,
}) => {
  const dispatch = useAppDispatch();
  const [isSubmitting, setIsSubmitting] = useState(false);

  const form = useForm<VoucherFormSchema>({
    resolver: zodResolver(voucherFormSchema),
    mode: "onChange",
    reValidateMode: "onChange",
    defaultValues: {
      voucherName: "",
      startDate: "",
      endDate: "",
      status: 0,
      voucherType: voucherType,
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
    },
  });

  const handleSubmit = async (values: VoucherFormSchema) => {
    setIsSubmitting(true);
    try {
      // Kiểm tra mã voucher có tồn tại hay không
      const resultAction = await dispatch(checkVoucherCodeExist({ code: values.code.trim(), voucherId: undefined }));
      if (checkVoucherCodeExist.fulfilled.match(resultAction)) {
        const isExist = resultAction.payload;

        if (isExist) {
          form.setError("code", {
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
      if (values.discountType === "$") {
        values.discountPercentage = undefined;
      } else {
        values.discountAmount = undefined;
      }

      const voucherData: VoucherReqDto = {
        ...values,
        discountAmount: values.discountAmount ?? null,
        discountPercentage: values.discountPercentage ?? null,
        minimumOrderAmount: values.minimumOrderAmount ?? null,
        description: values.description ?? null,
        createdByUserId: values.createdByUserId || "",
        lastModifiedByUserId: values.lastModifiedByUserId || "",
        lastModifiedOnDate: values.lastModifiedOnDate || new Date().toISOString(),
        createdOnDate: values.createdOnDate || new Date().toISOString(),
      };

      //Trim chuỗi
      voucherData.voucherName = values.voucherName.trim();
      voucherData.code = values.code.trim();
      voucherData.description = values.description?.trim() || null;

      // Chuyển thành giờ UTC
      if (values.startDate) {
        voucherData.startDate = new Date(values.startDate).toISOString();
      }
      if (values.endDate) {
        voucherData.endDate = new Date(values.endDate).toISOString();
      }

      var createRs = await dispatch(createVoucher(voucherData));
      if (createVoucher.fulfilled.match(createRs)) {
        onClose();
      }
    } catch (error) {
      console.error("Lỗi trong quá trình thêm voucher:", error);
    } finally {
      setIsSubmitting(false);
    }
  };

  return (
    <Sheet open={isOpen} onOpenChange={onClose}>
      <SheetContent className="w-[90%] sm:max-w-[80vw] max-w-none h-screen overflow-y-auto">
        <SheetHeader>
          <SheetTitle className="text-xl font-semibold text-gray-700">
            {voucherType === 1 ? "Tạo Voucher Toàn Shop" : "Tạo Voucher Sản Phẩm"}
          </SheetTitle>
          <SheetDescription>
            {voucherType === 1
            ? "Áp dụng cho tất cả sản phẩm trong Shop của bạn."
            : "Áp dụng cho những sản phẩm nhất định mà Shop chọn."}
          </SheetDescription>
        </SheetHeader>
        <br />
        <Form {...form}>
          <form
            onSubmit={form.handleSubmit(handleSubmit)}
            className="space-y-6 overflow-auto"
          >
            <BasicInfoFields control={form.control} />

            <div className="flex justify-end gap-2 pt-4">
              <Button type="submit" disabled={isSubmitting}>
                {isSubmitting ? "Đang tạo..." : "Tạo"}
              </Button>
              <Button variant="outline" onClick={onClose} type="button">
                Thoát
              </Button>
            </div>
          </form>
        </Form>
      </SheetContent>
    </Sheet>
  );
};

export default AddVoucherSheet;
