import React, { useEffect, useState } from "react";
import {
  Sheet,
  SheetContent,
  SheetDescription,
  SheetHeader,
  SheetTitle,
} from "@/components/ui/sheet";
import { useAppDispatch } from "@/hooks/use-app-dispatch";
import { useAppSelector } from "@/hooks/use-app-selector";
import { selectCategory } from "@/redux/apps/category/categorySelector";
import {
  fetchCategoryById,
  updateCategory,
} from "@/redux/apps/category/categorySlice";
import { Input } from "@/components/ui/input";
import { Button } from "@/components/ui/button";
import { useForm, FormProvider } from "react-hook-form";

interface DetailCategorySheetProps {
  categoryId: string;
  isOpen: boolean;
  onClose: () => void;
}

interface FormData {
  name: string;
  description: string;
  parentId: string;
  sortOrder: number;
  type: string;
  code: string;
  completeCode: string;
  completeName: string;
  completePath: string;
  color: string;
  material: string;
}

const DetailCategorySheet: React.FC<DetailCategorySheetProps> = ({
  categoryId,
  isOpen,
  onClose,
}) => {
  const dispatch = useAppDispatch();
  const category = useAppSelector(selectCategory);
  const [isEditing, setIsEditing] = useState(false);

  const methods = useForm<FormData>();
  const { register, handleSubmit, setValue } = methods;

  useEffect(() => {
    if (categoryId) {
      dispatch(fetchCategoryById(categoryId));
    }
  }, [dispatch, categoryId]);

  useEffect(() => {
    if (category) {
      setValue("name", category.name || "");
      setValue("description", category.description || "");
      setValue("parentId", category.id || "");
      setValue("sortOrder", category.sortOrder || 0);
      setValue("type", category.type || "");
      setValue("code", category.code || "");
      setValue("completeCode", category.completeCode || "");
      setValue("completeName", category.completeName || "");
      setValue("completePath", category.completePath || "");
      setValue(
        "color",
        category.metadataObj?.find((meta) => meta.fieldName === "color")
          ?.fieldValues || ""
      );
      setValue(
        "material",
        category.metadataObj?.find((meta) => meta.fieldName === "material")
          ?.fieldValues || ""
      );
    }
  }, [category, setValue]);

  const onSubmit = (value: FormData) => {
    const updatedCategory = {
      ...category,
      ...value,
      metadataObj: [
        {
          fieldName: "color",
          fieldValues: value.color,
          fieldDisplayName: "Color",
          fieldType: 0,
          fieldValueTexts: value.color,
          fieldValueType: "",
          fieldSelectionValues: [],
        },
        {
          fieldName: "material",
          fieldValues: value.material,
          fieldDisplayName: "Material",
          fieldType: 0,
          fieldValueTexts: value.material,
          fieldValueType: "string",
          fieldSelectionValues: [],
        },
      ],
    };
    dispatch(updateCategory({ id: categoryId, data: updatedCategory }));
    setIsEditing(false);
  };

  return (
    <Sheet open={isOpen} onOpenChange={onClose}>
      <SheetContent className="w-[90%] sm:max-w-[80vw] max-w-none h-screen overflow-y-auto">
        <SheetHeader>
          <SheetTitle className="text-xl font-semibold text-gray-700">
            Detail Category: {category?.name}
          </SheetTitle>
          <SheetDescription />
        </SheetHeader>
        <FormProvider {...methods}>
          <form onSubmit={handleSubmit(onSubmit)} className="space-y-4 mt-4">
            <Input
              {...register("name")}
              placeholder="Tên"
              disabled={!isEditing}
            />
            <Input
              {...register("description")}
              placeholder="Mô tả"
              disabled={!isEditing}
            />
            <Input
              {...register("parentId")}
              placeholder="Parent ID"
              disabled={!isEditing}
            />
            <Input
              {...register("sortOrder", { valueAsNumber: true })}
              placeholder="Thứ tự"
              disabled={!isEditing}
            />
            <Input
              {...register("type")}
              placeholder="Loại"
              disabled={!isEditing}
            />
            <Input
              {...register("code")}
              placeholder="Mã"
              disabled={!isEditing}
            />
            <Input
              {...register("completeCode")}
              placeholder="Mã hoàn chỉnh"
              disabled={!isEditing}
            />
            <Input
              {...register("completeName")}
              placeholder="Tên hoàn chỉnh"
              disabled={!isEditing}
            />
            <Input
              {...register("completePath")}
              placeholder="Đường dẫn hoàn chỉnh"
              disabled={!isEditing}
            />
            <Input
              {...register("color")}
              placeholder="Màu sắc"
              disabled={!isEditing}
            />
            <Input
              {...register("material")}
              placeholder="Chất liệu"
              disabled={!isEditing}
            />
            {!isEditing ? (
              <Button type="button" onClick={() => setIsEditing(true)}>
                Chỉnh sửa
              </Button>
            ) : (
              <div className="flex space-x-2">
                <Button type="submit">Lưu</Button>
                <Button
                  type="button"
                  variant="secondary"
                  onClick={() => setIsEditing(false)}
                >
                  Hủy
                </Button>
              </div>
            )}
          </form>
        </FormProvider>
      </SheetContent>
    </Sheet>
  );
};

export default DetailCategorySheet;
