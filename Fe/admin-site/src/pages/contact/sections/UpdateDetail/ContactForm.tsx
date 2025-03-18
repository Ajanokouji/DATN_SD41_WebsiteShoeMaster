import React from "react";
import { FormProvider, UseFormReturn } from "react-hook-form";
import { Input } from "@/components/ui/input";
import {
  FormItem,
  FormLabel,
  FormControl,
  FormMessage,
  Form,
} from "@/components/ui/form";

interface FormData {
  name: string;
  fullName: string;
  email: string;
  phoneNumber: string;
  dateOfBirth: string;
  address: string;
  content: string;
}

interface ContactFormProps {
  methods: UseFormReturn<FormData>;
  isEditing: boolean;
  isLoading?: boolean;
  onSubmit: (values: FormData) => void;
  children: React.ReactNode;
}

export const ContactForm: React.FC<ContactFormProps> = ({
  methods,
  isEditing,
  onSubmit,
  children,
}) => {
  return (
    <FormProvider {...methods}>
      <Form {...methods}>
        <form
          onSubmit={methods.handleSubmit(onSubmit)}
          className="space-y-6 mt-6 pb-16"
        >
          <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
            <FormItem>
              <FormLabel>Tên</FormLabel>
              <FormControl>
                <Input
                  {...methods.register("name")}
                  placeholder="Nhập tên"
                  disabled={!isEditing}
                  className="w-full"
                />
              </FormControl>
              <FormMessage />
            </FormItem>
            <FormItem>
              <FormLabel>Tên đầy đủ</FormLabel>
              <FormControl>
                <Input
                  {...methods.register("fullName")}
                  placeholder="Nhập tên đầy đủ"
                  disabled={!isEditing}
                  className="w-full"
                />
              </FormControl>
              <FormMessage />
            </FormItem>
          </div>
          <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
            <FormItem>
              <FormLabel>Ngày sinh</FormLabel>
              <FormControl>
                <Input
                  type="date"
                  {...methods.register("dateOfBirth")}
                  disabled={!isEditing}
                  className="w-full"
                />
              </FormControl>
              <FormMessage />
            </FormItem>
            <FormItem>
              <FormLabel>Địa chỉ</FormLabel>
              <FormControl>
                <Input
                  {...methods.register("address")}
                  placeholder="Nhập địa chỉ"
                  disabled={!isEditing}
                  className="w-full"
                />
              </FormControl>
              <FormMessage />
            </FormItem>
          </div>
          {children}
        </form>
      </Form>
    </FormProvider>
  );
};
