import React from "react";
import { UseFormReturn } from "react-hook-form";
import { Card, CardHeader, CardTitle, CardContent } from "@/components/ui/card";
import {
  FormControl,
  FormDescription,
  FormField,
  FormItem,
  FormLabel,
  FormMessage,
} from "@/components/ui/form";
import { Input } from "@/components/ui/input";
import { Button } from "@/components/ui/button";
import { Plus, Trash2 } from "lucide-react";
import { ProductFormSchema } from "./FormSchema";
import { SelectionValuesList } from "./SelectionValuesListComponent";

interface MetadataFieldCardProps {
  index: number;
  form: UseFormReturn<ProductFormSchema>;
  onRemove: () => void;
}

export const MetadataFieldCard: React.FC<MetadataFieldCardProps> = ({
  index,
  form,
  onRemove,
}) => {
  const addFieldSelectionValue = () => {
    const currentMetadata = form.getValues().metadataObj || [];
    const currentSelectionValues =
      currentMetadata[index].fieldSelectionValues || [];

    const newSelectionValues = [
      ...currentSelectionValues,
      {
        key: String(currentSelectionValues.length),
        code: "",
        value: "",
        order: currentSelectionValues.length,
      },
    ];

    form.setValue(
      `metadataObj.${index}.fieldSelectionValues`,
      newSelectionValues
    );
  };

  return (
    <Card className="border border-gray-200">
      <CardHeader className="pb-2">
        <div className="flex justify-between items-center">
          <CardTitle className="text-base">Trường dữ liệu #{index + 1}</CardTitle>
          <Button type="button" variant="ghost" size="sm" onClick={onRemove}>
            <Trash2 className="h-4 w-4 text-red-500" />
          </Button>
        </div>
      </CardHeader>
      <CardContent className="space-y-4">
        <div className="grid grid-cols-2 gap-4">
          <FormField
            control={form.control}
            name={`metadataObj.${index}.fieldName`}
            render={({ field }) => (
              <FormItem>
                <FormLabel>Tên trường</FormLabel>
                <FormControl>
                  <Input placeholder="Ví dụ: Trạng thái" {...field} />
                </FormControl>
                <FormMessage />
              </FormItem>
            )}
          />

          <FormField
            control={form.control}
            name={`metadataObj.${index}.fieldDisplayName`}
            render={({ field }) => (
              <FormItem>
                <FormLabel>Tên hiển trị</FormLabel>
                <FormControl>
                  <Input placeholder="Ví dụ: Trạng thái" {...field} />
                </FormControl>
                <FormMessage />
              </FormItem>
            )}
          />
        </div>

        <div className="space-y-2">
          <div className="flex justify-between items-center">
            <FormLabel>Lựa chọn giá trị</FormLabel>
            <Button
              type="button"
              variant="outline"
              size="sm"
              onClick={addFieldSelectionValue}
            >
              <Plus className="h-3 w-3 mr-1" /> Thêm lựa chọn
            </Button>
          </div>

          <SelectionValuesList metadataIndex={index} form={form} />
        </div>

        <div className="grid grid-cols-1 gap-4">
          <FormField
            control={form.control}
            name={`metadataObj.${index}.fieldValues`}
            render={({ field }) => (
              <FormItem>
                <FormLabel>Giá trị mặc định</FormLabel>
                <FormControl>
                  <Input placeholder="Nhập giá trị mặc định" {...field} />
                </FormControl>
                <FormDescription>
                Các giá trị mặc định được phân tách bằng dấu phẩy
                </FormDescription>
                <FormMessage />
              </FormItem>
            )}
          />
        </div>
      </CardContent>
    </Card>
  );
};
