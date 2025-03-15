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
import { createVoucher } from "@/redux/apps/voucher/voucherSlice";
import { voucherFormSchema, VoucherFormSchema } from "./FormSchema";
import VoucherReqDto from "@/types/voucher/voucher";
import { BasicInfoFields } from "./BasicFields";

interface AddVoucherSheetProps {
  isOpen: boolean;
  onClose: () => void;
}

const AddVoucherSheet: React.FC<AddVoucherSheetProps> = ({
  isOpen,
  onClose,
}) => {
  const dispatch = useAppDispatch();
  const [isSubmitting, setIsSubmitting] = useState(false);

  const form = useForm<VoucherFormSchema>({
    resolver: zodResolver(voucherFormSchema),
    defaultValues: {
      voucherName: "",
      startDate: "",
      endDate: "",
      status: 0,
      voucherType: 0,
      isdeleted: false,
      createdByUserId: "3fa85f64-5717-4562-b3fc-2c963f66afa6",
      lastModifiedByUserId: "3fa85f64-5717-4562-b3fc-2c963f66afa6",
      lastModifiedOnDate: new Date().toISOString(),
      createdOnDate: new Date().toISOString(),
    },
  });
  console.log(form.formState.errors);

  const handleSubmit = async (values: VoucherFormSchema) => {
    setIsSubmitting(true);
    try {
      const voucherData: VoucherReqDto = {
        ...values,
        createdByUserId: values.createdByUserId || "",
        lastModifiedByUserId: values.lastModifiedByUserId || "",
        lastModifiedOnDate:
          values.lastModifiedOnDate || new Date().toISOString(),
        createdOnDate: values.createdOnDate || new Date().toISOString(),
      };

      await dispatch(createVoucher(voucherData));

      onClose();
    } catch (error) {
      console.error("Create voucher error details:", error);
    } finally {
      setIsSubmitting(false);
    }
  };

  return (
    <Sheet open={isOpen} onOpenChange={onClose}>
      <SheetContent className="w-[90%] sm:max-w-[80vw] max-w-none h-screen overflow-y-auto">
        <SheetHeader>
          <SheetTitle className="text-xl font-semibold text-gray-700">
            Add Voucher
          </SheetTitle>
          <SheetDescription>
            Create a new voucher with custom metadata fields
          </SheetDescription>
        </SheetHeader>

        <Form {...form}>
          <form
            onSubmit={form.handleSubmit(handleSubmit)}
            className="space-y-6"
          >
            <BasicInfoFields control={form.control} />

            <div className="flex justify-end gap-2 absolute bottom-4 left-0 w-full px-6">
              <Button type="submit" disabled={isSubmitting}>
                {isSubmitting ? "Creating..." : "Add new voucher"}
              </Button>
              <Button variant="outline" onClick={onClose} type="button">
                Cancel
              </Button>
            </div>
          </form>
        </Form>
      </SheetContent>
    </Sheet>
  );
};

export default AddVoucherSheet;
