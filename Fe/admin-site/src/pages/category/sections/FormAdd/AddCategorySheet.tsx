import { Button } from "@/components/ui/button";
import { Form } from "@/components/ui/form";
import React, { useState } from "react";
import { useAppDispatch } from "@/hooks/use-app-dispatch";
import { createCategory } from "@/redux/apps/category/categorySlice";
import CategoryReqDto from "@/types/category/category";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { CategoryFormSchema, categoryFormSchema } from "./FormSchema";
import { BasicInfoFields } from "./BasicInfoFields";
import ActionHeader from "@/pages/relation/sections/Action";
import { MetadataSection } from "./MetadataSection";
import {
  Sheet,
  SheetContent,
  SheetDescription,
  SheetHeader,
  SheetTitle,
} from "@/components/ui/sheet";

interface AddCategorySheetProps {
  isOpen: boolean;
  onClose: () => void;
}

const AddCategorySheet: React.FC<AddCategorySheetProps> = ({
  isOpen,
  onClose,
}) => {

  const dispatch = useAppDispatch();
  const [isSubmitting, setIsSubmitting] = useState(false);

  const form = useForm<CategoryFormSchema>({
    resolver: zodResolver(categoryFormSchema),
    defaultValues: {
      code: "",
      name: "",
      description: "",
      parentId: "",
      metadataObj: [],
      sortOrder: 0,
      parentPath: "parent/path",
      createdByUserId: "3fa85f64-5717-4562-b3fc-2c963f66afa6",
      lastModifiedByUserId: "3fa85f64-5717-4562-b3fc-2c963f66afa6",
      lastModifiedOnDate: new Date().toISOString(),
      createdOnDate: new Date().toISOString(),
    },
  });

  const handleCategorySelect = (categoryId: string) => {
    form.setValue("parentId", categoryId);
  };


  const handleCategorySelect = (categoryId: string) => {
    form.setValue("parentId", categoryId);
  };


  const handleSubmit = async (values: CategoryFormSchema) => {
    setIsSubmitting(true);
    try {
      const categoryData: CategoryReqDto = {
        ...values,
        sortOrder: values.sortOrder || 0,
        createdByUserId: values.createdByUserId || "",
        lastModifiedByUserId: values.lastModifiedByUserId || "",
        lastModifiedOnDate:
          values.lastModifiedOnDate || new Date().toISOString(),
        createdOnDate: values.createdOnDate || new Date().toISOString(),
        metadataObj: values.metadataObj || [],
      };

      await dispatch(createCategory(categoryData));

      onClose();
    } catch (error) {
      console.error("Create category error details:", error);
    } finally {
      setIsSubmitting(false);
    }
  };

  return (
    <Sheet open={isOpen} onOpenChange={onClose}>
      <SheetContent className="w-[90%] sm:max-w-[80vw] max-w-none h-screen overflow-y-auto">
        <SheetHeader>
          <SheetTitle className="text-xl font-semibold text-gray-700">
            Tạo danh mục mới
          </SheetTitle>

          <SheetDescription>
            Nhập thông tin danh mục mới vào biểu mẫu bên dưới.
          </SheetDescription>
        </SheetHeader>

        <Form {...form}>
          <form
            onSubmit={form.handleSubmit(handleSubmit)}
            className="space-y-6"
          >
            <BasicInfoFields control={form.control} />
            <h3 className="text-lg font-medium">Chọn Danh mục Cha</h3>
            <ActionHeader onCategorySelect={handleCategorySelect}/>
            <MetadataSection form={form} />

            <div className="flex justify-end gap-2 absolute bottom-4 left-0 w-full px-6">
              <Button type="submit" disabled={isSubmitting}>
                {isSubmitting ? "Đang tạo..." : "Tạo danh  mục mới"}
              </Button>
              <Button variant="outline" onClick={onClose} type="button">
                Cancel
              </Button>
            </div>
          </form>
        </Form>
      </SheetContent>
    </Sheet>
  );
};

export default AddCategorySheet;
