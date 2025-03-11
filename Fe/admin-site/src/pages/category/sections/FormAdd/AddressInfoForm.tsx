import { useForm } from "react-hook-form";
import CustomSelect from "@/components/CustomSelect";
import { TimePickerWithRange } from "@/components/DatePickerRange";
import MultiSelectCard from "@/components/MultiSelectCard";
import { Input } from "@/components/ui/input";
import { Textarea } from "@/components/ui/textarea";
import { Button } from "@/components/ui/button";
import { cityOptions, districtOptions, wardOptions } from "@/types/category/seed";

interface FormValues {
  service: number[];
  notes: string;
  city: string;
  district: string;
  ward: string;
  address: string;
  follow_up_date: string;
  follow_down_date: string;
}

const AddressInfoForm: React.FC = () => {
  const {
    register,
    handleSubmit,
    setValue,
    watch,
    formState: { errors },
  } = useForm<FormValues>({
    defaultValues: {
      service: [],
      notes: "",
      city: "",
      district: "",
      ward: "",
      address: "",
      follow_up_date: "",
      follow_down_date: "",
    },
  });

  const onSubmit = (data: FormValues) => {
    console.log("Form Data:", data);
  };

  return (
    <form onSubmit={handleSubmit(onSubmit)} className="grid grid-cols-3 gap-5">
      <div>
        <h5 className="text-lg font-bold">Thông tin chi tiết</h5>
        <div className="flex flex-col gap-1">
          <MultiSelectCard
            selectedItems={watch("service")}
            onSelectionChange={(selected: number[]) => setValue("service", selected)}
          />
          <label className="text-sm text-gray-400">Ghi chú</label>
          <Textarea placeholder="Cần tư vấn chi tiết về xoa bóp cổ vai gáy" {...register("notes")} />
        </div>
      </div>

      <div className="flex flex-col gap-1">
        <label className="text-sm text-gray-400">Địa chỉ liên hệ</label>
        <div className="flex flex-col gap-2">
          <CustomSelect options={cityOptions} onChange={(value: string) => setValue("city", value)} />
          <CustomSelect options={districtOptions} onChange={(value: string) => setValue("district", value)} />
          <CustomSelect options={wardOptions} onChange={(value: string) => setValue("ward", value)} />
          <Input placeholder="Địa chỉ chi tiết" {...register("address", { required: true })} />
          {errors.address && <span className="text-red-500">Địa chỉ không được bỏ trống</span>}
        </div>
      </div>

      <div className="flex flex-col gap-1">
        <label className="text-sm text-gray-400">
          Chọn khung giờ <span className="text-red-600">*</span>
        </label>
        <TimePickerWithRange
          onTimeChange={(timeRange: { from: string; to: string }) => {
            setValue("follow_up_date", timeRange.from);
            setValue("follow_down_date", timeRange.to);
          }}
        />
      </div>

      <div className="col-span-3 flex justify-end">
        <Button type="submit">Lưu thông tin</Button>
      </div>
    </form>
  );
};

export default AddressInfoForm;
