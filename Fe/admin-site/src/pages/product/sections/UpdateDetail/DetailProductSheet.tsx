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
import { MediaList } from "./MediaList";
import { VariantForm } from "./VariantForm";
interface UpdateProductSheetProps {
  productId: string;
  isOpen: boolean;
  onClose: () => void;
}

const UpdateProductSheet: React.FC<UpdateProductSheetProps> = ({
  productId,
  isOpen,
  onClose,
}) => {
  const product = useAppSelector(selectProduct);
  const {
    isEditing,
    setIsEditing,
    methods,
    fieldSelectionValues,
    handleFieldSelectionValueChange,
    handleAddFieldSelectionValue,
    handleRemoveFieldSelectionValue,
    variantObjs,
    handleVariantChange,
    mediaObjs,
    handleAddMedia,
    handleRemoveMedia,
    handleSubmit,
  } = useProductForm(productId, product, onClose);

  return (
    <Sheet open={isOpen} onOpenChange={onClose}>
      <SheetContent className="w-[90%] sm:max-w-[80vw] max-w-none h-screen overflow-y-auto">
        <SheetHeader>
          <SheetTitle className="text-xl font-semibold text-gray-700">
            Detail Product: {product?.name}
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

          <VariantForm
            initialVariants={variantObjs}
            mode="list"
            isEditing={isEditing}
            onVariantChange={handleVariantChange}
          />

          

          <MediaList
            mediaUrls={mediaObjs}
            isEditing={isEditing}
            onAdd={handleAddMedia}
            onRemove={handleRemoveMedia}
          />

          {!isEditing ? (
            <Button
              className="flex items-end justify-end"
              type="button"
              onClick={() => setIsEditing(true)}
            >
              Chỉnh sửa
            </Button>
          ) : (
            <div className="flex items-end justify-end space-x-2">
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

export default UpdateProductSheet;
