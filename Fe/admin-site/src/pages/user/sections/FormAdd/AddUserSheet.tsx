import React, { useState } from "react";
import { useForm, FormProvider, useFormContext } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { z } from "zod";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Sheet, SheetContent, SheetHeader, SheetTitle } from "@/components/ui/sheet";
import { useAppDispatch } from "@/hooks/use-app-dispatch";
import { checkTrungCodeUser, createUser } from "@/redux/apps/user/userSlice";
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from "@/components/ui/select";
import { Eye, EyeOff } from "lucide-react";
import FileManagerModal from "@/pages/file-manager/FileManagerModal";
import { IoMdClose } from "react-icons/io";
import { Plus } from "lucide-react";
import { FormLabel } from "@/components/ui/form";

// FormSchema cho user
export const UserFormSchema = z.object({
  username: z
    .string()
    .min(3, "Tên đăng nhập tối thiểu 3 ký tự")
    .max(20, "Tên đăng nhập tối đa 20 ký tự")
    .regex(/^[a-zA-Z0-9]+$/, "Tên đăng nhập chỉ được chứa chữ cái không dấu và số"),
  name: z
    .string()
    .min(1, "Tên người dùng không được để trống")
    .max(50, "Tên người dùng tối đa 50 ký tự"),
  password: z
    .string()
    .min(8, "Mật khẩu tối thiểu 8 ký tự")
    .max(32, "Mật khẩu tối đa 32 ký tự")
    .regex(
      /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[!@#$%^&*()_\-+=\[\]{};':"\\|,.<>/?]).{8,32}$/,
      "Mật khẩu phải có chữ hoa, chữ thường, số và ký tự đặc biệt"
    ),
  isActive: z.boolean(),
  type: z.number().refine(val => val === 0 || val === 1, {
    message: "Loại tài khoản không hợp lệ",
  }),
  avartarUrl: z.string().optional(), // ảnh không bắt buộc
});
export type UserFormSchema = z.infer<typeof UserFormSchema>;

interface AddUserSheetProps {
  isOpen: boolean;
  onClose: () => void;
  setUserType: (type: number) => void; // thêm prop này
}

const AddUserSheet: React.FC<AddUserSheetProps> = ({ isOpen, onClose, setUserType }) => {
  const dispatch = useAppDispatch();
  const [showPassword, setShowPassword] = useState(false);
  const [isModalFileOpen, setIsModalFileOpen] = useState(false);

  const defaultValues: UserFormSchema = {
    username: "",
    name: "",
    password: "",
    isActive: true,
    type: 0,
    avartarUrl: "",
  };

  const methods = useForm<UserFormSchema>({
    resolver: zodResolver(UserFormSchema),
    defaultValues,
    mode: "onChange", // validate realtime khi nhập
  });

  const {
    handleSubmit,
    register,
    formState: { errors, isSubmitting },
    setValue,
    watch,
  } = methods;

  // Reset form mỗi khi mở sheet
  React.useEffect(() => {
    if (isOpen) {
      methods.reset(defaultValues);
    }
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [isOpen]);

  // Xử lý chọn ảnh
  const avatarValue = watch("avartarUrl");
  const handleSelectImage = (file: { completeFilePath: string }) => {
    setValue("avartarUrl", file.completeFilePath);
    setIsModalFileOpen(false);
  };
  const handleRemoveImage = () => {
    setValue("avartarUrl", "");
  };

  const onSubmit = async (values: UserFormSchema) => {
    //Trim dữ liệu
    values.username = values.username.trim();
    values.name = values.name.trim();
    values.password = values.password.trim();

    // Kiểm tra trùng username trước khi thêm
    const resultAction = await dispatch(checkTrungCodeUser({ username: values.username.trim(), id: undefined }));
    if (checkTrungCodeUser.fulfilled.match(resultAction)) {
      const isExist = resultAction.payload;

      if (isExist) {
        methods.setError("username", {
          type: "manual",
          message: "Tên tài khoản đã tồn tại",
        });
        methods.setFocus("username");
        return;
      }
    }
    else {
      console.error("Lỗi trong quá trình kiểm tra username:", resultAction.error.message);
      return;
    }
    
    // Nếu không trùng, tiếp tục thêm user
    await dispatch(createUser(values));
    setUserType(values.type); // chuyển combobox sang đúng loại vừa thêm
    onClose();
  };

  return (
    <Sheet open={isOpen} onOpenChange={onClose}>
      <SheetContent className="w-[90%] sm:max-w-[80vw] max-w-none h-screen overflow-y-auto">
        <SheetHeader>
          <SheetTitle>Thêm tài khoản</SheetTitle>
        </SheetHeader>
        <FormProvider {...methods}>
          <form onSubmit={handleSubmit(onSubmit)} className="space-y-6 p-4">
            <div>
              <label className="block font-medium mb-1">Loại tài khoản</label>
              <Select
                value={watch("type").toString()}
                onValueChange={val => setValue("type", Number(val) as 0 | 1)}
              >
                <SelectTrigger className="w-full">
                  <SelectValue placeholder="Chọn loại tài khoản" />
                </SelectTrigger>
                <SelectContent>
                  <SelectItem value="0">Admin</SelectItem>
                  <SelectItem value="1">Nhân viên</SelectItem>
                </SelectContent>
              </Select>
              {errors.type && (
                <span className="text-red-500 text-sm">{errors.type.message}</span>
              )}
            </div>

            <UsernameField />

            <div>
              <label className="block font-medium mb-1">Tên người dùng</label>
              <Input
                {...register("name")}
                placeholder="Nhập tên người dùng"
                autoComplete="name"
              />
              {errors.name && (
                <span className="text-red-500 text-sm">{errors.name.message}</span>
              )}
            </div>
            <div>
              <label className="block font-medium mb-1">Mật khẩu</label>
              <div className="relative">
                <Input
                  type={showPassword ? "text" : "password"}
                  {...register("password")}
                  placeholder="Nhập mật khẩu"
                  autoComplete="new-password"
                  className="pr-10"
                />
                <button
                  type="button"
                  className="absolute right-3 top-1/2 -translate-y-1/2 text-gray-500 hover:text-blue-600"
                  tabIndex={-1}
                  onClick={() => setShowPassword((v) => !v)}
                >
                  {showPassword ? <EyeOff size={18} /> : <Eye size={18} />}
                </button>
              </div>
              {errors.password && (
                <span className="text-red-500 text-sm">{errors.password.message}</span>
              )}
            </div>
            <div>
              <label className="block font-medium mb-1">Trạng thái</label>
              <Select
                value={watch("isActive") ? "1" : "0"}
                onValueChange={val => setValue("isActive", val === "1")}
              >
                <SelectTrigger className="w-full">
                  <SelectValue placeholder="Chọn trạng thái" />
                </SelectTrigger>
                <SelectContent>
                  <SelectItem value="1">Hoạt động</SelectItem>
                  <SelectItem value="0">Dừng hoạt động</SelectItem>
                </SelectContent>
              </Select>
              {errors.isActive && (
                <span className="text-red-500 text-sm">{errors.isActive.message}</span>
              )}
            </div>

            {/* Phần chọn ảnh đại diện */}
            <div className="grid gap-4">
              <div className="flex items-center justify-between">
                <FormLabel className="text-base">Ảnh đại diện</FormLabel>
              </div>
              <div className="flex gap-2 ml-2">
                <Button
                  type="button"
                  variant="outline"
                  onClick={() => setIsModalFileOpen(true)}
                  className="flex items-center"
                >
                  <Plus size={16} />
                  Thêm ảnh
                </Button>
              </div>
              {/* Hiển thị ảnh đã chọn (nếu có) */}
              {avatarValue && (
                <div className="relative w-24 h-24 ml-2">
                  <img
                    src={avatarValue}
                    alt="Avatar preview"
                    className="w-full h-full object-contain border rounded-md"
                  />
                  <Button
                    onClick={handleRemoveImage}
                    className="absolute top-1 right-1 p-1 text-sm bg shadow-md bg-gray hover:bg-gray-200"
                    type="button"
                  >
                    <IoMdClose />
                  </Button>
                </div>
              )}
              {isModalFileOpen && (
                <FileManagerModal
                  isOpen={isModalFileOpen}
                  onClose={() => setIsModalFileOpen(false)}
                  onSelectImage={handleSelectImage}
                />
              )}
            </div>

            <div className="flex justify-end gap-2">
              <Button type="button" variant="outline" onClick={onClose}>
                Hủy
              </Button>
              <Button type="submit" disabled={isSubmitting}>
                {isSubmitting ? "Đang thêm..." : "Thêm"}
              </Button>
            </div>
          </form>
        </FormProvider>
      </SheetContent>
    </Sheet>
  );
};

const UsernameField = () => {
  const { register, formState: { errors } } = useFormContext<UserFormSchema>();
  console.log("Username error:", errors.username);
  return (
    <div>
      <label className="block font-medium mb-1">Tên đăng nhập</label>
      <Input
        {...register("username")}
        placeholder="Nhập tên đăng nhập"
        autoComplete="username"
      />
      {errors.username && (
        <span className="text-red-500 text-sm">{errors.username.message}</span>
      )}
    </div>
  );
};

export default AddUserSheet;