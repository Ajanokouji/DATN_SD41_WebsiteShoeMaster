import React from "react";
import { FormProvider, UseFormReturn } from "react-hook-form";
import { Input } from "@/components/ui/input";
import { 
  FormItem, 
  FormLabel, 
  FormControl, 
  FormMessage,
  Form
} from "@/components/ui/form";

interface FormData {
  voucherName: string;
  startDate: string;
  endDate: string;
  // Add other fields as needed
}

interface VoucherFormProps {
  methods: UseFormReturn<FormData>;
  isEditing: boolean;
  onSubmit: (values: FormData) => void;
  children: React.ReactNode;
}

export const VoucherForm: React.FC<VoucherFormProps> = ({
  methods,
  isEditing,
  onSubmit,
  children,
}) => {
  return (
    <FormProvider {...methods}>
      <Form {...methods}>
        <form onSubmit={methods.handleSubmit(onSubmit)} className="space-y-6 mt-6 pb-16">
          <FormItem>
            <FormLabel>Tên voucher</FormLabel>
            <FormControl>
              <Input 
                {...methods.register("voucherName")} 
                placeholder="Nhập tên voucher" 
                disabled={!isEditing} 
                className="w-full"
              />
            </FormControl>
            <FormMessage />
          </FormItem>

          <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
            <FormItem>
              <FormLabel>Ngày bắt đầu</FormLabel>
              <FormControl>
                <Input 
                  type="date"
                  {...methods.register("startDate")} 
                  disabled={!isEditing} 
                  className="w-full"
                />
              </FormControl>
              <FormMessage />
            </FormItem>

            <FormItem>
              <FormLabel>Ngày kết thúc</FormLabel>
              <FormControl>
                <Input 
                  type="date"
                  {...methods.register("endDate")} 
                  disabled={!isEditing} 
                  className="w-full"
                />
              </FormControl>
              <FormMessage />
            </FormItem>
          </div>

          {/* You can add more fields here */}

          {children}
        </form>
      </Form>
    </FormProvider>
  );
};