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
import { selectContentBase } from "@/redux/apps/contentBase/contentBaseSelector";
import { useContentBaseForm } from "./use-contentBase-form";
import { ContentBaseForm } from "./ContentBaseForm";

interface DetailContentBaseSheetProps {
  contentBaseId: string;
  isOpen: boolean;
  onClose: () => void;
}

const DetailContentBaseSheet: React.FC<DetailContentBaseSheetProps> = ({
  contentBaseId,
  isOpen,
  onClose,
}) => {
  const contentBase = useAppSelector(selectContentBase);
  const {
    isEditing,
    setIsEditing,
    methods,
    handleSubmit,
  } = useContentBaseForm(contentBaseId, contentBase, onClose);

  return (
    <Sheet open={isOpen} onOpenChange={onClose}>
      <SheetContent className="w-[90%] sm:max-w-[80vw] max-w-none h-screen overflow-y-auto">
        <SheetHeader>
          <SheetTitle className="text-xl font-semibold text-gray-700">
            Chi tiết Content Base: {contentBase?.title}
          </SheetTitle>
          <SheetDescription />
        </SheetHeader>

        <ContentBaseForm
          methods={methods}
          isEditing={isEditing}
          onSubmit={handleSubmit}
        >
          {!isEditing ? (
            <Button
              className="flex justify-start bottom-3 left-7"
              type="button"
              onClick={() => setIsEditing(true)}
            >
              Chỉnh sửa
            </Button>
          ) : (
            <div className="flex justify-start bottom-3 left-7">
              <Button type="submit">Lưu</Button>
              <Button
                type="button"
                variant="secondary"
                onClick={() => {
                  methods.reset();
                  setIsEditing(false);
                }}
              >
                Hủy
              </Button>
            </div>
          )}
        </ContentBaseForm>
      </SheetContent>
    </Sheet>
  );
};

export default DetailContentBaseSheet;