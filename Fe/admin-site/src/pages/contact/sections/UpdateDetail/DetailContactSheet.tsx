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
import { selectContact } from "@/redux/apps/contact/contactSelector";
import { useContactForm } from "./use-contact-form";
import { ContactForm } from "./ContactForm";


interface DetailContactSheetProps {
  contactId: string;
  isOpen: boolean;
  onClose: () => void;
}

const DetailContactSheet: React.FC<DetailContactSheetProps> = ({
  contactId,
  isOpen,
  onClose,
}) => {
  const contact = useAppSelector(selectContact);
  const { isEditing, setIsEditing, methods, handleSubmit } = useContactForm(
    contactId,
    contact,
    onClose
  );

  return (
    <Sheet open={isOpen} onOpenChange={onClose}>
      <SheetContent className="w-[90%] sm:max-w-[80vw] max-w-none h-screen overflow-y-auto">
        <SheetHeader>
          <SheetTitle className="text-xl font-semibold text-gray-700">
            Chỉnh sửa nhân viên: {contact?.fullName}
          </SheetTitle>
          <SheetDescription />
        </SheetHeader>

        <ContactForm
          methods={methods}
          isEditing={isEditing}
          onSubmit={handleSubmit}
        >
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
        </ContactForm>
      </SheetContent>
    </Sheet>
  );
};

export default DetailContactSheet;
