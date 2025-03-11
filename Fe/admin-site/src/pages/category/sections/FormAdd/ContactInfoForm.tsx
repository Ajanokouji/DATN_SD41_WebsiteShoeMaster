import { useForm } from "react-hook-form";
// import CustomSelect from "@/components/CustomSelect";
import { Input } from "@/components/ui/input";
import { Button } from "@/components/ui/button";
// import { socialMediaOptions } from "@/types/category/seed";

interface FormValues {
  phone_number: string;
  email: string;
  social_media: string;
  detailed_info: string;
}

const ContactInfoForm: React.FC = () => {
  const {
    register,
    handleSubmit,
    // setValue,
    formState: { errors },
  } = useForm<FormValues>({
    defaultValues: {
      phone_number: "",
      email: "",
      social_media: "",
      detailed_info: "",
    },
  });

  const onSubmit = (data: FormValues) => {
    console.log("Form Data:", data);
  };

  return (
    <form onSubmit={handleSubmit(onSubmit)} className="grid grid-cols-1 md:grid-cols-3 gap-5">
      <div className="flex flex-col gap-1">
        <label className="text-sm text-gray-400">
          Số điện thoại <span className="text-red-600">*</span>
        </label>
        <Input placeholder="0366858512" type="text" {...register("phone_number", { required: true })} />
        {errors.phone_number && <span className="text-red-500 text-sm">Số điện thoại không được bỏ trống</span>}
      </div>
      <div className="flex flex-col gap-1">
        <label className="text-sm text-gray-400">Email</label>
        <Input placeholder="example@gmail.com" type="email" {...register("email")} />
      </div>
      <div className="grid grid-cols-5 gap-3 justify-center items-center">
        <div className="col-span-2">
          {/* <CustomSelect options={socialMediaOptions} onChange={(value) => setValue("social_media", value)} /> */}
        </div>
        <div className="col-span-3">
          <label className="invisible">Url</label>
          <Input type="text" {...register("detailed_info")} />
        </div>
      </div>
      <div className="col-span-3 flex justify-end">
        <Button type="submit">Lưu thông tin</Button>
      </div>
    </form>
  );
};

export default ContactInfoForm;
