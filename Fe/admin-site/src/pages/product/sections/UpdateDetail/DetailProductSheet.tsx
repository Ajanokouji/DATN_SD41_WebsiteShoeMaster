import React from "react";
import {
  Sheet,
  SheetContent,
  SheetHeader,
  SheetTitle,
  SheetDescription,
} from "@/components/ui/sheet";
import { useAppSelector } from "@/hooks/use-app-selector";
import { Button } from "@/components/ui/button";
import { FieldSelectionValuesList } from "./FieldSelectionValuesList";
import { ProductForm } from "./ProductForm";
import { useProductForm } from "./use-product-form";
import { selectProduct } from "@/redux/apps/product/productSelector";

interface DetailProductSheetProps {
  categoryId: string;
  isOpen: boolean;
  onClose: () => void;
}

const DetailProductSheet: React.FC<DetailProductSheetProps> = ({
  categoryId,
  isOpen,
  onClose,
}) => {
  const category = useAppSelector(selectProduct);
  const {
    isEditing,
    setIsEditing,
    methods,
    fieldSelectionValues,
    handleFieldSelectionValueChange,
    handleAddFieldSelectionValue,
    handleRemoveFieldSelectionValue,
    handleSubmit,
  } = useProductForm(categoryId, category, onClose);

  return (
    <Sheet open={isOpen} onOpenChange={onClose}>
      <SheetContent className="w-[90%] sm:max-w-[80vw] max-w-none h-screen overflow-y-auto">
        <SheetHeader>
          <SheetTitle className="text-xl font-semibold text-gray-700">
            Detail Product: {category?.name}
          </SheetTitle>
          <SheetDescription />
        </SheetHeader>

        <ProductForm
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
            <Button
              className="flex absolute bottom-3 left-7"
              type="button"
              onClick={() => setIsEditing(true)}
            >
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
        </ProductForm>
      </SheetContent>
    </Sheet>
  );
};

export default DetailProductSheet;
