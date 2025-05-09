import { Button } from "@/components/ui/button";
import { useState } from "react";
import { FaPlus } from "react-icons/fa6";
import AddVoucherSheet from "./FormAdd/AddVoucherSheet";

const ActionHeader = () => {
  const [isOpenAdd, setIsOpenAdd] = useState(false);
  const [voucherType, setVoucherType] = useState<number>(1);

  const handleOpenDialogAdd = (type: number) => {
    setVoucherType(type);
    setIsOpenAdd(true);
  };

  return (
    <section>
      <h2 className="text-2xl mb-6">Tạo Voucher</h2>

      <div className="grid grid-cols-4 gap-4 mb-8">
        <div className="p-4 border rounded-lg shadow-sm">
          <h3 className="font-bold mb-2">Voucher toàn Shop</h3>
          <p className="text-sm text-gray-600">
            Áp dụng cho tất cả sản phẩm trong Shop của bạn.
          </p>
          <Button onClick={() => handleOpenDialogAdd(1)} className="mt-4 px-4 py-2">
            <FaPlus />
            Tạo
          </Button>
        </div>

        <div className="p-4 border rounded-lg shadow-sm">
          <h3 className="font-bold mb-2">Voucher sản phẩm</h3>
          <p className="text-sm text-gray-600">
            Áp dụng cho những sản phẩm nhất định mà Shop chọn.
          </p>
          <Button onClick={() => handleOpenDialogAdd(2)} className="mt-4 px-4 py-2">
            <FaPlus />
            Tạo
          </Button>
        </div>
      </div>

      {isOpenAdd && (
        <AddVoucherSheet
          isOpen={isOpenAdd}
          onClose={() => setIsOpenAdd(false)}
          voucherType={voucherType}
        />
      )}
    </section>
  );
};

export default ActionHeader;