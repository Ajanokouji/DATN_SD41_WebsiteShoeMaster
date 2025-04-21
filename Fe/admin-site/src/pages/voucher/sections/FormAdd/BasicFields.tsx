import React, { useEffect } from "react";
import { Control, useFormContext } from "react-hook-form";
import {
  FormControl,
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
import { VoucherFormSchema } from "./FormSchema";

interface BasicInfoFieldsProps {
  control: Control<VoucherFormSchema>;
}

export const BasicInfoFields: React.FC<BasicInfoFieldsProps> = ({ control }) => {
  const { watch, setValue, trigger } = useFormContext();
  const discountType = watch("discountType");

  const handleDiscountTypeChange = (val: "$" | "%") => {
    setValue("discountType", val);

    if (val === "$") {
      trigger("discountAmount");
    } else {
      trigger("discountPercentage");
    }
  };

  useEffect(() => {
    setValue("discountType", "$");
    setValue("status", 1);
  }, [setValue]);

  return (
    <div className="space-y-4">
      <div className="grid grid-cols-2 gap-4">
        <FormField
          control={control}
          name="voucherName"
          render={({ field }) => (
            <FormItem>
              <FormLabel>Tên Voucher</FormLabel>
              <FormControl>
                <Input
                  placeholder="Nhập tên voucher"
                  {...field}
                  onChange={(e) => {
                    field.onChange(e);
                    trigger("voucherName");
                  }}
                />
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
                <Input
                  placeholder="Nhập mã voucher"
                  {...field}
                  onChange={(e) => {
                    field.onChange(e);
                    trigger("code");
                  }}
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
          name="startDate"
          render={({ field }) => (
            <FormItem>
              <FormLabel>Ngày bắt đầu</FormLabel>
              <FormControl>
                <Input
                  type="datetime-local"
                  step={1}
                  {...field}
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
                <Input
                  type="datetime-local"
                  step={1}
                  {...field}
                  onChange={(e) => {
                    field.onChange(e);
                    trigger("endDate");
                    trigger("startDate");
                  }}
                />
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
                      onChange={(e) => {
                        field.onChange(e);
                        trigger("discountAmount");
                      }}
                    />
                  </FormControl>
                  <Select
                    value={discountType}
                    onValueChange={(val) => handleDiscountTypeChange(val as "$" | "%")}
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
                      onChange={(e) => {
                        field.onChange(e);
                        trigger("discountPercentage");
                      }}
                    />
                  </FormControl>
                  <Select
                    value={discountType}
                    onValueChange={(val) => handleDiscountTypeChange(val as "$" | "%")}
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
                  placeholder="Nhập giá trị đơn hàng tối thiểu"
                  {...field}
                  onChange={(e) => {
                    field.onChange(e);
                    trigger("minimumOrderAmount");
                  }}
                />
              </FormControl>
              <FormMessage />
            </FormItem>
          )}
        />
      </div>

      <div className="grid grid-cols-1 gap-4">
        <FormField
          control={control}
          name="status"
          render={({ field }) => (
            <FormItem>
              <FormLabel>Trạng thái</FormLabel>
              <Select
                value={String(field.value)}
                onValueChange={(val) => {
                  field.onChange(Number(val));
                  trigger("status");
                }}
              >
                <FormControl>
                  <SelectTrigger>
                    <SelectValue placeholder="Chọn trạng thái" />
                  </SelectTrigger>
                </FormControl>
                <SelectContent>
                  <SelectItem value="1">Hoạt động</SelectItem>
                  <SelectItem value="0">Không hoạt động</SelectItem>
                </SelectContent>
              </Select>
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
                placeholder="Mô tả ngắn..."
                {...field}
                onChange={(e) => {
                  field.onChange(e);
                  trigger("description");
                }}
              />
            </FormControl>
            <FormMessage />
          </FormItem>
        )}
      />
    </div>
  );
};