// SelectionValuesList.tsx
import React from "react";
import { UseFormReturn } from "react-hook-form";
import {
  FormControl,
  FormField,
  FormItem,
  FormMessage,
} from "@/components/ui/form";
import { Input } from "@/components/ui/input";
import { Button } from "@/components/ui/button";
import { Trash2 } from "lucide-react";
import { CategoryFormSchema } from "./FormSchema";

interface SelectionValuesListProps {
  metadataIndex: number;
  form: UseFormReturn<CategoryFormSchema>;
}

export const SelectionValuesList: React.FC<SelectionValuesListProps> = ({ 
  metadataIndex,
  form 
}) => {
  const removeFieldSelectionValue = (valueIndex: number) => {
    const currentMetadata = form.getValues().metadataObj || [];
    const currentSelectionValues = currentMetadata[metadataIndex].fieldSelectionValues || [];
    
    form.setValue(
      `metadataObj.${metadataIndex}.fieldSelectionValues`,
      currentSelectionValues.filter((_, i) => i !== valueIndex)
    );
  };

  return (
    <>
      {form.watch(`metadataObj.${metadataIndex}.fieldSelectionValues`)?.map((_, valueIndex) => (
        <div key={valueIndex} className="flex items-center gap-2">
          <FormField
            control={form.control}
            name={`metadataObj.${metadataIndex}.fieldSelectionValues.${valueIndex}.code`}
            render={({ field }) => (
              <FormItem className="flex-1">
                <FormControl>
                  <Input placeholder="Code" {...field} />
                </FormControl>
                <FormMessage />
              </FormItem>
            )}
          />
          
          <FormField
            control={form.control}
            name={`metadataObj.${metadataIndex}.fieldSelectionValues.${valueIndex}.value`}
            render={({ field }) => (
              <FormItem className="flex-1">
                <FormControl>
                  <Input placeholder="Value" {...field} />
                </FormControl>
                <FormMessage />
              </FormItem>
            )}
          />
          
          <Button 
            type="button" 
            variant="ghost" 
            size="sm"
            onClick={() => removeFieldSelectionValue(valueIndex)}
          >
            <Trash2 className="h-4 w-4 text-red-500" />
          </Button>
        </div>
      ))}
    </>
  );
};