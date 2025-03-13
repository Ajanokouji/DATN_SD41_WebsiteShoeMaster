// MetadataFieldCard.tsx
import React from "react";
import { UseFormReturn } from "react-hook-form";
import { Card, CardHeader, CardTitle, CardContent } from "@/components/ui/card";
import {
  FormControl,
  FormDescription,
  FormField,
  FormItem,
  FormLabel,
  FormMessage,
} from "@/components/ui/form";
import { Input } from "@/components/ui/input";
import { Button } from "@/components/ui/button";
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select";
import { Plus, Trash2 } from "lucide-react";
import { CategoryFormSchema } from "./FormSchema";
import { SelectionValuesList } from "./SelectionValuesListComponent";

interface MetadataFieldCardProps {
  index: number;
  form: UseFormReturn<CategoryFormSchema>;
  onRemove: () => void;
}

export const MetadataFieldCard: React.FC<MetadataFieldCardProps> = ({
  index,
  form,
  onRemove,
}) => {
  const fieldTypes = [
    { value: 0, label: "Text" },
    { value: 1, label: "Number" },
    { value: 2, label: "Date" },
    { value: 3, label: "Select" },
    { value: 4, label: "Checkbox" },
  ];

  const addFieldSelectionValue = () => {
    const currentMetadata = form.getValues().metadataObj || [];
    const currentSelectionValues =
      currentMetadata[index].fieldSelectionValues || [];

    const newSelectionValues = [
      ...currentSelectionValues,
      {
        key: String(currentSelectionValues.length),
        code: "",
        value: "",
        order: currentSelectionValues.length,
      },
    ];

    form.setValue(
      `metadataObj.${index}.fieldSelectionValues`,
      newSelectionValues
    );
  };

  return (
    <Card className="border border-gray-200">
      <CardHeader className="pb-2">
        <div className="flex justify-between items-center">
          <CardTitle className="text-base">Field #{index + 1}</CardTitle>
          <Button type="button" variant="ghost" size="sm" onClick={onRemove}>
            <Trash2 className="h-4 w-4 text-red-500" />
          </Button>
        </div>
      </CardHeader>
      <CardContent className="space-y-4">
        <div className="grid grid-cols-2 gap-4">
          <FormField
            control={form.control}
            name={`metadataObj.${index}.fieldName`}
            render={({ field }) => (
              <FormItem>
                <FormLabel>Field Name</FormLabel>
                <FormControl>
                  <Input placeholder="e.g. status" {...field} />
                </FormControl>
                <FormMessage />
              </FormItem>
            )}
          />

          <FormField
            control={form.control}
            name={`metadataObj.${index}.fieldDisplayName`}
            render={({ field }) => (
              <FormItem>
                <FormLabel>Display Name</FormLabel>
                <FormControl>
                  <Input placeholder="e.g. Status" {...field} />
                </FormControl>
                <FormMessage />
              </FormItem>
            )}
          />
        </div>

        <div className="grid grid-cols-2 gap-4">
          <FormField
            control={form.control}
            name={`metadataObj.${index}.fieldType`}
            render={({ field }) => (
              <FormItem>
                <FormLabel>Field Type</FormLabel>
                <Select
                  onValueChange={(value) => field.onChange(parseInt(value))}
                  defaultValue={field.value.toString()}
                >
                  <FormControl>
                    <SelectTrigger>
                      <SelectValue placeholder="Select field type" />
                    </SelectTrigger>
                  </FormControl>
                  <SelectContent>
                    {fieldTypes.map((type) => (
                      <SelectItem
                        key={type.value}
                        value={type.value.toString()}
                      >
                        {type.label}
                      </SelectItem>
                    ))}
                  </SelectContent>
                </Select>
                <FormMessage />
              </FormItem>
            )}
          />

          <FormField
            control={form.control}
            name={`metadataObj.${index}.fieldValueType`}
            render={({ field }) => (
              <FormItem>
                <FormLabel>Value Type</FormLabel>
                <FormControl>
                  <Input placeholder="e.g. string, number" {...field} />
                </FormControl>
                <FormMessage />
              </FormItem>
            )}
          />
        </div>

        {form.watch(`metadataObj.${index}.fieldType`) === 3 && (
          <div className="space-y-2">
            <div className="flex justify-between items-center">
              <FormLabel>Selection Values</FormLabel>
              <Button
                type="button"
                variant="outline"
                size="sm"
                onClick={addFieldSelectionValue}
              >
                <Plus className="h-3 w-3 mr-1" /> Add Option
              </Button>
            </div>

            <SelectionValuesList metadataIndex={index} form={form} />
          </div>
        )}

        <div className="grid grid-cols-1 gap-4">
          <FormField
            control={form.control}
            name={`metadataObj.${index}.fieldValues`}
            render={({ field }) => (
              <FormItem>
                <FormLabel>Default Values</FormLabel>
                <FormControl>
                  <Input placeholder="Default values" {...field} />
                </FormControl>
                <FormDescription>
                  Comma-separated default values
                </FormDescription>
                <FormMessage />
              </FormItem>
            )}
          />
        </div>
      </CardContent>
    </Card>
  );
};
