import React, { useState, useEffect } from "react";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { UseFormReturn } from "react-hook-form";
import { X, Plus } from "lucide-react";
import { ProductFormSchema } from "./FormSchema";

import {ImageIconSelector} from "./ImageIconSelectorProps"
interface Option {
  id: string;
  parentId?: string | null;
  name: string;
  values: { id: string; value: string; parentId: string | null }[];
}

interface VariantFormProps {
  form: UseFormReturn<ProductFormSchema>;
  initialVariants?: CombinationData[];
  mode?: "create" | "edit";
}


const validateSku = (sku: string): string | null => {
  if (!sku) return "SKU không được để trống";
  if (!/^[A-Z0-9-]{3,20}$/.test(sku)) 
    return "SKU chỉ gồm chữ hoa, số, dấu '-', dài 3-20 ký tự";
  return null;
};

interface CombinationData {
  group1: string;
  group2: string;
  price: string;
  stock: number;
  sku: string;
  imgUrl?: string;
}

// 👉 Hàm chuẩn hoá: bỏ dấu tiếng Việt + ký tự đặc biệt, trả về UPPER-CASE slug
const normalizeText = (txt: string) =>
  txt
    .trim()
    .toUpperCase()
    .normalize("NFD")
    .replace(/\p{Diacritic}/gu, "")
    .replace(/[^A-Z0-9]+/g, "-")
    .replace(/(^-|-$)/g, "");

export const VariantForm: React.FC<VariantFormProps> = ({ form }) => {
  const [options, setOptions] = useState<Option[]>([]);
  const [isColorGroupDisabled, setIsColorGroupDisabled] = useState(false);
  const [isSizeGroupDisabled, setIsSizeGroupDisabled] = useState(false);
  const [showDefaultInputs, setShowDefaultInputs] = useState(true);

  const [combinationData, setCombinationData] = useState<CombinationData[]>([]);
  const [skuErrors, setSkuErrors] = useState<Record<number, string>>({});
  const [duplicateErrors, setDuplicateErrors] = useState<Record<string, string>>({});

  const areValuesEmpty = () => options.every((o) => o.values.length === 0);
  const isAnyButtonDisabled = () => isColorGroupDisabled || isSizeGroupDisabled;

  useEffect(() => {
    setShowDefaultInputs(areValuesEmpty() && !isAnyButtonDisabled());
  }, [options, isColorGroupDisabled, isSizeGroupDisabled]);

 
  const handleAddGroup = (name: string, disable: () => void) => {
    if (options.some((o) => o.name === name)) return;
    setOptions((prev) => [
      ...prev,
      { id: crypto.randomUUID(), parentId: null, name, values: [] },
    ]);
    disable();
  };

  const handleRemoveOption = (id: string) => {
    const opt = options.find((o) => o.id === id);
    if (opt?.name === "Màu sắc") setIsColorGroupDisabled(false);
    if (opt?.name === "Kích cỡ") setIsSizeGroupDisabled(false);
    setOptions((prev) => prev.filter((o) => o.id !== id));
  };

  const checkDuplicateValue = (optionId: string, value: string, currentValueId: string): boolean => {
    const option = options.find(o => o.id === optionId);
    if (!option) return false;

    return option.values.some(v => 
      v.id !== currentValueId && v.value.toLowerCase() === value.toLowerCase()
    );
  };

  const handleValueChange = (
    optionId: string,
    valueId: string,
    value: string
  ) => {
    if (checkDuplicateValue(optionId, value, valueId)) {
      setDuplicateErrors(prev => ({
        ...prev,
        [valueId]: `Giá trị "${value}" đã tồn tại trong nhóm này`
      }));
      return;
    }

    setDuplicateErrors(prev => {
      const newErrors = { ...prev };
      delete newErrors[valueId];
      return newErrors;
    });

    setOptions((prev) =>
      prev.map((o) =>
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
    setOptions((prev) =>
      prev.map((o) =>
        o.id === optionId
          ? {
              ...o,
              values: [
                ...o.values,
                { id: crypto.randomUUID(), value: "", parentId: optionId },
              ],
            }
          : o
      )
    );
  };

  const handleRemoveValue = (optionId: string, valueId: string) => {
    setOptions((prev) =>
      prev.map((o) =>
        o.id === optionId
          ? { ...o, values: o.values.filter((v) => v.id !== valueId) }
          : o
      )
    );
  };

  /* ------------------ Sinh combinations ----------------- */
  const generateCombinations = (): CombinationData[] => {
    const colorOption = options.find((o) => o.name === "Màu sắc");
    const sizeOption = options.find((o) => o.name === "Kích cỡ");
    const colorValues = colorOption?.values ?? [];
    const sizeValues = sizeOption?.values ?? [];

    if (!colorValues.length && !sizeValues.length) return [];

    // Một chiều
    if (colorValues.length && !sizeValues.length)
      return colorValues.map((v) => ({
        group1: v.value,
        group2: "",
        price: "",
        stock: 0,
        sku: "",
        imgUrl: "",
      }));

    if (!colorValues.length && sizeValues.length)
      return sizeValues.map((v) => ({
        group1: "",
        group2: v.value,
        price: "",
        stock: 0,
        sku: "",
        imgUrl: "",
      }));

    // Hai chiều
    const out: CombinationData[] = [];
    colorValues.forEach((c) =>
      sizeValues.forEach((s) =>
        out.push({
          group1: c.value,
          group2: s.value,
          price: "",
          stock: 0,
          sku: "",
          imgUrl: "",
        })
      )
    );
    return out;
  };

  
  useEffect(() => {
    const combos = generateCombinations();
    setCombinationData(combos);
    form.setValue("variantObjs", combos);
  }, [options]);

 
  const handleCombinationChange = (
    idx: number,
    field: keyof CombinationData,
    value: string
  ) => {
    
    const updated = [...combinationData];
    updated[idx] = {
      ...updated[idx],
      [field]: field === "stock" ? Number(value) : value,
    };
    
    setCombinationData(updated);
    form.setValue("variantObjs", updated);

    // Validate nếu là sku
    if (field === "sku") {
      const errorMsg = validateSku(value);
      setSkuErrors((prev) => ({ ...prev, [idx]: errorMsg || "" }));
    }
    console.log(form.getValues("variantObjs"))
  };

 
  const handleGenerateSkus = () => {
    const baseCodeRaw = form.getValues("code") || "SP";
    const base = normalizeText(baseCodeRaw);

    const updated = combinationData.map((it) => {
      const g1 = normalizeText(it.group1);
      const g2 = normalizeText(it.group2);
      const sku = [base, g1, g2].filter(Boolean).join("-");
      return { ...it, sku };
    });

    setCombinationData(updated);
    form.setValue("variantObjs", updated);
  };


  const group1RowSpans: Record<string, number> = {};
  combinationData.forEach((c) => {
    group1RowSpans[c.group1] = (group1RowSpans[c.group1] || 0) + 1;
  });

  /* ---------------------- Render ------------------------ */
  return (
    <div className="p-4 border rounded space-y-4">
      <h2 className="text-lg font-semibold">Phân loại hàng</h2>

      {/* ------ Buttons thêm nhóm ------ */}
      <div className="flex gap-4">
        <Button
          type="button"
          onClick={() => handleAddGroup("Màu sắc", () => setIsColorGroupDisabled(true))}
          disabled={isColorGroupDisabled}
          variant="outline"
        >
          <Plus size={16} className="mr-2" />Tạo nhóm Màu sắc
        </Button>
        <Button
          type="button"
          onClick={() => handleAddGroup("Kích cỡ", () => setIsSizeGroupDisabled(true))}
          disabled={isSizeGroupDisabled}
          variant="outline"
        >
          <Plus size={16} className="mr-2" />Tạo nhóm Kích cỡ
        </Button>
      </div>

      {/* ------ Danh sách option & value ------ */}
      {options.map((opt) => (
        <div key={opt.id} className="border p-3 rounded bg-gray-50 mt-4 space-y-2">
          <div className="flex items-center gap-2">
            <Input value={opt.name} disabled className="w-1/3" />
            <Button
              type="button"
              onClick={() => handleRemoveOption(opt.id)}
              variant="ghost"
              size="icon"
              className="ml-auto text-gray-500"
            >
              <X size={18} />
            </Button>
          </div>
          <div className="flex flex-wrap gap-2">
            {opt.values.map((val) => (
              <div key={val.id} className="relative">
                <Input
                  value={val.value}
                  placeholder="Giá trị phân loại"
                  onChange={(e) => handleValueChange(opt.id, val.id, e.target.value)}
                  className={`pr-6 ${duplicateErrors[val.id] ? 'border-red-500' : ''}`}
                />
                {duplicateErrors[val.id] && (
                  <p className="text-xs text-red-600 mt-1">{duplicateErrors[val.id]}</p>
                )}
                <Button
                  type="button"
                  onClick={() => handleRemoveValue(opt.id, val.id)}
                  size="icon"
                  variant="ghost"
                  className="absolute top-1 right-1 text-gray-500"
                >
                  <X size={12} />
                </Button>
              </div>
            ))}
            <Button type="button" size="sm" variant="ghost" onClick={() => handleAddValue(opt.id)}>
              <Plus size={14} className="mr-1" />Thêm giá trị
            </Button>
          </div>
        </div>
      ))}

      {/* ------ Inputs mặc định khi chưa có nhóm ------ */}
      {showDefaultInputs && (
        <div className="mt-4 space-y-2">
          <Input placeholder="Nhập giá trị mặc định 1" />
          <Input placeholder="Nhập giá trị mặc định 2" />
        </div>
      )}

      {/* ------ Table combinations ------ */}
      {combinationData.length > 0 && (
        <div className="mt-4 border rounded-lg p-4 space-y-2">
          <h3 className="font-semibold mb-2">Danh sách phân loại hàng</h3>

          <Button type="button" variant="outline" className="mb-2" onClick={handleGenerateSkus}>
            Sinh SKU tự động
          </Button>

          <table className="w-full border-collapse">
            <thead>
              <tr className="bg-gray-100">
                <th className="border p-2 text-left">Màu sắc</th>
                <th className="border p-2 text-left">Kích cỡ</th>
                <th className="border p-2 text-left">Giá</th>
                <th className="border p-2 text-left">Kho hàng</th>
                <th className="border p-2 text-left">SKU</th>
                <th className="border p-2 text-left">Hỉnh ảnh</th>
              </tr>
            </thead>
            <tbody>
              {combinationData.map((c, idx) => {
                const isFirst = idx === 0 || c.group1 !== combinationData[idx - 1].group1;
                const rowSpan = group1RowSpans[c.group1];
                return (
                  <tr key={idx}>
                    {isFirst && (
                      <td className="border p-2 align-middle" rowSpan={rowSpan}>
                        {c.group1}
                      </td>
                    )}
                    <td className="border p-2">{c.group2}</td>
                    <td className="border p-2">
                      <Input
                        value={c.price}
                        onChange={(e) => handleCombinationChange(idx, "price", e.target.value)}
                        placeholder="Giá"
                      />
                    </td>
                    <td className="border p-2">
                      <Input
                        value={c.stock.toString()}
                        onChange={(e) => handleCombinationChange(idx, "stock", e.target.value)}
                        placeholder="Kho"
                      />
                    </td>
                    <td className="border p-2">
                      <Input
                        value={c.sku}
                        onChange={(e) => handleCombinationChange(idx, "sku", e.target.value)}
                        placeholder="SKU"
                        className={skuErrors[idx] ? "border-red-500" : ""}
                      />
                      {skuErrors[idx] && (
                        <p className="text-xs text-red-600 mt-1">{skuErrors[idx]}</p>
                      )}
                    </td>
                    <td  className="border p-2">
                    <ImageIconSelector 
                          value={c.imgUrl}
                          onChange={(value) => handleCombinationChange(idx, "imgUrl", value)} className="my-4" />
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
