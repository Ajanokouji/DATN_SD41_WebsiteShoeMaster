import { Button } from "@/components/ui/button";
import { Form } from "@/components/ui/form";
import React, { useState } from "react";
import { useAppDispatch } from "@/hooks/use-app-dispatch";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import {
  Sheet,
  SheetContent,
  SheetDescription,
  SheetHeader,
  SheetTitle,
} from "@/components/ui/sheet";
import { BasicInfoFields } from "./BasicFields";
import { contactFormSchema, ContactFormSchema } from "./FormSchema";
import ContactReqDto from "@/types/contact/contact";
import { createContact } from "@/redux/apps/contact/contactSlice";

interface AddContactSheetProps {
  isOpen: boolean;
  onClose: () => void;
}

const AddContactSheet: React.FC<AddContactSheetProps> = ({
  isOpen,
  onClose,
}) => {
  const dispatch = useAppDispatch();
  const [isSubmitting, setIsSubmitting] = useState(false);

  const form = useForm<ContactFormSchema>({
    resolver: zodResolver(contactFormSchema),
    defaultValues: {
      name: "",
      fullName: "",
      address: "",
      phoneNumber: "",
      dateOfBirth: "",
      email: "",
      isdeleted: false,
      createdByUserId: "3fa85f64-5717-4562-b3fc-2c963f66afa6",
      lastModifiedByUserId: "3fa85f64-5717-4562-b3fc-2c963f66afa6",
      lastModifiedOnDate: new Date().toISOString(),
      createdOnDate: new Date().toISOString(),
    },
  });

  const handleSubmit = async (values: ContactFormSchema) => {
    setIsSubmitting(true);
    try {
      const contactData: ContactReqDto = {
        ...values,
        createdByUserId: values.createdByUserId || "",
        lastModifiedByUserId: values.lastModifiedByUserId || "",
        lastModifiedOnDate:
          values.lastModifiedOnDate || new Date().toISOString(),
        createdOnDate: values.createdOnDate || new Date().toISOString(),
        imageUrl: "",
        content: "",
      };

      await dispatch(createContact(contactData));

      onClose();
    } catch (error) {
      console.error("Create contact error details:", error);
    } finally {
      setIsSubmitting(false);
    }
  };

  return (
    <Sheet open={isOpen} onOpenChange={onClose}>
      <SheetContent className="w-[90%] sm:max-w-[80vw] max-w-none h-screen overflow-y-auto">
        <SheetHeader>
          <SheetTitle className="text-xl font-semibold text-gray-700">
            Thêm nhân viên
          </SheetTitle>
        </SheetHeader>

        <Form {...form}>
          <form
            onSubmit={form.handleSubmit(handleSubmit)}
            className="space-y-6"
          >
            <BasicInfoFields control={form.control} />

            <div className="flex justify-end gap-2 absolute bottom-4 left-0 w-full px-6">
              <Button type="submit" disabled={isSubmitting}>
                {isSubmitting ? "Đang tạo..." : "Thêm nhân viên"}
              </Button>
              <Button variant="outline" onClick={onClose} type="button">
                Hủy
              </Button>
            </div>
          </form>
        </Form>
      </SheetContent>
    </Sheet>
  );
};

export default AddContactSheet;
