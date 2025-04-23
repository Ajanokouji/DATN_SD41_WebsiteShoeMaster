import React, { useState } from "react";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { X, Plus } from "lucide-react";

interface Option {
  id: string;
  parentId?: string | null; // Parent ID của nhóm phân loại
  name: string;
  values: { id: string; value: string; parentId: string | null }[]; // Giá trị có parentId
}

export const VariantForm = () => {
  const [options, setOptions] = useState<Option[]>([]);

  const handleAddOption = () => {
    if (options.length >= 2) return; // Chặn nếu đã đủ 2 nhóm
    setOptions([
      ...options,
      {
        id: crypto.randomUUID(),
        parentId: null, // Nhóm phân loại cha không có parentId
        name: "",
        values: [],
      },
    ]);
  };

  const handleRemoveOption = (id: string) => {
    const updated = options.filter((o) => o.id !== id);
    setOptions(updated);
  };

  const handleOptionNameChange = (id: string, name: string) => {
    setOptions(
      options.map((o) => (o.id === id ? { ...o, name } : o))
    );
  };

  const handleValueChange = (optionId: string, valueId: string, value: string) => {
    setOptions(
      options.map((o) =>
        o.id === optionId
          ? {
              ...o,
              values: o.values.map((v) =>
                v.id === valueId ? { ...v, value } : v
              ),
            }
          : o
      )
    );
  };

  const handleAddValue = (optionId: string) => {
    setOptions(
      options.map((o) =>
        o.id === optionId
          ? {
              ...o,
              values: [
                ...o.values,
                {
                  id: crypto.randomUUID(), // Tạo ID duy nhất cho giá trị
                  value: "",
                  parentId: optionId, // Gán parentId bằng id của nhóm phân loại cha
                },
              ],
            }
          : o
      )
    );
  };

  const handleRemoveValue = (optionId: string, valueId: string) => {
    setOptions(
      options.map((o) =>
        o.id === optionId
          ? {
              ...o,
              values: o.values.filter((v) => v.id !== valueId),
            }
          : o
      )
    );
  };

  // Tạo tổ hợp giá trị giữa các nhóm phân loại
  const generateCombinations = () => {
    
    if (options.length === 0) return [];
    if (options.length === 1)
      return options[0].values.map((v) => [v.value]);

    const [firstGroup, secondGroup] = options;
    const combinations: string[][] = [];

    firstGroup.values.forEach((val1) => {
      secondGroup.values.forEach((val2) => {
        combinations.push([val1.value, val2.value]);
      });
    });


    return combinations;
  };

  const combinations = generateCombinations();

  return (
    <div className="p-4 border rounded space-y-4">
      <h2 className="text-lg font-semibold">Phân loại hàng</h2>

      {options.length === 0 ? (
        <>
          <Button
            variant="outline"
            onClick={handleAddOption}
            className="border-dashed"
          >
            <Plus size={16} className="mr-2" />
            Tạo nhóm phân loại
          </Button>

          <div className="space-y-4 mt-4">
            <div>
              <label className="font-medium text-sm block mb-1">* Giá</label>
              <Input placeholder="₫ Nhập vào" />
            </div>
            <div>
              <label className="font-medium text-sm block mb-1">
                * Kho hàng <span className="text-gray-500">ⓘ</span>
              </label>
              <Input defaultValue="0" />
            </div>
          </div>
        </>
      ) : (
        <>
          {options.map((option) => (
            <div
              key={option.id}
              className="border p-3 rounded space-y-2 bg-gray-50"
            >
              <div className="flex items-center gap-2">
                <Input
                  placeholder="Tên phân loại"
                  value={option.name}
                  onChange={(e) =>
                    handleOptionNameChange(option.id, e.target.value)
                  }
                  className="w-1/3"
                />
                <Button
                  type="button"
                  onClick={() => handleRemoveOption(option.id)}
                  className="ml-auto text-gray-500"
                >
                  <X size={18} />
                </Button>
              </div>
              <div className="flex flex-wrap gap-2">
                {option.values.map((val) => (
                  <div key={val.id} className="relative">
                    <Input
                      value={val.value}
                      placeholder="Giá trị phân loại"
                      onChange={(e) =>
                        handleValueChange(option.id, val.id, e.target.value)
                      }
                      className="pr-6"
                    />
                    <Button
                      onClick={() => handleRemoveValue(option.id, val.id)}
                      className="absolute top-1 right-1 text-gray-500"
                      size="icon"
                      variant="ghost"
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
                  <Plus size={14} className="mr-1" />
                  Thêm giá trị
                </Button>
              </div>
            </div>
          ))}

          {options.length < 2 && (
            <Button type="button" onClick={handleAddOption}>
              + Thêm nhóm phân loại
            </Button>
          )}

          {/* Bảng hiển thị động */}
          <div className="mt-4 border rounded-lg p-4 space-y-2">
            <h3 className="font-semibold">Danh sách phân loại hàng</h3>
            <div
              className={`grid gap-4`}
              style={{
                gridTemplateColumns: `repeat(${options.length + 3}, minmax(0, 1fr))`,
              }}
            >
              {options.map((option, index) => (
                <div key={option.id}>{`Nhóm phân loại ${index + 1}`}</div>
              ))}
              <div>* Giá</div>
              <div>* Kho hàng</div>
              <div>SKU phân loại</div>
            </div>

            {combinations.map((combination, rowIndex) => (
              <div
                key={rowIndex}
                className={`grid gap-4`}
                style={{
                  gridTemplateColumns: `repeat(${options.length + 3}, minmax(0, 1fr))`,
                }}
              >
                {combination.map((value, colIndex) => (
                  <Input
                    key={colIndex}
                    placeholder={`Giá trị ${colIndex + 1}`}
                    value={value}
                    disabled
                  />
                ))}
                <Input placeholder="Nhập giá" />
                <Input placeholder="0" />
                <Input placeholder="Nhập SKU" />
              </div>
            ))}
          </div>
        </>
      )}
    </div>
  );
};