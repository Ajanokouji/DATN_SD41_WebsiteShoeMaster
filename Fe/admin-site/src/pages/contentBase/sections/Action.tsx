import { Button } from "@/components/ui/button";
import { FaPlus } from "react-icons/fa6";
import AddContentBaseSheet from "./FormAdd/AddContentBaseSheet";
import { useState } from "react";

const ActionHeader = () => {
  const [isOpenAdd, setIsOpenAdd] = useState(false);

  return (
    <section>
      <h2 className="text-2xl mb-6">Tạo Content Base</h2>
      <Button onClick={() => setIsOpenAdd(true)} className="mt-4 px-4 py-2">
        <FaPlus />
        Tạo
      </Button>
      {isOpenAdd && (
        <AddContentBaseSheet
          isOpen={isOpenAdd}
          onClose={() => setIsOpenAdd(false)}
        />
      )}
    </section>
  );
};

export default ActionHeader;