import React from "react";
import {
  Sheet,
  SheetContent,
  SheetHeader,
  SheetTitle,
  SheetDescription,
} from "@/components/ui/sheet";
import { useAppSelector } from "@/hooks/use-app-selector";
import { Button } from "@/components/ui/button";
import { selectVoucher } from "@/redux/apps/voucher/voucherSelector";
import { useVoucherForm } from "./use-voucher-form";
import { VoucherForm } from "./VoucherForm";
import { ImageFieldEditComponent } from "./ImageFieldEditComponent";

interface DetailVoucherSheetProps {
  voucherId: string;
  isOpen: boolean;
  onClose: () => void;
}

const DetailVoucherSheet: React.FC<DetailVoucherSheetProps> = ({
  voucherId,
  isOpen,
  onClose,
}) => {
  const voucher = useAppSelector(selectVoucher);
  const {
    isEditing,
    setIsEditing,
    methods,
    handleSubmit,
    isLoading,
    errorMessage,
  } = useVoucherForm(voucherId, voucher, onClose);

  return (
    <Sheet open={isOpen} onOpenChange={onClose}>
      <SheetContent className="w-[90%] sm:max-w-[80vw] max-w-none h-screen overflow-y-auto">
        <SheetHeader>
          {!isLoading && (
            <SheetTitle className="text-xl font-semibold text-gray-700">
              Chi tiết Voucher: {voucher?.voucherName}
            </SheetTitle>
          )}
          <SheetDescription />
        </SheetHeader>

        <VoucherForm
          methods={methods}
          isEditing={isEditing}
          onSubmit={handleSubmit}
          isLoading={isLoading}
          errorMessage={errorMessage}
        >
        <hr className="border border-gray-300"/>
        <br/>
        <ImageFieldEditComponent control={methods.control} isEditing={isEditing} />
        <br/>
        
          {!isEditing ? (
            <Button
              type="button"
              onClick={() => setIsEditing(true)}
            >
              Chỉnh sửa
            </Button>
          ) : (
            <div className="flex justify-start bottom-3 left-7">
              <Button type="submit">Lưu</Button>
              <Button
                type="button"
                variant="secondary"
                onClick={() => {
                  methods.reset();
                  setIsEditing(false);
                }}
              >
                Hủy
              </Button>
            </div>
          )}
        </VoucherForm>
      </SheetContent>
    </Sheet>
  );
};

export default DetailVoucherSheet;
