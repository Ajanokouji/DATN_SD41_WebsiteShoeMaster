import React from "react";
import { UseFormReturn } from "react-hook-form";
import { Button } from "@/components/ui/button";
import { Plus } from "lucide-react";
import { ProductFormSchema } from "./FormSchema";
import { VariantFieldCard } from "./VariantFieldCardComponent";

interface VariantSectionProps {
    form: UseFormReturn<ProductFormSchema>;
}

export const VariantSection: React.FC<VariantSectionProps> = ({ form }) => {
    const addVariant = () => {
        const currentVariants = form.getValues().variantObjs || [];
        form.setValue("variantObjs", [
            ...currentVariants,
            {
                id: "",
                productId: "",
                size: "",
                sizeType: "numeric",
                lowestAsk: 0,
            },
        ]);
    };

    const removeVariant = (index: number) => {
        const currentVariants = form.getValues().variantObjs || [];
        form.setValue(
            "variantObjs",
            currentVariants.filter((_, i) => i !== index)
        );
    };

    return (
        <div className="space-y-4">
            <div className="flex justify-between items-center">
                <h3 className="text-lg font-medium">Product Variants</h3>
                <Button type="button" variant="outline" size="sm" onClick={addVariant}>
                    <Plus className="h-4 w-4 mr-2" /> Add Variant
                </Button>
            </div>

            {form.watch("variantObjs")?.map((_, index) => (
                <VariantFieldCard
                    key={index}
                    index={index}
                    form={form}
                    onRemove={() => removeVariant(index)}
                />
            ))}
        </div>
    );
};

export default VariantSection;