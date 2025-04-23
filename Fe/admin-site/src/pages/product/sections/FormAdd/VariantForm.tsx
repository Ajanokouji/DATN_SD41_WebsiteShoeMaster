import React, { useState, useEffect } from "react";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { UseFormReturn } from "react-hook-form";
import { X, Plus } from "lucide-react";
import { ProductFormSchema } from "./FormSchema";

interface Option {
  id: string;
  parentId?: string | null;
  name: string;
  values: { id: string; value: string; parentId: string | null }[];
}

interface VariantFormProps {
  form: UseFormReturn<ProductFormSchema>;
}

interface CombinationData {
  group1: string;
  group2: string;
  price: string;
  stock: string;
  sku: string;
}

export const VariantForm: React.FC<VariantFormProps> = ({ form }) => {
  const [options, setOptions] = useState<Option[]>([]);
  const [isColorGroupDisabled, setIsColorGroupDisabled] = useState(false);
  const [isSizeGroupDisabled, setIsSizeGroupDisabled] = useState(false);
  const [showDefaultInputs, setShowDefaultInputs] = useState(true);
  const [combinationData, setCombinationData] = useState<CombinationData[]>([]);

  const areValuesEmpty = () => {
    return options.every((option) => option.values.length === 0);
  };

  const isAnyButtonDisabled = () => {
    return isColorGroupDisabled || isSizeGroupDisabled;
  };

  useEffect(() => {
    setShowDefaultInputs(areValuesEmpty() && !isAnyButtonDisabled());
  }, [options, isColorGroupDisabled, isSizeGroupDisabled]);

  const handleAddGroup = (name: string, disableCallback: () => void) => {
    if (options.find((o) => o.name === name)) return;
    setOptions([
      ...options,
      { id: crypto.randomUUID(), parentId: null, name, values: [] },
    ]);
    disableCallback();
  };

  const handleRemoveOption = (id: string) => {
    const option = options.find((o) => o.id === id);
    if (option?.name === "Màu sắc") setIsColorGroupDisabled(false);
    if (option?.name === "Kích cỡ") setIsSizeGroupDisabled(false);
    setOptions(options.filter((o) => o.id !== id));
  };

  const handleValueChange = (optionId: string, valueId: string, value: string) => {
    setOptions(options.map((o) =>
      o.id === optionId
        ? {
            ...o,
            values: o.values.map((v) =>
              v.id === valueId ? { ...v, value } : v
            ),
          }
        : o
    ));
  };

  const handleAddValue = (optionId: string) => {
    setOptions(options.map((o) =>
      o.id === optionId
        ? {
            ...o,
            values: [
              ...o.values,
              {
                id: crypto.randomUUID(),
                value: "",
                parentId: optionId,
              },
            ],
          }
        : o
    ));
  };

  const handleRemoveValue = (optionId: string, valueId: string) => {
    setOptions(options.map((o) =>
      o.id === optionId
        ? { ...o, values: o.values.filter((v) => v.id !== valueId) }
        : o
    ));
  };

  const generateCombinations = (): CombinationData[] => {
    const colorOption = options.find((o) => o.name === "Màu sắc");
    const sizeOption = options.find((o) => o.name === "Kích cỡ");

    const colorValues = colorOption?.values ?? [];
    const sizeValues = sizeOption?.values ?? [];

    if (colorValues.length === 0 && sizeValues.length === 0) return [];

    if (colorValues.length > 0 && sizeValues.length === 0) {
      return colorValues.map((v) => ({
        group1: v.value,
        group2: "",
        price: "",
        stock: "",
        sku: "",
      }));
    }

    if (colorValues.length === 0 && sizeValues.length > 0) {
      return sizeValues.map((v) => ({
        group1: "",
        group2: v.value,
        price: "",
        stock: "",
        sku: "",
      }));
    }

    const combinations: CombinationData[] = [];
    colorValues.forEach((color) => {
      sizeValues.forEach((size) => {
        combinations.push({
          group1: color.value,
          group2: size.value,
          price: "",
          stock: "",
          sku: "",
        });
      });
    });

    return combinations;
  };

  useEffect(() => {
    const combinations = generateCombinations();
    setCombinationData(combinations);
    form.setValue("variantObjs", combinations);
  }, [options]);

  const handleCombinationChange = (index: number, field: keyof CombinationData, value: string) => {
    const updated = [...combinationData];
    updated[index][field] = value;
    setCombinationData(updated);
    form.setValue("variantObjs", updated);
  };

  const group1RowSpans: { [key: string]: number } = {};
  combinationData.forEach((c) => {
    group1RowSpans[c.group1] = (group1RowSpans[c.group1] || 0) + 1;
  });

  return (
    <div className="p-4 border rounded space-y-4">
      <h2 className="text-lg font-semibold">Phân loại hàng</h2>

      <div className="flex gap-4">
        <Button type="button" onClick={() => handleAddGroup("Màu sắc", () => setIsColorGroupDisabled(true))} disabled={isColorGroupDisabled} variant="outline">
          <Plus size={16} className="mr-2" />Tạo nhóm phân loại Màu sắc
        </Button>
        <Button type="button" onClick={() => handleAddGroup("Kích cỡ", () => setIsSizeGroupDisabled(true))} disabled={isSizeGroupDisabled} variant="outline">
          <Plus size={16} className="mr-2" />Tạo nhóm phân loại Kích cỡ
        </Button>
      </div>

      {options.map((option) => (
        <div key={option.id} className="border p-3 rounded bg-gray-50 mt-4 space-y-2">
          <div className="flex items-center gap-2">
            <Input value={option.name} disabled className="w-1/3" />
            <Button type="button" onClick={() => handleRemoveOption(option.id)} variant="ghost" size="icon" className="ml-auto text-gray-500">
              <X size={18} />
            </Button>
          </div>
          <div className="flex flex-wrap gap-2">
            {option.values.map((val) => (
              <div key={val.id} className="relative">
                <Input
                  value={val.value}
                  placeholder="Giá trị phân loại"
                  onChange={(e) => handleValueChange(option.id, val.id, e.target.value)}
                  className="pr-6"
                />
                <Button type="button" onClick={() => handleRemoveValue(option.id, val.id)} size="icon" variant="ghost" className="absolute top-1 right-1 text-gray-500">
                  <X size={12} />
                </Button>
              </div>
            ))}
            <Button type="button" size="sm" variant="ghost" onClick={() => handleAddValue(option.id)}>
              <Plus size={14} className="mr-1" />Thêm giá trị
            </Button>
          </div>
        </div>
      ))}

      {showDefaultInputs && (
        <div className="mt-4 space-y-2">
          <Input placeholder="Nhập giá trị mặc định 1" />
          <Input placeholder="Nhập giá trị mặc định 2" />
        </div>
      )}

      {combinationData.length > 0 && (
        <div className="mt-4 border rounded-lg p-4 space-y-2">
          <h3 className="font-semibold mb-2">Danh sách phân loại hàng</h3>
          <table className="w-full border-collapse">
            <thead>
              <tr className="bg-gray-100">
                <th className="border p-2 text-left">Màu sắc</th>
                <th className="border p-2 text-left">Kích cỡ</th>
                <th className="border p-2 text-left">Giá</th>
                <th className="border p-2 text-left">Kho hàng</th>
                <th className="border p-2 text-left">SKU phân loại</th>
              </tr>
            </thead>
            <tbody>
              {combinationData.map((combination, index) => {
                const isFirst = index === 0 || combination.group1 !== combinationData[index - 1].group1;
                const rowSpan = group1RowSpans[combination.group1];

                return (
                  <tr key={index}>
                    {isFirst && (
                      <td className="border p-2 align-middle" rowSpan={rowSpan}>
                        {combination.group1}
                      </td>
                    )}
                    <td className="border p-2">{combination.group2}</td>
                    <td className="border p-2">
                      <Input value={combination.price} onChange={(e) => handleCombinationChange(index, "price", e.target.value)} placeholder="Giá" />
                    </td>
                    <td className="border p-2">
                      <Input value={combination.stock} onChange={(e) => handleCombinationChange(index, "stock", e.target.value)} placeholder="Kho" />
                    </td>
                    <td className="border p-2">
                      <Input value={combination.sku} onChange={(e) => handleCombinationChange(index, "sku", e.target.value)} placeholder="SKU" />
                    </td>
                  </tr>
                );
              })}
            </tbody>
          </table>
        </div>
      )}
    </div>
  );
};
