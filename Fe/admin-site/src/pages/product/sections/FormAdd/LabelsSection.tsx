import React from "react";
import { UseFormReturn } from "react-hook-form";
import { Button } from "@/components/ui/button";
import { Plus } from "lucide-react";
import { ProductFormSchema } from "./FormSchema";
import { LabelCard } from "./LabelCardComponent";

interface LabelsSectionProps {
  form: UseFormReturn<ProductFormSchema>;
}

export const LabelsSection: React.FC<LabelsSectionProps> = ({ form }) => {
  const addLabel = () => {
    const currentLabels = form.getValues().labelsObjs || [];
    form.setValue("labelsObjs", [
      ...currentLabels,
      {
        objectId: "",
        objectCode: "",
        objectName: "",
        color: "#808080",
      },
    ]);
  };

  const removeLabel = (index: number) => {
    const currentLabels = form.getValues().labelsObjs || [];
    form.setValue(
      "labelsObjs",
      currentLabels.filter((_, i) => i !== index)
    );
  };

  return (
    <div className="space-y-4">
      <div className="flex justify-between items-center">
        <h3 className="text-lg font-medium">Nhãn sản phẩm</h3>
        <Button type="button" variant="outline" size="sm" onClick={addLabel}>
          <Plus className="h-4 w-4 mr-2" /> Thêm nhãn dán
        </Button>
      </div>

      {form.watch("labelsObjs")?.map((_, index) => (
        <LabelCard
          key={index}
          index={index}
          form={form}
          onRemove={() => removeLabel(index)}
        />
      ))}
    </div>
  );
};
