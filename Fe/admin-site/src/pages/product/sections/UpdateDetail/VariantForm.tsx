import React, { useState, useEffect } from "react";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { UseFormReturn } from "react-hook-form";
import { X, Plus } from "lucide-react";
import { ProductFormSchema } from "../FormAdd/FormSchema";

import {ImageIconSelector} from "../FormAdd/ImageIconSelectorProps";
interface Option {
  id: string;
  parentId?: string | null;
  name: string;
  values: { id: string; value: string; parentId: string | null }[];
}

interface VariantFormProps {
  form?: UseFormReturn<ProductFormSchema>;
  initialVariants?: CombinationData[];
  mode?: "create" | "list";
  isEditing?: boolean;
  onVariantChange?: (variants: CombinationData[]) => void;
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
  imgUrl: string;
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

export const VariantForm: React.FC<VariantFormProps> = ({
  form,
  initialVariants = [],
  mode = "create",
  isEditing = true,
  onVariantChange
}) => {
  const [options, setOptions] = useState<Option[]>([]);
  const [isColorGroupDisabled, setIsColorGroupDisabled] = useState(false);
  const [isSizeGroupDisabled, setIsSizeGroupDisabled] = useState(false);
  const [showDefaultInputs, setShowDefaultInputs] = useState(true);
  const [combinationData, setCombinationData] = useState<CombinationData[]>(initialVariants);
  const [skuErrors, setSkuErrors] = useState<Record<number, string>>({});
  const [pendingUpdate, setPendingUpdate] = useState<NodeJS.Timeout | null>(null);
  const [duplicateErrors, setDuplicateErrors] = useState<Record<string, string>>({});
  const areValuesEmpty = () => options.every((o) => o.values.length === 0);
  const isAnyButtonDisabled = () => isColorGroupDisabled || isSizeGroupDisabled;

  useEffect(() => {
    setShowDefaultInputs(areValuesEmpty() && !isAnyButtonDisabled());
  }, [options, isColorGroupDisabled, isSizeGroupDisabled]);

  useEffect(() => {
    if (mode === "list" && initialVariants.length > 0) {
      const colorValues = new Set(initialVariants.map(v => v.group1).filter(Boolean));
      const sizeValues = new Set(initialVariants.map(v => v.group2).filter(Boolean));
      const newOptions: Option[] = [];
      if (colorValues.size > 0) {
        newOptions.push({
          id: crypto.randomUUID(),
          parentId: null,
          name: "Màu sắc",
          values: Array.from(colorValues).map(value => ({
            id: crypto.randomUUID(),
            value,
            parentId: null
          }))
        });
        setIsColorGroupDisabled(true);
      }
      if (sizeValues.size > 0) {
        newOptions.push({
          id: crypto.randomUUID(),
          parentId: null,
          name: "Kích cỡ",
          values: Array.from(sizeValues).map(value => ({
            id: crypto.randomUUID(),
            value,
            parentId: null
          }))
        });
        setIsSizeGroupDisabled(true);
      }
      setOptions(newOptions);
      setCombinationData(initialVariants);
    }
  }, [mode, initialVariants]);

  useEffect(() => {
    if (mode === "list" && form) {
      form.setValue("variantObjs", combinationData);
      
      const variantObjs = form.getValues("variantObjs");
      const serializedVariants = variantObjs.map(variant => ({
        Sku: variant.sku,
        ImgUrl: variant.imgUrl || "",
        Group1: variant.group1,
        Group2: variant.group2,
        Size: null,
        Price: variant.price,
        Stock: variant.stock
      }));
      console.log("serializedVariants", serializedVariants);
      form.setValue("variantJson", JSON.stringify(serializedVariants));
      
      if (onVariantChange) onVariantChange(combinationData);
    }
  }, [combinationData, mode, form, onVariantChange]);

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

  const handleAddValue = (optionId: string) => {
    const updatedOptions = options.map((o) =>
      o.id === optionId
        ? {
            ...o,
            values: [
              ...o.values,
              { id: crypto.randomUUID(), value: "", parentId: optionId },
            ],
          }
        : o
    );
    setOptions(updatedOptions);
  };

  const handleRemoveValue = (optionId: string, valueId: string) => {
    const updatedOptions = options.map((o) =>
      o.id === optionId
        ? { ...o, values: o.values.filter((v) => v.id !== valueId) }
        : o
    );
    setOptions(updatedOptions);

    const newCombinations = generateCombinationsFromOptions(updatedOptions);
    setCombinationData(newCombinations);
    
    if (form) {
      form.setValue("variantObjs", newCombinations);
    }
    if (onVariantChange) {
      onVariantChange(newCombinations);
    }
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

    const updatedOptions = options.map((o) =>
      o.id === optionId
        ? {
            ...o,
            values: o.values.map((v) =>
              v.id === valueId ? { ...v, value } : v
            ),
          }
        : o
    );
    setOptions(updatedOptions);

    if (mode === "list") {
      if (pendingUpdate) clearTimeout(pendingUpdate);
      const timeout = setTimeout(() => {
        const colorOption = updatedOptions.find(o => o.name === "Màu sắc");
        const sizeOption = updatedOptions.find(o => o.name === "Kích cỡ");
        
        const existingValues = new Map<string, CombinationData>();
        combinationData.forEach(item => {
          const key = `${item.group1}-${item.group2}`;
          existingValues.set(key, item);
        });

        const newCombinations: CombinationData[] = [];
        
        if (colorOption && sizeOption) {
          colorOption.values.forEach(color => {
            sizeOption.values.forEach(size => {
              const key = `${color.value}-${size.value}`;
              const existing = existingValues.get(key);
              if (existing) {
                newCombinations.push(existing);
              } else if (color.value.trim() !== "" && size.value.trim() !== "") {
                newCombinations.push({
                  group1: color.value,
                  group2: size.value,
                  price: "",
                  stock: 0,
                  sku: "",
                  imgUrl: ""
                });
              }
            });
          });
        } else if (colorOption) {
          colorOption.values.forEach(color => {
            const key = `${color.value}-`;
            const existing = existingValues.get(key);
            if (existing) {
              newCombinations.push(existing);
            } else if (color.value.trim() !== "") {
              newCombinations.push({
                group1: color.value,
                group2: "",
                price: "",
                stock: 0,
                sku: "",
                imgUrl: ""
              });
            }
          });
        } else if (sizeOption) {
          sizeOption.values.forEach(size => {
            const key = `-${size.value}`;
            const existing = existingValues.get(key);
            if (existing) {
              newCombinations.push(existing);
            } else if (size.value.trim() !== "") {
              newCombinations.push({
                group1: "",
                group2: size.value,
                price: "",
                stock: 0,
                sku: "",
                imgUrl: ""
              });
            }
          });
        }

        setCombinationData(newCombinations);
        if (form) {
          form.setValue("variantObjs", newCombinations);
        }
        if (onVariantChange) {
          onVariantChange(newCombinations);
        }
      }, 500);
      setPendingUpdate(timeout);
    }

    if (value === "sku") {
      const errorMsg = validateSku(value);
      setSkuErrors((prev) => ({ ...prev, [combinationData.findIndex(c => c.group1 === optionId && c.group2 === valueId)]: errorMsg || "" }));
    }
  };

  const generateCombinationsFromOptions = (currentOptions: Option[]): CombinationData[] => {
    const colorOption = currentOptions.find((o) => o.name === "Màu sắc");
    const sizeOption = currentOptions.find((o) => o.name === "Kích cỡ");
    const colorValues = colorOption?.values ?? [];
    const sizeValues = sizeOption?.values ?? [];

    if (!colorValues.length && !sizeValues.length) return [];

    const existingValues = new Map<string, CombinationData>();
    combinationData.forEach(item => {
      const key = `${item.group1}-${item.group2}`;
      existingValues.set(key, {
        ...item,
        imgUrl: item.imgUrl || ""
      });
    });

    if (colorValues.length && !sizeValues.length) {
      return colorValues.map((v) => {
        const key = `${v.value}-`;
        const existing = existingValues.get(key);
        return existing || {
          group1: v.value,
          group2: "",
          price: "",
          stock: 0,
          sku: "",
          imgUrl: ""
        };
      });
    }

    if (!colorValues.length && sizeValues.length) {
      return sizeValues.map((v) => {
        const key = `-${v.value}`;
        const existing = existingValues.get(key);
        return existing || {
          group1: "",
          group2: v.value,
          price: "",
          stock: 0,
          sku: "",
          imgUrl: ""
        };
      });
    }

    const out: CombinationData[] = [];
    colorValues.forEach((c) =>
      sizeValues.forEach((s) => {
        const key = `${c.value}-${s.value}`;
        const existing = existingValues.get(key);
        out.push(existing || {
          group1: c.value,
          group2: s.value,
          price: "",
          stock: 0,
          sku: "",
          imgUrl: ""
        });
      })
    );
    return out;
  };

  const handleCombinationChange = (
    idx: number,
    field: keyof CombinationData,
    value: string
  ) => {
    const updated = [...combinationData];
    updated[idx] = {
      ...updated[idx],
      [field]: field === "stock" ? Number(value) : value,
      imgUrl: updated[idx].imgUrl || ""
    };
    
    setCombinationData(updated);
    if (form) {
      form.setValue("variantObjs", updated);
    }
    if (onVariantChange) {
      onVariantChange(updated);
    }

    if (field === "sku") {
      const errorMsg = validateSku(value);
      setSkuErrors((prev) => ({ ...prev, [idx]: errorMsg || "" }));
    }
  };

  const handleGenerateSkus = () => {
    const baseCodeRaw = form?.getValues("code") || "SP";
    const base = normalizeText(baseCodeRaw);

    const updated = combinationData.map((it) => {
      if (!it.sku) {
        const g1 = normalizeText(it.group1);
        const g2 = normalizeText(it.group2);
        const sku = [base, g1, g2].filter(Boolean).join("-");
        return { ...it, sku };
      }
      return it;
    });

    setCombinationData(updated);
    if (form) {
      form.setValue("variantObjs", updated);
    }
    if (onVariantChange) {
      onVariantChange(updated);
    }
  };

  const group1RowSpans: Record<string, number> = {};
  combinationData.forEach((c) => {
    group1RowSpans[c.group1] = (group1RowSpans[c.group1] || 0) + 1;
  });

  useEffect(() => {
    return () => {
      if (pendingUpdate) {
        clearTimeout(pendingUpdate);
      }
    };
  }, [pendingUpdate]);

  return (
    <div className="p-4 border rounded space-y-4">
      <h2 className="text-lg font-semibold">Phân loại hàng</h2>

      {(mode === "create" || (mode === "list" && isEditing)) && (
        <>
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
        </>
      )}

      {combinationData.length > 0 && (
        <div className="mt-4 border rounded-lg p-4 space-y-2">
          <h3 className="font-semibold mb-2">Danh sách phân loại hàng</h3>

          {isEditing && (
            <Button type="button" variant="outline" className="mb-2" onClick={handleGenerateSkus}>
              Sinh SKU tự động cho các mã trống
            </Button>
          )}

          <table className="w-full border-collapse">
            <thead>
              <tr className="bg-gray-100">
                <th className="border p-2 text-left">Màu sắc</th>
                <th className="border p-2 text-left">Kích cỡ</th>
                <th className="border p-2 text-left">Giá</th>
                <th className="border p-2 text-left">Kho hàng</th>
                <th className="border p-2 text-left">SKU</th>
                <th className="border p-2 text-left">Hình ảnh</th>
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
                        type="number"
                        value={c.price}
                        onChange={(e) => handleCombinationChange(idx, "price", e.target.value)}
                        placeholder="Giá"
                        disabled={!isEditing}
                      />
                    </td>
                    <td className="border p-2">
                      <Input
                        type="number"
                        value={c.stock}
                        onChange={(e) => handleCombinationChange(idx, "stock", e.target.value)}

                        placeholder="Kho"
                        disabled={!isEditing}
                      />
                    </td>
                    <td className="border p-2">
                      <Input
                        value={c.sku}
                        onChange={(e) => handleCombinationChange(idx, "sku", e.target.value)}
                        placeholder="SKU"
                        className={skuErrors[idx] ? "border-red-500" : ""}
                        disabled={!isEditing}
                      />
                      {skuErrors[idx] && (
                        <p className="text-xs text-red-600 mt-1">{skuErrors[idx]}</p>
                      )}
                    </td>
                    <td className="border p-2">
                      <ImageIconSelector 
                        value={c.imgUrl}
                        onChange={(value) => handleCombinationChange(idx, "imgUrl", value)}
                        className="my-4"
                      />
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
