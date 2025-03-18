import { useEffect, useState } from "react";
import { useForm } from "react-hook-form";
import { useAppDispatch } from "@/hooks/use-app-dispatch";
import { fetchContactById, updateContact } from "@/redux/apps/contact/contactSlice";


interface FormData {
  name: string;
  fullName: string;
  address: string;
  email: string;
  phoneNumber: string;
  content: string;
  dateOfBirth: string;

}

export const useContactForm = (
  contactId: string,
  // eslint-disable-next-line @typescript-eslint/no-explicit-any
  contact: any,
  onClose: () => void
) => {
  const dispatch = useAppDispatch();
  const [isEditing, setIsEditing] = useState(false);
  const [isLoading, setIsLoading] = useState(false);

  // Initialize form with empty values
  const methods = useForm<FormData>({
    defaultValues: {
      name: "",
      fullName: "",
      address: "",
      email: "",
      phoneNumber: "",
      content: "",
      dateOfBirth: "",
    },
  });

  useEffect(() => {
    if (contact) {
      methods.reset({
        name: contact.name || "",
        fullName: contact.fullName || "",
        address: contact.address || "",
        email: contact.email || "",
        phoneNumber: contact.phoneNumber || "",
        content: contact.content || "",
        dateOfBirth: contact.dateOfBirth || "",
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

  const handleSubmit = (value: FormData) => {
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
