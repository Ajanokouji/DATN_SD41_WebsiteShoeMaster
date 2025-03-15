import React from "react";
import { Control } from "react-hook-form";
import {
  FormControl,
  FormField,
  FormItem,
  FormLabel,
  FormMessage,
} from "@/components/ui/form";
import { Input } from "@/components/ui/input";
import { VoucherFormSchema } from "./FormSchema";

interface BasicInfoFieldsProps {
  control: Control<VoucherFormSchema>;
}

export const BasicInfoFields: React.FC<BasicInfoFieldsProps> = ({
  control,
}) => {
  return (
    <>
      <div className="grid grid-cols-2 gap-4">
        <FormField
          control={control}
          name="voucherName"
          render={({ field }) => (
            <FormItem>
              <FormLabel>Code</FormLabel>
              <FormControl>
                <Input placeholder="Voucher name" {...field} />
              </FormControl>
              <FormMessage />
            </FormItem>
          )}
        />
      </div>
      <div className="grid grid-cols-2 gap-4">
        <FormField
          control={control}
          name="startDate"
          render={({ field: { onChange, value, ...fieldProps } }) => (
            <FormItem>
              <FormLabel>Start date</FormLabel>
              <FormControl>
                <Input 
                  type="date" 
                  placeholder="Start date"
                  value={value ? new Date(value).toISOString().split('T')[0] : ''}
                  onChange={(e) => {
                    const date = e.target.value;
                    onChange(date);
                  }}
                  {...fieldProps} 
                />
              </FormControl>
              <FormMessage />
            </FormItem>
          )}
        />
        <FormField
          control={control}
          name="endDate"
          render={({ field: { onChange, value, ...fieldProps } }) => (
            <FormItem>
              <FormLabel>End date</FormLabel>
              <FormControl>
                <Input 
                  type="date" 
                  placeholder="End date"
                  value={value ? new Date(value).toISOString().split('T')[0] : ''}
                  onChange={(e) => {
                    const date = e.target.value;
                    onChange(date);
                  }}
                  {...fieldProps} 
                />
              </FormControl>
              <FormMessage />
            </FormItem>
          )}
        />
      </div>
    </>
  );
};