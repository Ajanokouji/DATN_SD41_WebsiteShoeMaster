import React, { useState, useEffect } from "react";
import { useForm, FormProvider } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Sheet, SheetContent, SheetHeader, SheetTitle } from "@/components/ui/sheet";
import { useAppDispatch } from "@/hooks/use-app-dispatch";
import { updateUser, fetchUserById, checkTrungCodeUser } from "@/redux/apps/user/userSlice";
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from "@/components/ui/select";
import FileManagerModal from "@/pages/file-manager/FileManagerModal";
import { IoMdClose } from "react-icons/io";
import { Eye, EyeOff, Plus } from "lucide-react";
import { FormLabel } from "@/components/ui/form";
import { z } from "zod";

interface EditUserSheetProps {
  isOpen: boolean;
  onClose: () => void;
  userId: string;
}

export const EditUserFormSchema = z.object({
  id: z.string(), // Thêm dòng này để form luôn có id
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
  avartarUrl: z.string().optional(), // ảnh không bắt buộc
});
export type EditUserFormSchema = z.infer<typeof EditUserFormSchema>;

const EditUserSheet: React.FC<EditUserSheetProps> = ({ isOpen, onClose, userId }) => {
  const dispatch = useAppDispatch();
  const [showPassword, setShowPassword] = useState(false);
  const [isModalFileOpen, setIsModalFileOpen] = useState(false);
  const [loading, setLoading] = useState(true);
  const [isEdit, setIsEdit] = useState(false);
  const [originalData, setOriginalData] = useState<EditUserFormSchema | null>(null);

  const methods = useForm<EditUserFormSchema>({
    resolver: zodResolver(EditUserFormSchema),
    defaultValues: {
      username: "",
      name: "",
      password: "",
      isActive: true,
      avartarUrl: "",
    },
    mode: "onChange",
  });

  const {
    handleSubmit,
    register,
    formState: { errors, isSubmitting },
    setValue,
    watch,
    reset,
  } = methods;

  // Lấy thông tin user khi mở sheet
  useEffect(() => {
    if (isOpen && userId) {
      setLoading(true);
      setIsEdit(false);
      dispatch(fetchUserById(userId)).then((action: any) => {
        if (fetchUserById.fulfilled.match(action)) {
          const user = action.payload;
          const formData: EditUserFormSchema = {
            id: user.id, // thêm id
            username: user.username ?? "",
            name: user.name ?? "",
            password: user.password ?? "",
            isActive: user.isActive ?? true,
            avartarUrl: user.avartarUrl ?? "",
          };
          reset(formData);
          setOriginalData(formData);
        }
        setLoading(false);
      });
    }
  }, [isOpen, userId, dispatch, reset]);

  // Xử lý chọn ảnh
  const avatarValue = watch("avartarUrl");
  const handleSelectImage = (file: { completeFilePath: string }) => {
    setValue("avartarUrl", file.completeFilePath);
    setIsModalFileOpen(false);
  };
  const handleRemoveImage = () => {
    setValue("avartarUrl", "");
  };

  const onSubmit = async (values: EditUserFormSchema) => {
    // Trim dữ liệu
    values.username = values.username.trim();
    values.name = values.name.trim();
    values.password = values.password.trim();

    // Check trùng username
    const isExist = await dispatch(
      checkTrungCodeUser({ username: values.username, id: userId })
    ).unwrap();

    if (isExist) {
      methods.setError("username", {
        type: "manual",
        message: "Tên đăng nhập đã tồn tại!",
      });
      methods.setFocus("username");
      return;
    }

    // Thêm id vào userData khi gửi lên
    await dispatch(updateUser({ id: userId, userData: values }));
    setIsEdit(false);
    setOriginalData(values);
    onClose();
  };

  const handleEdit = () => setIsEdit(true);

  const handleCancelEdit = () => {
    if (originalData) {
      reset(originalData);
    }
    setIsEdit(false);
  };

  return (
    <Sheet open={isOpen} onOpenChange={onClose}>
      <SheetContent className="w-[90%] sm:max-w-[80vw] max-w-none h-screen overflow-y-auto">
        <SheetHeader>
          <SheetTitle>Cập nhật tài khoản</SheetTitle>
        </SheetHeader>
        {loading ? (
          <div className="p-4">Đang tải dữ liệu...</div>
        ) : (
          <FormProvider {...methods}>
            <form onSubmit={handleSubmit(onSubmit)} className="space-y-6 p-4">
              <div>
                <label className="block font-medium mb-1">Tên đăng nhập</label>
                <Input
                  {...register("username")}
                  placeholder="Nhập tên đăng nhập"
                  autoComplete="username"
                  disabled={!isEdit}
                />
                {errors.username && (
                  <span className="text-red-500 text-sm">{errors.username.message}</span>
                )}
              </div>
              <div>
                <label className="block font-medium mb-1">Tên người dùng</label>
                <Input
                  {...register("name")}
                  placeholder="Nhập tên người dùng"
                  autoComplete="name"
                  disabled={!isEdit}
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
                    disabled={!isEdit}
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
                  disabled={!isEdit}
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
                    onClick={() => isEdit && setIsModalFileOpen(true)}
                    className="flex items-center"
                    disabled={!isEdit}
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
                      disabled={!isEdit}
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
                {!isEdit ? (
                  <Button type="button" onClick={handleEdit}>
                    Chỉnh sửa
                  </Button>
                ) : (
                  <>
                    <Button type="button" variant="outline" onClick={handleCancelEdit}>
                      Hủy
                    </Button>
                    <Button type="submit" disabled={isSubmitting}>
                      {isSubmitting ? "Đang cập nhật..." : "Lưu"}
                    </Button>
                  </>
                )}
              </div>
            </form>
          </FormProvider>
        )}
      </SheetContent>
    </Sheet>
  );
};

export default EditUserSheet;