import { Button } from "@/components/ui/button";
import {
  Sheet,
  SheetContent,
  SheetHeader,
  SheetTitle,
  SheetDescription,
} from "@/components/ui/sheet";
import { Form } from "@/components/ui/form";
import React, { useState } from "react";
import { useAppDispatch } from "@/hooks/use-app-dispatch";
import { useForm, Controller } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { BasicInfoFields } from "./BasicInfoFields";
import { MetadataSection } from "./MetadataSection";
import { productFormSchema, ProductFormSchema } from "./FormSchema";
import ProductReqDto from "@/types/product/product";
import { createProduct } from "@/redux/apps/product/productSlice";
import { ImageField } from "./ImageFieldComponent";
import ActionHeader from "@/pages/relation/sections/Action";
import { VariantForm } from "./VariantForm";
import { MultipleImageField } from "./MultipleImageField";

interface AddProductSheetProps {
  isOpen: boolean;
  onClose: () => void;
}

const AddProductSheet: React.FC<AddProductSheetProps> = ({
  isOpen,
  onClose,
}) => {
  const dispatch = useAppDispatch();
  const [isSubmitting, setIsSubmitting] = useState(false);

  const form = useForm<ProductFormSchema>({
    resolver: zodResolver(productFormSchema),
    defaultValues: {
      code: "",
      name: "",
      description: "",
      imageUrl: "",
      mediaObjs: [],
      metadataObj: [],
      labelsObjs: [],
      mainCategoryId: "",
      variantObjs: [],
      sortOrder: "0",
      lastModifiedDate: new Date().toISOString(),
      createdOnDate: new Date().toISOString(),
      publicOnDate: new Date().toISOString(),
      createdByUserId: "3fa85f64-5717-4562-b3fc-2c963f66afa6",
      lastModifiedByUserId: "3fa85f64-5717-4562-b3fc-2c963f66afa6",
      status: "Available",
      workFlowStates: "Available",
    },
  });

  const handleSubmit = async (values: ProductFormSchema) => {
    console.log("Form is submitting with values:", values);
    setIsSubmitting(true);
    try {
      const productData: ProductReqDto = {
        ...values,
        sortOrder: String(values.sortOrder || 0),
        createdByUserId: values.createdByUserId || "",
        lastModifiedByUserId: values.lastModifiedByUserId || "",
        lastModifiedDate: values.lastModifiedDate || new Date().toISOString(),
        createdOnDate: values.createdOnDate || new Date().toISOString(),
        publicOnDate: values.createdOnDate || new Date().toISOString(),
        metadataObj: values.metadataObj || [],
        labelsObjs: values.labelsObjs || [],
        variantObjs:
          values.variantObjs?.map((variant) => ({
            ...variant,
            imgUrl: variant.imgUrl || "",
          })) || [],
      };

      await dispatch(createProduct(productData));
      onClose();
    } catch (error) {
      console.error("Create product error:", error);
    } finally {
      setIsSubmitting(false);
    }
  };

  return (
    <Sheet open={isOpen} onOpenChange={onClose}>
      <SheetContent className="w-[90%] sm:max-w-[80vw] max-w-none h-screen overflow-y-auto">
        <SheetHeader>
          <SheetTitle className="text-xl font-semibold text-gray-700">
            Thêm Sản Phẩm
          </SheetTitle>
          <SheetDescription>
            Tạo sản phẩm mới với các trường thuộc tính tùy chỉnh
          </SheetDescription>
        </SheetHeader>

        <Form {...form}>
          <form
            onSubmit={form.handleSubmit(handleSubmit, (e) => {
              console.warn("Form validation failed", e);
            })}
            className="space-y-6"
          >
            <BasicInfoFields control={form.control} />

            {/* Chọn danh mục sản phẩm có hiển thị lỗi */}
            <div>
              <h3 className="text-lg font-medium">Chọn Danh mục sản phẩm</h3>
              <Controller
                name="mainCategoryId"
                control={form.control}
                render={({ field, fieldState }) => (
                  <div>
                    <ActionHeader
                      onCategorySelect={(categoryId) =>
                        field.onChange(categoryId)
                      }
                    />
                    {fieldState.error && (
                      <p className="text-sm text-red-500 mt-1">
                        {fieldState.error.message}
                      </p>
                    )}
                  </div>
                )}
              />
            </div>

            <ImageField control={form.control} />
            <MultipleImageField control={form.control} />
            <MetadataSection form={form} />
            {/* <LabelsSection form={form} /> */}
            <VariantForm form={form} />

            {/* Debug lỗi toàn bộ form */}
            <pre className="text-red-600 text-xs bg-gray-50 p-2 rounded">
              {JSON.stringify(form.formState.errors, null, 2)}
            </pre>

            <div className="flex justify-end gap-2 pt-4">
              <Button type="submit" disabled={isSubmitting}>
                {isSubmitting ? "Đang tạo..." : "Tạo sản phẩm"}
              </Button>
              <Button variant="outline" onClick={onClose} type="button">
                Huỷ
              </Button>
            </div>
          </form>
        </Form>
      </SheetContent>
    </Sheet>
  );
};

export default AddProductSheet;
