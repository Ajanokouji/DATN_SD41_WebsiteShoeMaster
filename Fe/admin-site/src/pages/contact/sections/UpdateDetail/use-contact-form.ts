import { useEffect, useState } from "react";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { useAppDispatch } from "@/hooks/use-app-dispatch";
import { fetchContactById, updateContact } from "@/redux/apps/contact/contactSlice";
import { contactFormSchema, ContactFormSchema } from "../FormAdd/FormSchema";

// Sử dụng ContactFormSchema làm type duy nhất
export const useContactForm = (
  contactId: string,
  contact: any,
  onClose: () => void
) => {
  const dispatch = useAppDispatch();
  const [isEditing, setIsEditing] = useState(false);
  const [isLoading, setIsLoading] = useState(false);

  // Đảm bảo defaultValues có đầy đủ các trường như schema
  const methods = useForm<ContactFormSchema>({
    resolver: zodResolver(contactFormSchema),
    defaultValues: {
      name: "",
      fullName: "",
      address: "",
      email: "",
      phoneNumber: "",
      dateOfBirth: "",
      isdeleted: false,
      createdByUserId: "",
      lastModifiedByUserId: "",
      lastModifiedOnDate: "",
      createdOnDate: "",
    },
  });

  // Thêm hàm format
  function formatDate(dateString?: string): string {
    if (!dateString) return "";
    const d = new Date(dateString);
    if (isNaN(d.getTime())) return "";
    // Lấy ngày theo local timezone thay vì UTC
    const year = d.getFullYear();
    const month = String(d.getMonth() + 1).padStart(2, "0");
    const day = String(d.getDate()).padStart(2, "0");
    return `${year}-${month}-${day}`;
  }

  useEffect(() => {
    if (contact) {
      methods.reset({
        name: contact.name || "",
        fullName: contact.fullName || "",
        address: contact.address || "",
        email: contact.email || "",
        phoneNumber: contact.phoneNumber || "",
        dateOfBirth: formatDate(contact.dateOfBirth),
        isdeleted: contact.isdeleted ?? false,
        createdByUserId: contact.createdByUserId || "",
        lastModifiedByUserId: contact.lastModifiedByUserId || "",
        lastModifiedOnDate: contact.lastModifiedOnDate || "",
        createdOnDate: contact.createdOnDate || "",
      });
    }
  }, [contact, methods]);

  // Fetch contact data when contactId changes
  useEffect(() => {
    if (contactId) {
      setIsLoading(true);
      dispatch(fetchContactById(contactId)).finally(() => setIsLoading(false));
    }
  }, [dispatch, contactId]);

  const handleSubmit = (value: ContactFormSchema) => {
    const updatedContact = {
      ...contact,
      ...value,
    };

    dispatch(updateContact({ id: contactId, data: updatedContact }));
    setIsEditing(false);
    onClose();
  };

  return {
    isEditing,
    setIsEditing,
    isLoading,
    methods,
    handleSubmit,
  };
};
