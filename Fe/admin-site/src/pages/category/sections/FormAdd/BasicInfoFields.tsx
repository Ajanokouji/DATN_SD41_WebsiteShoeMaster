// BasicInfoFields.tsx
import React from "react";
import { Control, useController, useWatch } from "react-hook-form";
import {
  FormControl,
  FormDescription,
  FormField,
  FormItem,
  FormLabel,
  FormMessage,
} from "@/components/ui/form";
import { Input } from "@/components/ui/input";
import { Textarea } from "@/components/ui/textarea";
import { CategoryFormSchema } from "./FormSchema";

interface BasicInfoFieldsProps {
  control: Control<CategoryFormSchema>;
}

const formatType = (name: string) => {
  return name
    .toLowerCase()
    .normalize("NFD")
    .replace(/\p{Diacritic}/gu, "")
    .replace(/[^a-z0-9]+/g, "-")
    .replace(/(^-|-$)/g, "");
};

export const BasicInfoFields: React.FC<BasicInfoFieldsProps> = ({ control }) => {
  const nameValue = useWatch({ control, name: "name" });
  const codeValue = useWatch({ control, name: "code" });

  const { field: typeField } = useController({ control, name: "type" });
  const { field: completeCodeField } = useController({ control, name: "completeCode" });
  const { field: completeNameField } = useController({ control, name: "completeName" });
  const { field: completePathField } = useController({ control, name: "completePath" });


  React.useEffect(() => {
    if (nameValue) {
      const formattedType = formatType(nameValue);
      typeField.onChange(formattedType);
      completeNameField.onChange(nameValue + " complete");
      completePathField.onChange(`/${formattedType}`);
    }
  }, [nameValue, typeField, completeNameField, completePathField]);

  React.useEffect(() => {
    if (codeValue) {
      completeCodeField.onChange(codeValue + " complete");
    }
  }, [codeValue, completeCodeField]);
  
  return (
    <>
      <div className="grid grid-cols-2 gap-4">
      <FormField
          control={control}
          name="name"
          render={({ field }) => (
            <FormItem>
              <FormLabel>Tên</FormLabel>
              <FormControl>
                <Input placeholder="Tên danh mục" {...field} />
              </FormControl>
              <FormMessage />
            </FormItem>
          )}
        />

        <FormField
          control={control}
          name="code"
          render={({ field }) => (
            <FormItem>
              <FormLabel>Mã danh mục</FormLabel>
              <FormControl>
                <Input placeholder="Mã danh mục"
                  {...field}
                  onFocus={() => {
                    if (nameValue) {
                      const generated = formatType(nameValue);
                      field.onChange(generated);
                    }
                  }} />
              </FormControl>
              <FormMessage />
            </FormItem>
          )}
        />
      </div>

      <FormField
        control={control}
        name="description"
        render={({ field }) => (
          <FormItem>
            <FormLabel>Mô tả</FormLabel>
            <FormControl>
              <Textarea
                placeholder="Nhập mô tả danh mục"
                className="min-h-24"
                {...field}
              />
            </FormControl>
            <FormMessage />
          </FormItem>
        )}
      />

      <FormField
        control={control}
        name="sortOrder"
        render={({ field }) => (
          <FormItem>
            <FormLabel>Sort Order</FormLabel>
            <FormControl>
              <Input
                type="number"
                placeholder="0"
                {...field}
                onChange={(e) => field.onChange(parseInt(e.target.value) || 0)}
              />
            </FormControl>
            <FormDescription>
              Determines the display order of categories
            </FormDescription>
            <FormMessage />
          </FormItem>
        )}
      />
    </>
  );
};
