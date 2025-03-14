import React from "react";
import { UseFormReturn } from "react-hook-form";
import { Button } from "@/components/ui/button";
import { Plus } from "lucide-react";
import { ProductFormSchema } from "./FormSchema";
import { MetadataFieldCard } from "./MetadataFieldCardComponent";

interface MetadataSectionProps {
  form: UseFormReturn<ProductFormSchema>;
}

export const MetadataSection: React.FC<MetadataSectionProps> = ({ form }) => {
  const addMetadata = () => {
    const currentMetadata = form.getValues().metadataObj || [];
    form.setValue("metadataObj", [
      ...currentMetadata,
      {
        fieldName: "",
        fieldDisplayName: "",
        fieldType: 1,
        fieldValues: "",
        fieldValueTexts: "",
        fieldValueType: "string",
        fieldSelectionValues: [],
      },
    ]);
  };

  const removeMetadata = (index: number) => {
    const currentMetadata = form.getValues().metadataObj || [];
    form.setValue(
      "metadataObj",
      currentMetadata.filter((_, i) => i !== index)
    );
  };

  return (
    <div className="space-y-4">
      <div className="flex justify-between items-center">
        <h3 className="text-lg font-medium">Metadata Fields</h3>
        <Button type="button" variant="outline" size="sm" onClick={addMetadata}>
          <Plus className="h-4 w-4 mr-2" /> Add Field
        </Button>
      </div>

      {form.watch("metadataObj")?.map((_, index) => (
        <MetadataFieldCard
          key={index}
          index={index}
          form={form}
          onRemove={() => removeMetadata(index)}
        />
      ))}
    </div>
  );
};
