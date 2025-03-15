/* eslint-disable @typescript-eslint/no-explicit-any */
import React from "react";
import { FormProvider, useWatch } from "react-hook-form";
import { Input } from "@/components/ui/input";
import { Card, CardContent } from "@/components/ui/card";

interface ProductFormProps {
  methods: any;
  isEditing: boolean;
  onSubmit: (values: any) => void;
  children: React.ReactNode;
}

export const ProductForm: React.FC<ProductFormProps> = ({
  methods,
  isEditing,
  onSubmit,
  children,
}) => {
  const { register, handleSubmit, control } = methods;
  const imageUrl = useWatch({
    control,
    name: "imageUrl",
    defaultValue: "",
  });

  return (
    <FormProvider {...methods}>
      <form onSubmit={handleSubmit(onSubmit)} className="space-y-4 mt-4">
        <Input {...register("name")} placeholder="Tên" disabled={!isEditing} />
        <Input
          {...register("description")}
          placeholder="Mô tả"
          disabled={!isEditing}
        />
        <div className="grid grid-cols-2 gap-4">
          <Input
            {...register("imageUrl")}
            placeholder="Link ảnh"
            disabled={!isEditing}
          />
          {imageUrl && (
            <Card className="overflow-hidden">
              <CardContent className="p-2">
                <div className="relative">
                  <img
                    src={imageUrl}
                    alt="Product preview"
                    className="w-full h-52 object-contain rounded"
                    onError={(e) => {
                      const target = e.target as HTMLImageElement;
                      target.src = "/placeholder-image.jpg";
                      target.onerror = null;
                    }}
                  />
                  {isEditing && (
                    <div className="absolute top-2 right-2 text-xs bg-gray-100 px-2 py-1 rounded opacity-75">
                      Preview
                    </div>
                  )}
                </div>
              </CardContent>
            </Card>
          )}
        </div>
        {children}
      </form>
    </FormProvider>
  );
};
