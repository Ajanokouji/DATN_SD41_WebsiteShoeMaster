import React from "react";
import {
  Sheet,
  SheetContent,
  SheetDescription,
  SheetHeader,
  SheetTitle,
} from "@/components/ui/sheet";

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
  return (
    <Sheet open={isOpen} onOpenChange={onClose}>
      <SheetContent className="w-[90%] sm:max-w-[80vw] max-w-none h-screen overflow-y-auto">
        <SheetHeader>
          <SheetTitle className="text-xl font-semibold text-gray-700">
            Detail Category {categoryId}
          </SheetTitle>
          <SheetDescription>
            Create a new category with custom metadata fields
          </SheetDescription>
        </SheetHeader>
      </SheetContent>
    </Sheet>
  );
};

export default DetailCategorySheet;
