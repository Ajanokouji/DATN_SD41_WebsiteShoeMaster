import React from "react";
import { UseFormReturn } from "react-hook-form";
import { ProductFormSchema } from "./FormSchema";
import { Card, CardContent } from "@/components/ui/card";
import {
  FormField,
  FormItem,
  FormLabel,
  FormControl,
  FormMessage,
} from "@/components/ui/form";
import { Input } from "@/components/ui/input";
import { Button } from "@/components/ui/button";
import { Trash2 } from "lucide-react";

interface LabelCardProps {
  index: number;
  form: UseFormReturn<ProductFormSchema>;
  onRemove: () => void;
}

export const LabelCard: React.FC<LabelCardProps> = ({
  index,
  form,
  onRemove,
}) => {
  return (
    <Card className="border border-gray-200">
      <CardContent className="pt-4">
        <div className="flex justify-between items-start mb-4">
          <h4 className="text-md font-medium">Nhãn dán #{index + 1}</h4>
          <Button
            type="button"
            variant="ghost"
            size="sm"
            onClick={onRemove}
            className="text-red-500 hover:text-red-700 hover:bg-red-50"
          >
            <Trash2 className="h-4 w-4" />
          </Button>
        </div>

        <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
          <FormField
            control={form.control}
            name={`labelsObjs.${index}.objectCode`}
            render={({ field }) => (
              <FormItem>
                <FormLabel>Mã nhãn dán</FormLabel>
                <FormControl>
                  <Input placeholder="Nhập mã nhãn dán" {...field} />
                </FormControl>
                <FormMessage />
              </FormItem>
            )}
          />

          <FormField
            control={form.control}
            name={`labelsObjs.${index}.objectName`}
            render={({ field }) => (
              <FormItem>
                <FormLabel>Tên nhãn dán</FormLabel>
                <FormControl>
                  <Input placeholder="Nhập tên nhãn dán" {...field} />
                </FormControl>
                <FormMessage />
              </FormItem>
            )}
          />

          <FormField
            control={form.control}
            name={`labelsObjs.${index}.color`}
            render={({ field }) => (
              <FormItem className="col-span-2">
                <FormLabel>Màu Sắc</FormLabel>
                <div className="flex items-center gap-2">
                  <FormControl>
                    <Input type="color" {...field} className="w-16 h-8" />
                  </FormControl>
                  <Input
                    type="text"
                    value={field.value}
                    onChange={field.onChange}
                    placeholder="#RRGGBB"
                    className="flex-1"
                  />
                </div>
                <FormMessage />
              </FormItem>
            )}
          />
        </div>
      </CardContent>
    </Card>
  );
};
