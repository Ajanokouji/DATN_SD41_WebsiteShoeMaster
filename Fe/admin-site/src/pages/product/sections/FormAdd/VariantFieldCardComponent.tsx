import React from "react";
import { UseFormReturn } from "react-hook-form";
import { Card, CardHeader, CardTitle, CardContent } from "@/components/ui/card";
import {
    FormControl,
    FormField,
    FormItem,
    FormLabel,
    FormMessage,
} from "@/components/ui/form";
import { Input } from "@/components/ui/input";
import { Button } from "@/components/ui/button";
import { Trash2 } from "lucide-react";
import { ProductFormSchema } from "./FormSchema";
import {
    Select,
    SelectContent,
    SelectItem,
    SelectTrigger,
    SelectValue,
} from "@/components/ui/select";

interface VariantFieldCardProps {
    index: number;
    form: UseFormReturn<ProductFormSchema>;
    onRemove: () => void;
}

export const VariantFieldCard: React.FC<VariantFieldCardProps> = ({
    index,
    form,
    onRemove,
}) => {
    const sizeTypes = [
        { value: "numeric", label: "Numeric" },
        { value: "alpha", label: "Alphabetic" },
        { value: "eu", label: "European" },
        { value: "us", label: "US" },
        { value: "us w", label: "US W" },
        { value: "us m", label: "US M" },
        { value: "jp", label: "Japanese" },
        { value: "kr", label: "Korean" },
        { value: "uk", label: "UK" },
        { value: "cm", label: "Centimeters" },
        { value: "in", label: "Inches" },
        { value: "mm", label: "Millimeters" },
        { value: "eu w", label: "European W" },
    ];

    return (
        <Card className="border border-gray-200">
            <CardHeader className="pb-2">
                <div className="flex justify-between items-center">
                    <CardTitle className="text-base">Variant #{index + 1}</CardTitle>
                    <Button type="button" variant="ghost" size="sm" onClick={onRemove}>
                        <Trash2 className="h-4 w-4 text-red-500" />
                    </Button>
                </div>
            </CardHeader>
            <CardContent className="space-y-4">
                <div className="grid grid-cols-2 gap-4">
                    <FormField
                        control={form.control}
                        name={`variantObjs.${index}.size`}
                        render={({ field }) => (
                            <FormItem>
                                <FormLabel>Size</FormLabel>
                                <FormControl>
                                    <Input placeholder="e.g. M, 42, 10.5" {...field} />
                                </FormControl>
                                <FormMessage />
                            </FormItem>
                        )}
                    />

                    <FormField
                        control={form.control}
                        name={`variantObjs.${index}.sizeType`}
                        render={({ field }) => (
                            <FormItem>
                                <FormLabel>Size Type</FormLabel>
                                <Select
                                    onValueChange={field.onChange}
                                    defaultValue={field.value}
                                >
                                    <FormControl>
                                        <SelectTrigger>
                                            <SelectValue placeholder="Select size type" />
                                        </SelectTrigger>
                                    </FormControl>
                                    <SelectContent>
                                        {sizeTypes.map((type) => (
                                            <SelectItem key={type.value} value={type.value}>
                                                {type.label}
                                            </SelectItem>
                                        ))}
                                    </SelectContent>
                                </Select>
                                <FormMessage />
                            </FormItem>
                        )}
                    />
                </div>

                <FormField
                    control={form.control}
                    name={`variantObjs.${index}.lowestAsk`}
                    render={({ field }) => (
                        <FormItem>
                            <FormLabel>Lowest Ask Price</FormLabel>
                            <FormControl>
                                <Input
                                    type="number"
                                    placeholder="0.00"
                                    {...field}
                                    onChange={(e) => field.onChange(parseFloat(e.target.value))}
                                />
                            </FormControl>
                            <FormMessage />
                        </FormItem>
                    )}
                />

                {/* Hidden fields that may be needed for form submission */}
                <input
                    type="hidden"
                    {...form.register(`variantObjs.${index}.id`)}
                    defaultValue=""
                />
                <input
                    type="hidden"
                    {...form.register(`variantObjs.${index}.productId`)}
                    defaultValue=""
                />
            </CardContent>
        </Card>
    );
};

// Component to manage a list of variants
export const VariantsList: React.FC<{
    form: UseFormReturn<ProductFormSchema>;
}> = ({ form }) => {
    const variants = form.watch("variantObjs") || [];

    const addVariant = () => {
        const currentVariants = form.getValues().variantObjs || [];

        const newVariant = {
            id: "",
            productId: "",
            size: "",
            sizeType: "numeric",
            lowestAsk: 0,
        };

        form.setValue("variantObjs", [...currentVariants, newVariant]);
    };

    const removeVariant = (index: number) => {
        const currentVariants = form.getValues().variantObjs || [];
        const updatedVariants = currentVariants.filter((_, i) => i !== index);
        form.setValue("variantObjs", updatedVariants);
    };

    return (
        <div className="space-y-4">
            {variants.map((_, index) => (
                <VariantFieldCard
                    key={index}
                    index={index}
                    form={form}
                    onRemove={() => removeVariant(index)}
                />
            ))}

            <Button
                type="button"
                variant="outline"
                onClick={addVariant}
                className="w-full"
            >
                Add Variant
            </Button>
        </div>
    );
};