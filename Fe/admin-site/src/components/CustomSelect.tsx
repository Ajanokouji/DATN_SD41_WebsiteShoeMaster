import {
  Popover,
  PopoverContent,
  PopoverTrigger,
} from "@/components/ui/popover";
import { Button } from "@/components/ui/button";
import React, { useState } from "react";
import { Check, ChevronDown, Plus } from "lucide-react";
// import { useAppDispatch } from "@/hooks/use-app-dispatch";
// import {
//   createCustomerSource,
//   createCustomerStatus,
// } from "@/redux/apps/customer/customerSlice";

interface CustomSelectProps<T extends string | number> {
  options: { title: string; value: T }[];
  label?: React.ReactNode;
  onChange: (value: T) => void;
}

const CustomSelect = <T extends string | number>({
  options: initialOptions,
  label,
  onChange,
}: CustomSelectProps<T>) => {
  // const dispatch = useAppDispatch();

  const [options, setOptions] = useState(initialOptions);
  const [selectedValue, setSelectedValue] = useState<T>(
    initialOptions[0]?.value || ("" as T)
  );
  const [newOption, setNewOption] = useState("");

  const handleOptionClick = (value: T) => {
    setSelectedValue(value);
    onChange(value);
  };

  const handleAddOption = async () => {
    if (newOption.trim() !== "") {
      let newOptionValue: T;

      if (typeof selectedValue === "number") {
        const parsedValue = parseFloat(newOption);
        if (!isNaN(parsedValue)) {
          newOptionValue = parsedValue as T;
        } else {
          return;
        }
      } else {
        newOptionValue = newOption as T;
      }

      // const titleValue = newOptionValue.toString();

      try {
        if (React.isValidElement(label)) {
          const labelText = label.props.children as string;

          if (labelText.includes("Nguồn khách hàng")) {
            // await dispatch(createCustomerSource({ title: titleValue }));
            console.log("createCustomerSource");
          } else if (labelText.includes("Trạng thái")) {
            // await dispatch(createCustomerStatus({ title: titleValue }));
            console.log("createCustomerStatus");
          }
        }

        setOptions((prev) => [
          ...prev,
          { title: newOption, value: newOptionValue },
        ]);
        setSelectedValue(newOptionValue);
        onChange(newOptionValue);
        setNewOption(""); 
      } catch (error) {
        console.error("Error creating option:", error);
      }
    }
  };

  return (
    <div className="grid grid-cols-1">
      {label && <label className="mb-1 text-sm text-gray-400">{label}</label>}
      <Popover>
        <PopoverTrigger asChild>
          <Button
            variant="outline"
            className="justify-between overflow-hidden px-4"
          >
            {options.find((option) => option.value === selectedValue)?.title}
            <ChevronDown size={18} />
          </Button>
        </PopoverTrigger>
        <PopoverContent className="p-2 w-60">
          <ul className="space-y-1">
            {options.map((option) => (
              <li
                key={option.value.toString()}
                className={`p-2 cursor-pointer hover:bg-gray-100 rounded flex items-center ${
                  selectedValue === option.value ? "bg-gray-100" : ""
                }`}
                onClick={() => handleOptionClick(option.value)}
              >
                {selectedValue === option.value && (
                  <Check size={16} className="mr-2 text-blue-500" />
                )}
                {option.title}
              </li>
            ))}
          </ul>
          <div className="grid grid-cols-3 items-center gap-2 border-t pt-2">
            <input
              type="text"
              value={newOption}
              onChange={(e) => setNewOption(e.target.value)}
              placeholder="Nhập dữ liệu"
              className="px-2 py-1 text-sm border rounded focus:outline-none col-span-2"
            />
            <Button
              type="button"
              variant="ghost"
              onClick={handleAddOption}
              className="col-span-1 flex items-end gap-1 text-sm"
            >
              <Plus size={16} /> Thêm
            </Button>
          </div>
        </PopoverContent>
      </Popover>
    </div>
  );
};

export default CustomSelect;
