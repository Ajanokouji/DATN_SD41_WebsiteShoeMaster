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
  } = useVoucherForm(voucherId, voucher, onClose);

  return (
    <Sheet open={isOpen} onOpenChange={onClose}>
      <SheetContent className="w-[90%] sm:max-w-[80vw] max-w-none h-screen overflow-y-auto">
        <SheetHeader>
          <SheetTitle className="text-xl font-semibold text-gray-700">
            Chi tiết Voucher: {voucher?.voucherName}
          </SheetTitle>
          <SheetDescription />
        </SheetHeader>

        <VoucherForm
          methods={methods}
          isEditing={isEditing}
          onSubmit={handleSubmit}
        >
          

          {!isEditing ? (
            <Button
              className="flex justify-start bottom-3 left-7"
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
