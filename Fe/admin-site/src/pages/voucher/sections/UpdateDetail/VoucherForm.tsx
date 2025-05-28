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
import { FaCartShopping, FaStore } from "react-icons/fa6";

interface VoucherFormProps {
  methods: UseFormReturn<VoucherFormSchema>;
  isEditing: boolean;
  isLoading?: boolean;
  onSubmit: (values: VoucherFormSchema) => void;
  children?: React.ReactNode;
  errorMessage?: string | null;
}

export const VoucherForm: React.FC<VoucherFormProps> = ({
  methods,
  isEditing,
  onSubmit,
  children,
  isLoading,
  errorMessage,
}) => {
  const { control, watch, setValue, trigger } = methods;
  const discountType = watch("discountType");

  const handleDiscountTypeChange = (val: "VNĐ" | "%") => {
    setValue("discountType", val);

    if (val === "VNĐ") {
      trigger("discountAmount");
    } else {
      trigger("discountPercentage");
    }
  };

  if (isLoading) {
    return (
      <div className="flex items-center justify-center h-full">
        <p>Đang tải chi tiết voucher...</p>
      </div>
    );
  }

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

          <h2 className="pb-5 text-lg font-semibold">Thông tin cơ bản</h2>
          <div className="mb-5 mt-5">
            <FormField
              control={control}
              name="voucherName"
              render={({ field }) => (
                <FormItem>
                  <div className="flex items-center gap4">
                    <FormLabel className="w-32">Tên Voucher</FormLabel>
                    <FormControl className="flex-1">
                      <Input
                        placeholder="Nhập tên voucher tối đa 100 ký tự"
                        {...field}
                        disabled={!isEditing}
                      />
                    </FormControl>
                  </div>
                  <FormMessage className="text-center ml-32" />
                </FormItem>
              )}
            />
          </div>
          <div className="mb-5">
            <FormField
              control={control}
              name="code"
              render={({ field }) => (
                <FormItem>
                  <div className="flex items-center gap-4">
                    <FormLabel className="w-32">Mã Voucher</FormLabel>
                    <FormControl className="flex-1">
                      <Input
                        placeholder="Nhập mã voucher tối đa 10 ký tự, chỉ bao gồm chữ cái và số, vui lòng không nhập các ký tự đặc biệt và khoảng trắng"
                        {...field}
                        disabled
                      />
                    </FormControl>
                  </div>
                  <FormMessage className="text-center ml-32" />
                </FormItem>
              )}
            />
          </div>
          <div className="flex mb-10">
            <FormField
              control={control}
              name="startDate"
              render={({ field }) => (
                <FormItem>
                  <div className="flex items-center gap-4">
                    <FormLabel className="w-32">Thời gian lưu mã</FormLabel>
                    <FormControl className="flex-1">
                      <Input
                        type="datetime-local"
                        {...field}
                        disabled={!isEditing}
                      />
                    </FormControl>
                  </div>
                  <FormMessage className="text-center ml-32" />
                </FormItem>
              )}
            />
            <div className="pl-5 pr-5">__</div>
            <FormField
              control={control}
              name="endDate"
              render={({ field }) => (
                <FormItem>
                  <div className="flex items-center">
                    <FormControl>
                      <Input
                        type="datetime-local"
                        {...field}
                        disabled={!isEditing}
                      />
                    </FormControl>
                  </div>
                  <FormMessage className="text-center" />
                </FormItem>
              )}
            />
          </div>

          <hr className="border border-gray-300"/>

          <div className="mt-10">
            <div className="mt-10">
              <h2 className="text-lg font-semibold">Thiết lập mã giảm giá</h2>
            </div>
            <div className="mt-10 ml-5">
              <div className="mt-5">
                {discountType === "VNĐ" && (
                  <FormField
                    control={control}
                    name="discountAmount"
                    render={({ field }) => (
                      <FormItem>
                        <div className="flex items-center gap-4">
                          <FormLabel className="w-32">Giá trị giảm</FormLabel>
                          <div className="flex items-center gap-2 flex-1">
                            <FormControl>
                              <Input
                                type="number"
                                placeholder="Nhập số tiền giảm, phải lớn hơn 0"
                                {...field}
                                disabled={!isEditing}
                              />
                            </FormControl>
                            <Select
                              value={discountType}
                              onValueChange={(val) => handleDiscountTypeChange(val as "VNĐ" | "%")}
                            >
                              <SelectTrigger className="w-20"
                                disabled={!isEditing}
                              >
                                <SelectValue />
                              </SelectTrigger>
                              <SelectContent>
                                <SelectItem value="VNĐ">VNĐ</SelectItem>
                                <SelectItem value="%">%</SelectItem>
                              </SelectContent>
                            </Select>
                          </div>
                        </div>
                        <FormMessage className="text-center ml-32"/>
                      </FormItem>
                    )}
                  />
                )}

                {discountType === "%" && (
                  <div className="flex gap-4">
                    {/* Giá trị giảm */}
                    <FormField
                      control={control}
                      name="discountPercentage"
                      render={({ field }) => (
                        <FormItem className="flex-1">
                          <div className="flex items-center gap-2">
                            <FormLabel className="w-32">Giá trị giảm</FormLabel>
                            <FormControl className="flex-1">
                              <Input
                                type="number"
                                placeholder="Nhập phần trăm giảm, 0 < x <= 100"
                                {...field}
                                disabled={!isEditing}
                              />
                            </FormControl>
                            <Select
                              value={discountType}
                              onValueChange={(val) => handleDiscountTypeChange(val as "VNĐ" | "%")}
                            >
                              <SelectTrigger className="w-16"
                                disabled={!isEditing}
                              >
                                <SelectValue />
                              </SelectTrigger>
                              <SelectContent>
                                <SelectItem value="VNĐ">VNĐ</SelectItem>
                                <SelectItem value="%">%</SelectItem>
                              </SelectContent>
                            </Select>
                          </div>
                          <FormMessage className="text-center ml-20" />
                        </FormItem>
                      )}
                    />

                    {/* Tối đa */}
                    <FormField
                      control={control}
                      name="maxDiscountAmount"
                      render={({ field }) => (
                        <FormItem className="flex-1 ml-9">
                          <div className="flex items-center gap-2">
                            <FormLabel className="w-20">Tối đa</FormLabel>
                            <FormControl className="flex-1">
                              <Input
                                type="number"
                                placeholder="Nhập số tiền giảm tối đa, phải lớn hơn 0"
                                {...field}
                                disabled={!isEditing}
                              />
                            </FormControl>
                          </div>
                          <FormMessage className="text-center ml-20" />
                        </FormItem>
                      )}
                    />
                  </div>
                )}
              </div>
              <div className="mt-5">
                <FormField
                  control={control}
                  name="minimumOrderAmount"
                  render={({ field }) => (
                    <FormItem>
                      <div className="flex items-center gap-4">
                        <FormLabel className="w-32">Giá trị đơn hàng tối thiểu</FormLabel>
                        <FormControl className="flex-1">
                          <Input
                            type="number"
                            placeholder="Nhập giá trị đơn hàng tối thiểu, phải lớn hơn hoặc bằng 0"
                            {...field}
                            disabled={!isEditing}
                          />
                        </FormControl>
                      </div>
                      <FormMessage className="text-center ml-32" />
                    </FormItem>
                  )}
                />
              </div>
              <div className="mt-5">
                <FormField
                  control={control}
                  name="totalMaxUsage"
                  render={({ field }) => (
                    <FormItem>
                      <div className="flex items-center gap-4">
                        <FormLabel className="w-32">Tổng lượt sử dụng tối đa</FormLabel>
                        <FormControl className="flex-1">
                          <Input
                            type="number"
                            placeholder="Nhập tổng số mã giảm giá có thể sử dụng, phải lớn hơn 0"
                            {...field}
                            disabled={!isEditing}
                          />
                        </FormControl>
                      </div>
                      <FormMessage className="text-center ml-32" />
                    </FormItem>
                  )}
                />
              </div>
              <div className="mt-5">
                <FormField
                  control={control}
                  name="maxUsagePerCustomer"
                  render={({ field }) => (
                    <FormItem>
                      <div className="flex items-center gap-4">
                        <FormLabel className="w-32">Lượt sử dụng tối đa/người mua</FormLabel>
                        <FormControl className="flex-1">
                          <Input
                            type="number"
                            placeholder="Nhập số lượt sử dụng tối đa trên một người mua, phải lớn hơn 0 và không được lớn hơn Tổng lượt sử dụng tối đa"
                            {...field}
                            disabled={!isEditing}
                          />
                        </FormControl>
                      </div>
                      <FormMessage className="text-center ml-32" />
                    </FormItem>
                  )}
                />
              </div>
              <div className="mt-5">
                <FormField
                  control={control}
                  name="status"
                  render={({ field }) => (
                    <FormItem>
                      <div className="flex items-center gap-4">
                        <FormLabel className="w-32">Trạng thái</FormLabel>
                        <Select
                          value={String(field.value)}
                          onValueChange={(val) => {
                            field.onChange(Number(val));
                            trigger("status");
                          }}
                          disabled={!isEditing}
                        >
                          <FormControl className="flex-1">
                            <SelectTrigger>
                              <SelectValue placeholder="Chọn trạng thái" />
                            </SelectTrigger>
                          </FormControl>
                          <SelectContent>
                            <SelectItem value="1">Hoạt động</SelectItem>
                            <SelectItem value="0">Không hoạt động</SelectItem>
                          </SelectContent>
                        </Select>
                      </div>
                      <FormMessage className="text-center" />
                    </FormItem>
                  )}
                />
              </div>
              <div className="mt-5 mb-10">
                <FormField
                  control={control}
                  name="description"
                  render={({ field }) => (
                    <FormItem>
                      <div className="flex items-center gap-4">
                        <FormLabel className="w-32">Mô tả</FormLabel>
                        <FormControl className="flex-1">
                          <Textarea
                            placeholder="Nhập mô tả dưới 256 ký tự"
                            {...field}
                            disabled={!isEditing}
                          />
                        </FormControl>
                      </div>
                      <FormMessage className="text-center" />
                    </FormItem>
                  )}
                />
              </div>
            </div>
          </div>

          {errorMessage && (
            <div className="text-red-500 text-center font-semibold mb-2">
              {errorMessage}
            </div>
          )}
          {children}
        </form>
      </Form>
    </FormProvider>
  );
};