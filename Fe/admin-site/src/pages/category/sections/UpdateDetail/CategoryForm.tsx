/* eslint-disable @typescript-eslint/no-explicit-any */
import React from "react";
import { FormProvider } from "react-hook-form";
import { Input } from "@/components/ui/input";

interface CategoryFormProps {
  methods: any;
  isEditing: boolean;
  onSubmit: (values: any) => void;
  children: React.ReactNode;
}

export const CategoryForm: React.FC<CategoryFormProps> = ({
  methods,
  isEditing,
  onSubmit,
  children,
}) => {
  const { register, handleSubmit } = methods;

  return (
    <FormProvider {...methods}>
      <form onSubmit={handleSubmit(onSubmit)} className="space-y-4 mt-4">
        <Input {...register("name")} placeholder="Tên" disabled={!isEditing} />
        <Input
          {...register("description")}
          placeholder="Mô tả"
          disabled={!isEditing}
        />

        {children}
      </form>
    </FormProvider>
  );
};
