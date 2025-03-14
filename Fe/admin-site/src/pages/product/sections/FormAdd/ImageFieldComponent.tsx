import React from "react";
import { Control, useWatch } from "react-hook-form";
import {
  FormControl,
  FormField,
  FormItem,
  FormLabel,
  FormMessage,
} from "@/components/ui/form";
import { Input } from "@/components/ui/input";
import { ProductFormSchema } from "./FormSchema";

interface ImageFieldProps {
  control: Control<ProductFormSchema>;
}

export const ImageField: React.FC<ImageFieldProps> = ({ control }) => {
  const imageUrl = useWatch({ control, name: "imageUrl" });

  return (
    <>
      <div className="grid grid-cols-2 gap-4">
        <FormField
          control={control}
          name="imageUrl"
          render={({ field }) => (
            <FormItem>
              <FormLabel>URL Hình ảnh</FormLabel>
              <FormControl>
                <Input placeholder="https://example.com/image.jpg" {...field} />
              </FormControl>
              <FormMessage />
            </FormItem>
          )}
        />
        {imageUrl && (
          <div>
            <p className="text-sm font-medium mb-3">Xem trước hình ảnh:</p>
            <div className="border rounded-md overflow-hidden w-full max-w-xs h-48 bg-slate-100">
              <img
                src={imageUrl}
                alt="Product preview"
                className="w-full h-full object-contain"
                onError={(e) => {
                  const target = e.target as HTMLImageElement;
                  target.src = "/api/placeholder/200/200";
                  target.alt = "Image error";
                }}
              />
            </div>
          </div>
        )}
      </div>
    </>
  );
};
