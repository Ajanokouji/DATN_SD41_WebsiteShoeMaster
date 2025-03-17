import { Input } from "@/components/ui/input";
import { Filter } from "lucide-react";

const ActionHeader = () => {
  return (
    <section>
      <div className="grid grid-cols-2 justify-between">
        <div className="flex gap-2 items-center w-1/2">
          <Input type="text" placeholder="Filters" className="shadow-none" />
          <div className="border rounded-full p-1">
            <Filter className="text-xs" />
          </div>
        </div>
      </div>
    </section>
  );
};

export default ActionHeader;
