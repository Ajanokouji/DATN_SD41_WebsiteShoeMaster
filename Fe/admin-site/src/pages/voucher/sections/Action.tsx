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
      <Button onClick={() => handleOpenDialogAdd(1)} className="mt-4 px-4 py-2">
        <FaPlus />
        Tạo
      </Button>
      <br/>
      <br/>
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