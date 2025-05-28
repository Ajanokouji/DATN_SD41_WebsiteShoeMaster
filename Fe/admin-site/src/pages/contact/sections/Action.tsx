import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Filter } from "lucide-react";
import { useState } from "react";
import { FaPlus } from "react-icons/fa6";
import AddContactSheet from "./FormAdd/AddContactSheet";

const ActionHeader = () => {
  const [isOpenAdd, setIsOpenAdd] = useState(false);

  const handleOpenDialogAdd = () => {
    setIsOpenAdd(true);
  };
  return (
    <section>
      <div className="grid grid-cols-2 justify-between">
        <div className="flex gap-2 items-center w-1/2">
          <Input type="text" placeholder="Filters" className="shadow-none" />
          <div className="border rounded-full p-1">
            <Filter className="text-xs" />
          </div>
        </div>
        <div className="flex gap-5 justify-end">
          <Button onClick={handleOpenDialogAdd}>
            <FaPlus />
            Thêm nhân viên
          </Button>
        </div>
      </div>

      {isOpenAdd && (
        <AddContactSheet
          isOpen={isOpenAdd}
          onClose={() => setIsOpenAdd(false)}
        />
      )}
    </section>
  );
};

export default ActionHeader;
