import { Button } from "@/components/ui/button";
import { Form } from "@/components/ui/form";
import React, { useEffect, useState } from "react";
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
import VoucherProductReqDto from "@/types/voucherProduct/voucherProduct";
import { createVoucherProduct } from "@/redux/apps/voucherProduct/voucherProductSlice"
import { v4 as uuidv4 } from "uuid";
import { VariantObjs } from "@/types/product/product";
import VoucherUserReqDto from "@/types/voucherUser/voucherUser";
import { createVoucherUsers } from "@/redux/apps/voucherUser/voucherUserSlice";
import { clearUsers, setUserPage, setUserPageSize } from "@/redux/apps/voucherUser/voucherUserSlice";
import { ImageFieldComponent } from "./ImageFieldComponent";

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
  
  useEffect(() => {
      if (isOpen) {
        setUserPage(0);
        setUserPageSize(20);
        dispatch(clearUsers()); // Xóa danh sách products khi form được mở
      }
    }, [isOpen, dispatch]);

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
      isDeleted: false,
      createdByUserId: "3fa85f64-5717-4562-b3fc-2c963f66afa6",
      lastModifiedByUserId: "3fa85f64-5717-4562-b3fc-2c963f66afa6",
      lastModifiedOnDate: new Date().toISOString(),
      createdOnDate: new Date().toISOString(),
      code: "",
      discountAmount: undefined,
      discountPercentage: undefined,
      description: "",
      minimumOrderAmount: undefined,
      totalMaxUsage: undefined,
      maxUsagePerCustomer: undefined,
      maxDiscountAmount: undefined,
      redeemCount: undefined,
      productsIsSelected: null,
      usersIsSelected: null,
      imageUrl: null, // Thêm trường imageUrl vào defaultValues
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
          form.setFocus("code");
          return;
        }
      }
      else {
        console.error("Lỗi trong quá trình kiểm tra mã voucher:", resultAction.error.message);
        return;
      }

      //Xác định chỉ có một trong hai discountAmount hoặc discountPercentage được nhập
      if (values.discountType === "VNĐ") {
        values.discountPercentage = undefined;
        values.maxDiscountAmount = undefined;
      } else {
        values.discountAmount = undefined;
      }

      const voucherData: VoucherReqDto = {
        ...values,
        id: uuidv4(),
        discountAmount: values.discountAmount ?? null,
        discountPercentage: values.discountPercentage ?? null,
        minimumOrderAmount: values.minimumOrderAmount ?? null,
        description: values.description ?? null,
        createdByUserId: values.createdByUserId || "",
        lastModifiedByUserId: values.lastModifiedByUserId || "",
        lastModifiedOnDate: values.lastModifiedOnDate || new Date().toISOString(),
        createdOnDate: values.createdOnDate || new Date().toISOString(),
        totalMaxUsage: values.totalMaxUsage ?? null,
        maxUsagePerCustomer: values.maxUsagePerCustomer ?? null,
        maxDiscountAmount: values.maxDiscountAmount ?? null,
        redeemCount: 0,  //RedeemCount mặc định lúc mới tạo là = 0
        imageUrl: values.imageUrl ?? null,
      };

      //Trim chuỗi
      voucherData.voucherName = values.voucherName.trim();
      voucherData.code = values.code.trim();
      voucherData.description = values.description?.trim() || null;

      // Chuyển thành giờ UTC
      if (values.startDate) {
        voucherData.startDate = new Date(values.startDate.replace("T", " ") + ":00").toISOString();
      }
      if (values.endDate) {
        voucherData.endDate = new Date(values.endDate.replace("T", " ") + ":00").toISOString();
      }

      const createRs = await dispatch(createVoucher(voucherData));
      if (createVoucher.fulfilled.match(createRs)) {
        ////Thêm VoucherUser
        if(values.usersIsSelected !== null &&
          values.usersIsSelected.length > 0
        ){
          // Map usersIsSelected thành VoucherUserReqDto
          const voucherUsers: VoucherUserReqDto[] = values.usersIsSelected.map((user) => ({
            voucherId: voucherData.id,
            userId: user.id,
            createdByUserId: "3fa85f64-5717-4562-b3fc-2c963f66afa6", // GUID hợp lệ
            lastModifiedByUserId: "3fa85f64-5717-4562-b3fc-2c963f66afa6", // GUID hợp lệ
            lastModifiedOnDate: new Date().toISOString(), // Giá trị hợp lệ
            createdOnDate: new Date().toISOString(), // Giá trị hợp lệ
            isUsed: false, // Mặc định là chưa sử dụng
          }));

          // Gửi danh sách voucherUsers lên API
          await dispatch(createVoucherUsers(voucherUsers));
        }

        ////Thêm voucherProduct
        if (
          voucherData.voucherType === 2 &&
          values.productsIsSelected !== null &&
          values.productsIsSelected.length > 0
        ) {
          // Map voucherProducts
          const voucherProducts: VoucherProductReqDto[] = values.productsIsSelected.flatMap((product) =>
            product.variantObjs.map((variant: VariantObjs) => ({
              voucherId: voucherData.id,
              productId: product.id,
              varientProductId: variant.sku,
              createdByUserId: "3fa85f64-5717-4562-b3fc-2c963f66afa6",
              lastModifiedByUserId: "3fa85f64-5717-4562-b3fc-2c963f66afa6",
              lastModifiedOnDate: new Date().toISOString(),
              createdOnDate: new Date().toISOString(),
            })) 
          );
        
          // Create voucherProduct
          await dispatch(createVoucherProduct(voucherProducts));
        }

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
        <SheetHeader className="sr-only">
          <SheetTitle></SheetTitle>
          <SheetDescription></SheetDescription>
        </SheetHeader>
        <br />
        <Form {...form}>
          <form
            onSubmit={form.handleSubmit(handleSubmit)}
            className="space-y-6 overflow-auto"
          >
            <BasicInfoFields control={form.control} voucherType={voucherType} />
            <hr className="border border-gray-300"/>
            <ImageFieldComponent control={form.control} />

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
