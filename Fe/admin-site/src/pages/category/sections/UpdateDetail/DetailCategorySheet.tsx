import React from "react";
import { Sheet, SheetContent, SheetHeader, SheetTitle, SheetDescription } from "@/components/ui/sheet";
import { useAppSelector } from "@/hooks/use-app-selector";
import { selectCategory } from "@/redux/apps/category/categorySelector";
import { Button } from "@/components/ui/button";
import { useCategoryForm } from "./use-category-form";
import { CategoryForm } from "./CategoryForm";
import { FieldSelectionValuesList } from "./FieldSelectionValuesList";


interface DetailCategorySheetProps {
  categoryId: string;
  isOpen: boolean;
  onClose: () => void;
}

const DetailCategorySheet: React.FC<DetailCategorySheetProps> = ({
  categoryId,
  isOpen,
  onClose,
}) => {
  const category = useAppSelector(selectCategory);
  const {
    isEditing, 
    setIsEditing,
    methods,
    fieldSelectionValues,
    handleFieldSelectionValueChange,
    handleAddFieldSelectionValue,
    handleRemoveFieldSelectionValue,
    handleSubmit
  } = useCategoryForm(categoryId, category, onClose);

  return (
    <Sheet open={isOpen} onOpenChange={onClose}>
      <SheetContent className="w-[90%] sm:max-w-[80vw] max-w-none h-screen overflow-y-auto">
        <SheetHeader>
          <SheetTitle className="text-xl font-semibold text-gray-700">
            Detail Category: {category?.name}
          </SheetTitle>
          <SheetDescription />
        </SheetHeader>
        
        <CategoryForm 
          methods={methods} 
          isEditing={isEditing} 
          onSubmit={handleSubmit}
        >
          <FieldSelectionValuesList 
            fieldSelectionValues={fieldSelectionValues}
            isEditing={isEditing}
            onValueChange={handleFieldSelectionValueChange}
            onAdd={handleAddFieldSelectionValue}
            onRemove={handleRemoveFieldSelectionValue}
          />

          {!isEditing ? (
            <Button className="flex absolute bottom-3 left-7" type="button" onClick={() => setIsEditing(true)}>
              Chỉnh sửa
            </Button>
          ) : (
            <div className="flex absolute bottom-3 left-7 space-x-2">
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
        </CategoryForm>
      </SheetContent>
    </Sheet>
  );
};

export default DetailCategorySheet;