import React from "react";
import { FormProvider, UseFormReturn } from "react-hook-form";
import { Input } from "@/components/ui/input";
import {
  FormItem,
  FormLabel,
  FormControl,
  FormMessage,
  Form,
  FormField,
} from "@/components/ui/form";
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from "@/components/ui/select";
import { ContactFormSchema } from "../FormAdd/FormSchema"; // Thêm dòng này

interface FormData {
  name: string;
  fullName: string;
  email: string;
  phoneNumber: string;
  dateOfBirth: string;
  address: string;
}

interface ContactFormProps {
  methods: UseFormReturn<ContactFormSchema>; // Sửa lại type ở đây
  isEditing: boolean;
  isLoading?: boolean;
  onSubmit: (values: ContactFormSchema) => void; // Sửa lại type ở đây
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
            <FormField
              control={methods.control}
              name="name"
              render={({ field }) => (
                <FormItem>
                  <FormLabel>Mã nhân viên</FormLabel>
                  <FormControl>
                    <Input
                      {...field}
                      placeholder="Nhập mã nhân viên"
                      disabled={!isEditing}
                      className="w-full"
                    />
                  </FormControl>
                  <FormMessage />
                </FormItem>
              )}
            />
            <FormField
              control={methods.control}
              name="fullName"
              render={({ field }) => (
                <FormItem>
                  <FormLabel>Họ tên</FormLabel>
                  <FormControl>
                    <Input
                      {...field}
                      placeholder="Nhập tên đầy đủ"
                      disabled={!isEditing}
                      className="w-full"
                    />
                  </FormControl>
                  <FormMessage />
                </FormItem>
              )}
            />
          </div>
          <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
            <FormField
              control={methods.control}
              name="dateOfBirth"
              render={({ field }) => (
                <FormItem>
                  <FormLabel>Năm sinh</FormLabel>
                  <FormControl>
                    <Input
                      type="date"
                      {...field}
                      disabled={!isEditing}
                      className="w-full"
                    />
                  </FormControl>
                  <FormMessage />
                </FormItem>
              )}
            />
            <FormField
              control={methods.control}
              name="address"
              render={({ field }) => (
                <FormItem>
                  <FormLabel>Địa chỉ</FormLabel>
                  <FormControl>
                    <Input
                      {...field}
                      placeholder="Nhập địa chỉ"
                      disabled={!isEditing}
                      className="w-full"
                    />
                  </FormControl>
                  <FormMessage />
                </FormItem>
              )}
            />
          </div>
          <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
            <FormField
              control={methods.control}
              name="email"
              render={({ field }) => (
                <FormItem>
                  <FormLabel>Email</FormLabel>
                  <FormControl>
                    <Input
                      {...field}
                      placeholder="Nhập email"
                      disabled={!isEditing}
                      className="w-full"
                    />
                  </FormControl>
                  <FormMessage />
                </FormItem>
              )}
            />
            <FormField
              control={methods.control}
              name="phoneNumber"
              render={({ field }) => (
                <FormItem>
                  <FormLabel>Số điện thoại</FormLabel>
                  <FormControl>
                    <Input
                      {...field}
                      placeholder="Nhập số điện thoại"
                      disabled={!isEditing}
                      className="w-full"
                    />
                  </FormControl>
                  <FormMessage />
                </FormItem>
              )}
            />
          </div>
          {children}
        </form>
      </Form>
    </FormProvider>
  );
};
