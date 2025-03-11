import { useForm } from "react-hook-form";
// import CustomSelect from "@/components/CustomSelect";
import { DatePickerInput } from "@/components/DateInput";
import { Input } from "@/components/ui/input";
import { Button } from "@/components/ui/button";
// import { sourceOptions, statusOptions } from "@/types/category/seed";

interface FormValues {
  full_name: string;
  gender: string;
  date_of_birth: string;
  source: string;
  status: string;
}

const BasicInfoForm: React.FC = () => {
  const {
    register,
    handleSubmit,
    setValue,
    watch,
    formState: { errors },
  } = useForm<FormValues>({
    defaultValues: {
      full_name: "",
      gender: "",
      date_of_birth: "",
      source: "",
      status: "",
    },
  });

  const onSubmit = (data: FormValues) => {
    console.log("Form Data:", data);
  };

  return (
    <form onSubmit={handleSubmit(onSubmit)} className="grid grid-cols-3 gap-4">
      <div className="flex flex-col gap-1">
        <label className="text-sm text-gray-400">
          Họ tên khách hàng <span className="text-red-600">*</span>
        </label>
        <Input type="text" {...register("full_name", { required: true })} />
        {errors.full_name && <span className="text-red-500 text-sm">Họ tên không được bỏ trống</span>}
      </div>

      <div className="flex justify-center items-center pt-5 gap-3">
        <label className="text-sm text-gray-400">Giới tính</label>
        <div className="flex items-center gap-4">
          {["Nam", "Nữ", "Khác"].map((gender) => (
            <label key={gender} className="flex items-center">
              <input
                type="radio"
                {...register("gender", { required: true })}
                value={gender}
                className="mr-2"
              />
              {gender}
            </label>
          ))}
        </div>
      </div>

      <div className="flex flex-col gap-1">
        <label className="text-sm text-gray-400">Ngày sinh</label>
        <DatePickerInput
          selected={watch("date_of_birth") ? new Date(watch("date_of_birth")) : undefined}
          onChange={(date) => setValue("date_of_birth", date.toISOString())}
        />
      </div>

      {/* <div className="col-span-1 grid grid-cols-2 gap-3">
        <CustomSelect options={sourceOptions} onChange={(value) => setValue("source", value)} />
        <CustomSelect options={statusOptions} onChange={(value) => setValue("status", value)} />
      </div> */}

      <div className="col-span-3 flex justify-end">
        <Button type="submit">Lưu thông tin</Button>
      </div>
    </form>
  );
};

export default BasicInfoForm;
