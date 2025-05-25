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
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select";
import { ProductFormSchema } from "./FormSchema";

interface BasicInfoFieldsProps {
  control: Control<ProductFormSchema>;
}

const formatType = (name: string) => {
  return name
    .toLowerCase()
    .normalize("NFD")
    .replace(/\p{Diacritic}/gu, "")
    .replace(/[^a-z0-9]+/g, "-")
    .replace(/(^-|-$)/g, "");
};

export const slugify = (str: string) =>
  str
    .trim()
    .toUpperCase()
    .normalize("NFD")
    .replace(/\p{Diacritic}/gu, "")
    .replace(/[^A-Z0-9]+/g, "-")
    .replace(/(^-|-$)/g, "");

export const buildSku = ({
  base,
  color,
  size,
}: {
  base: string;
  color?: string;
  size?: string;
}) =>
  [slugify(base), slugify(color || ""), slugify(size || "")]
    .filter(Boolean)
    .join("-");

export const BasicInfoFields: React.FC<BasicInfoFieldsProps> = ({
  control,
}) => {
  const nameValue = useWatch({ control, name: "name" });
  const codeValue = useWatch({ control, name: "code" });

  const { field: completeCodeField } = useController({
    control,
    name: "completeCode",
  });
  const { field: completeNameField } = useController({
    control,
    name: "completeName",
  });
  const { field: completePathField } = useController({
    control,
    name: "completePath",
  });

  React.useEffect(() => {
    if (nameValue) {
      const formattedType = formatType(nameValue);
      completeNameField.onChange(nameValue + " complete");
      completePathField.onChange(`/${formattedType}`);
    }
  }, [nameValue, completeNameField, completePathField]);

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
          name="code"
          render={({ field }) => (
            <FormItem>
              <FormLabel>Mã Sản Phẩm</FormLabel>
              <FormControl>
                <Input placeholder="Nhập mã sản phẩm" {...field} />
              </FormControl>
              <FormMessage />
            </FormItem>
          )}
        />

        <FormField
          control={control}
          name="name"
          render={({ field }) => (
            <FormItem>
              <FormLabel>Tên Sản Phẩm</FormLabel>
              <FormControl>
                <Input placeholder="Nhập tên sản phẩm" {...field} />
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
            <FormLabel>Mô Tả</FormLabel>
            <FormControl>
              <Textarea
                placeholder="Nhập mô tả sản phẩm"
                className="min-h-24"
                {...field}
              />
            </FormControl>
            <FormMessage />
          </FormItem>
        )}
      />

      <div className="grid grid-cols-2 gap-4">
        {/* <FormField
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
                  onChange={(e) =>
                    field.onChange(parseInt(e.target.value) || 0)
                  }
                />
              </FormControl>
              <FormDescription>
                Determines the display order of products
              </FormDescription>
              <FormMessage />
            </FormItem>
          )}
        /> */}

        <FormField
          control={control}
          name="status"
          render={({ field }) => (
            <FormItem>
              <FormLabel>Trạng thái</FormLabel>
              <Select onValueChange={field.onChange} defaultValue={field.value}>
                <FormControl>
                  <SelectTrigger>
                    <SelectValue placeholder="Chọn trạng thái" />
                  </SelectTrigger>
                </FormControl>
                <SelectContent>
                  <SelectItem value="Available">Có sẵn</SelectItem>
                  <SelectItem value="Unavailable">Không có sẵn</SelectItem>
                </SelectContent>
              </Select>
              <FormDescription>
                Trạng thái hiển thị của sản phẩm
              </FormDescription>
              <FormMessage />
            </FormItem>
          )}
        />
      </div>
    </>
  );
};
