import React from "react";
import { FormProvider, UseFormReturn } from "react-hook-form";
import {
  Form,
  FormField,
  FormItem,
  FormLabel,
  FormControl,
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
import { VoucherFormSchema } from "./FormSchema";

interface VoucherFormProps {
  methods: UseFormReturn<VoucherFormSchema>;
  isEditing: boolean;
  isLoading?: boolean;
  onSubmit: (values: VoucherFormSchema) => void;
  children?: React.ReactNode;
}

export const VoucherForm: React.FC<VoucherFormProps> = ({
  methods,
  isEditing,
  onSubmit,
  children,
}) => {
  const { control, watch, setValue, trigger } = methods;
  const discountType = watch("discountType");

  const handleDiscountTypeChange = (val: "$" | "%") => {
    setValue("discountType", val);

    if (val === "$") {
      trigger("discountAmount");
    } else {
      trigger("discountPercentage");
    }
  };

  return (
    <FormProvider {...methods}>
      <Form {...methods}>
        <form
          onSubmit={methods.handleSubmit(onSubmit)}
          className="space-y-4 mt-6"
        >
          <label className="flex text-sm text-gray-500">
            Ngày tạo: {new Date(methods.getValues("createdOnDate")?.replace(" ", "T") + "Z").toLocaleString() || "Chưa có dữ liệu"}
          </label>
          <label className="text-sm text-gray-500">
            Lần cuối chỉnh sửa: {new Date(methods.getValues("lastModifiedOnDate")?.replace(" ", "T") + "Z").toLocaleString() || "Chưa có dữ liệu"}
          </label>

          <div className="grid grid-cols-2 gap-4">
            <FormField
              control={control}
              name="voucherName"
              render={({ field }) => (
                <FormItem>
                  <FormLabel>Tên Voucher</FormLabel>
                  <FormControl>
                    <Input {...field} placeholder="Nhập tên voucher" disabled={!isEditing} />
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
                  <FormLabel>Mã Voucher</FormLabel>
                  <FormControl>
                    <Input {...field} placeholder="Nhập mã voucher" disabled={!isEditing} />
                  </FormControl>
                  <FormMessage />
                </FormItem>
              )}
            />
          </div>

          <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
            <FormField
              control={control}
              name="startDate"
              render={({ field }) => (
                <FormItem>
                  <FormLabel>Ngày bắt đầu</FormLabel>
                  <FormControl>
                    <Input
                      type="datetime-local"
                      step={1}
                      {...field}
                      disabled={!isEditing}
                      onChange={(e) => {
                        field.onChange(e);
                        trigger("startDate");
                        trigger("endDate");
                      }}
                    />
                  </FormControl>
                  <FormMessage />
                </FormItem>
              )}
            />
            <FormField
              control={control}
              name="endDate"
              render={({ field }) => (
                <FormItem>
                  <FormLabel>Ngày kết thúc</FormLabel>
                  <FormControl>
                    <Input type="datetime-local" step={1} {...field} disabled={!isEditing} 
                    onChange={(e) => {
                      field.onChange(e);
                      trigger("endDate");
                      trigger("startDate");
                    }}/>
                  </FormControl>
                  <FormMessage />
                </FormItem>
              )}
            />
          </div>

          <div className="grid grid-cols-2 gap-4">
            {discountType === "$" && (
              <FormField
                control={control}
                name="discountAmount"
                render={({ field }) => (
                  <FormItem>
                    <FormLabel>Giá trị giảm</FormLabel>
                    <div className="flex items-center gap-2">
                      <FormControl>
                        <Input
                          type="number"
                          placeholder="Nhập số tiền giảm"
                          {...field}
                          disabled={!isEditing}
                          onChange={(e) => {
                            field.onChange(e);
                            trigger("discountAmount");
                          }}
                        />
                      </FormControl>
                      <Select
                        value={discountType}
                        onValueChange={(val) => handleDiscountTypeChange(val as "$" | "%")}
                        disabled={!isEditing}
                      >
                        <SelectTrigger className="w-16">
                          <SelectValue />
                        </SelectTrigger>
                        <SelectContent>
                          <SelectItem value="$">$</SelectItem>
                          <SelectItem value="%">%</SelectItem>
                        </SelectContent>
                      </Select>
                    </div>
                    <FormMessage />
                  </FormItem>
                )}
              />
            )}

            {discountType === "%" && (
              <FormField
                control={control}
                name="discountPercentage"
                render={({ field }) => (
                  <FormItem>
                    <FormLabel>Giá trị giảm</FormLabel>
                    <div className="flex items-center gap-2">
                      <FormControl>
                        <Input
                          type="number"
                          placeholder="Nhập phần trăm giảm"
                          {...field}
                          disabled={!isEditing}
                          onChange={(e) => {
                            field.onChange(e);
                            trigger("discountPercentage");
                          }}
                        />
                      </FormControl>
                      <Select
                        value={discountType}
                        onValueChange={(val) => handleDiscountTypeChange(val as "$" | "%")}
                        disabled={!isEditing}
                      >
                        <SelectTrigger className="w-16">
                          <SelectValue />
                        </SelectTrigger>
                        <SelectContent>
                          <SelectItem value="$">$</SelectItem>
                          <SelectItem value="%">%</SelectItem>
                        </SelectContent>
                      </Select>
                    </div>
                    <FormMessage />
                  </FormItem>
                )}
              />
            )}

            <FormField
              control={control}
              name="minimumOrderAmount"
              render={({ field }) => (
                <FormItem>
                  <FormLabel>Giá trị đơn hàng tối thiểu</FormLabel>
                  <FormControl>
                    <Input
                      type="number"
                      {...field}
                      placeholder="Nhập giá trị đơn hàng tối thiểu"
                      disabled={!isEditing}
                    />
                  </FormControl>
                  <FormMessage />
                </FormItem>
              )}
            />
          </div>

          <div className="grid grid-cols-2 gap-4">
            <FormField
              control={control}
              name="status"
              render={({ field }) => (
                <FormItem>
                  <FormLabel>Trạng thái</FormLabel>
                  <FormControl>
                    <Select
                      disabled={!isEditing}
                      value={String(field.value)}
                      onValueChange={(val) => field.onChange(Number(val))}
                    >
                      <SelectTrigger>
                        <SelectValue placeholder="Chọn trạng thái" />
                      </SelectTrigger>
                      <SelectContent>
                        <SelectItem value="1">Hoạt động</SelectItem>
                        <SelectItem value="0">Dừng hoạt động</SelectItem>
                      </SelectContent>
                    </Select>
                  </FormControl>
                  <FormMessage />
                </FormItem>
              )}
            />

            <FormField
              control={control}
              name="voucherType"
              render={({ field }) => (
                <FormItem>
                  <FormLabel>Loại mã</FormLabel>
                  <FormControl>
                    <Select
                      disabled={!isEditing}
                      value={String(field.value)}
                      onValueChange={(val) => field.onChange(Number(val))}
                    >
                      <SelectTrigger>
                        <SelectValue placeholder="Chọn loại mã" />
                      </SelectTrigger>
                      <SelectContent>
                        <SelectItem value="1">Voucher toàn shop</SelectItem>
                        <SelectItem value="2">Voucher sản phẩm</SelectItem>
                      </SelectContent>
                    </Select>
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
                    {...field}
                    placeholder="Mô tả ngắn..."
                    disabled={!isEditing}
                  />
                </FormControl>
                <FormMessage />
              </FormItem>
            )}
          />

          {children}
        </form>
      </Form>
    </FormProvider>
  );
};