import { Button } from "@/components/ui/button";
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogFooter,
  DialogHeader,
  DialogTitle,
} from "@/components/ui/dialog";
import { Form } from "@/components/ui/form";
import React, { useState } from "react";
import { useAppDispatch } from "@/hooks/use-app-dispatch";
import { createCategory } from "@/redux/apps/category/categorySlice";
import CategoryReqDto from "@/types/category/category";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { CategoryFormSchema, categoryFormSchema } from "./FormSchema";
import { BasicInfoFields } from "./BasicInfoFields";
import { MetadataSection } from "./MetadataSection";

interface AddCategoryDialogProps {
  isOpen: boolean;
  onClose: () => void;
}

const AddCategoryDialog: React.FC<AddCategoryDialogProps> = ({
  isOpen,
  onClose,
}) => {
  const dispatch = useAppDispatch();
  const [isSubmitting, setIsSubmitting] = useState(false);

  const form = useForm<CategoryFormSchema>({
    resolver: zodResolver(categoryFormSchema),
    defaultValues: {
      code: "",
      name: "",
      description: "",
      metadataObj: [],
      sortOrder: 0,
      createdByUserId: "",
      lastModifiedByUserId: "",
      lastModifiedDate: new Date().toISOString(),
      createdOnDate: new Date().toISOString(),
    },
  });

  const handleSubmit = async (values: CategoryFormSchema) => {
    setIsSubmitting(true);
    try {
      // Convert to appropriate type
      const categoryData: CategoryReqDto = {
        ...values,
        sortOrder: values.sortOrder || 0,
        createdByUserId: values.createdByUserId || "",
        lastModifiedByUserId: values.lastModifiedByUserId || "",
        lastModifiedDate: values.lastModifiedDate || new Date().toISOString(),
        createdOnDate: values.createdOnDate || new Date().toISOString(),
        metadataObj: values.metadataObj || [],
      };

      await dispatch(createCategory(categoryData));

      onClose();
    } catch (error) {
      console.error("Create category error details:", error);
    } finally {
      setIsSubmitting(false);
    }
  };

  return (
    <Dialog open={isOpen} onOpenChange={onClose}>
      <DialogContent className="max-w-4xl max-h-[90vh] overflow-y-auto">
        <DialogHeader>
          <DialogTitle className="text-xl font-semibold text-gray-700">
            Add Category
          </DialogTitle>
          <DialogDescription>
            Create a new category with custom metadata fields
          </DialogDescription>
        </DialogHeader>

        <Form {...form}>
          <form
            onSubmit={form.handleSubmit(handleSubmit)}
            className="space-y-6"
          >
            <BasicInfoFields control={form.control} />
            <MetadataSection form={form} />

            <DialogFooter className="gap-2 pt-4">
              <Button type="submit" disabled={isSubmitting}>
                {isSubmitting ? "Creating..." : "Add new category"}
              </Button>
              <Button variant="outline" onClick={onClose} type="button">
                Cancel
              </Button>
            </DialogFooter>
          </form>
        </Form>
      </DialogContent>
    </Dialog>
  );
};

export default AddCategoryDialog;
