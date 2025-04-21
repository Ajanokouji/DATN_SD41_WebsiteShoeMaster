import React, { useState } from "react";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { X, Plus } from "lucide-react";

interface Option {
  id: number;
  name: string;
  values: string[];
}

export const VariantForm = () => {
  const [options, setOptions] = useState<Option[]>([{
    id: 1,
    name: "",
    values: [""],
  }]);

  const handleAddOption = () => {
    setOptions([...options, { id: Date.now(), name: "", values: [""] }]);
  };

  const handleRemoveOption = (id: number) => {
    setOptions(options.filter((o) => o.id !== id));
  };

  const handleOptionNameChange = (id: number, name: string) => {
    setOptions(
      options.map((o) => (o.id === id ? { ...o, name } : o))
    );
  };

  const handleValueChange = (optionId: number, valueIndex: number, value: string) => {
    setOptions(
      options.map((o) =>
        o.id === optionId
          ? {
              ...o,
              values: o.values.map((v, i) => (i === valueIndex ? value : v))
            }
          : o
      )
    );
  };

  const handleAddValue = (optionId: number) => {
    setOptions(
      options.map((o) =>
        o.id === optionId ? { ...o, values: [...o.values, ""] } : o
      )
    );
  };

  const handleRemoveValue = (optionId: number, index: number) => {
    setOptions(
      options.map((o) =>
        o.id === optionId
          ? { ...o, values: o.values.filter((_, i) => i !== index) }
          : o
      )
    );
  };

  return (
    <div className="p-4 border rounded space-y-4">
      <h2 className="text-lg font-semibold">Thông tin bán hàng</h2>
      <div className="space-y-4">
        {options.map((option) => (
          <div
            key={option.id}
            className="border p-3 rounded space-y-2 bg-gray-50"
          >
            <div className="flex items-center gap-2">
              <Input
                placeholder="Tên phân loại"
                value={option.name}
                onChange={(e) => handleOptionNameChange(option.id, e.target.value)}
                className="w-1/3"
              />
              <Button
                variant="destructive"
                size="icon"
                onClick={() => handleRemoveOption(option.id)}
              >
                <X size={16} />
              </Button>
            </div>
            <div className="flex flex-wrap gap-2">
              {option.values.map((val, i) => (
                <div key={i} className="relative">
                  <Input
                    value={val}
                    placeholder="Giá trị phân loại"
                    onChange={(e) => handleValueChange(option.id, i, e.target.value)}
                    className="pr-6"
                  />

                  <Button
                    onClick={() => handleRemoveValue(option.id, i)}
                    className="absolute top-1 right-1 text-gray-500"
                  >
                    <X size={12} />
                  </Button>
                </div>
              ))}
              
              <Button
                type="button"
                size="sm"
                variant="ghost"
                onClick={() => handleAddValue(option.id)}
              >
                <Plus size={14} /> Thêm giá trị
              </Button>
            </div>
          </div>
        ))}
        <Button type="button" onClick={handleAddOption}>
          + Thêm nhóm phân loại
        </Button>
      </div>
    </div>
  );
};

 