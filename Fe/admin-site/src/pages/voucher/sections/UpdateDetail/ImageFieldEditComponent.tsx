import React, { useState } from "react";
import { Control, useController } from "react-hook-form";
import { VoucherFormSchema } from "../UpdateDetail/FormSchema";
import FileManagerModal from "@/pages/file-manager/FileManagerModal";
import { Button } from "@/components/ui/button";
import { Pencil, Plus } from "lucide-react";
import { IoMdClose } from "react-icons/io";
import { FormLabel } from "@/components/ui/form";
import type { FileItem } from "@/types/file";

interface ImageFieldEditProps {
  control: Control<VoucherFormSchema>;
  isEditing: boolean;
}

export const ImageFieldEditComponent: React.FC<ImageFieldEditProps> = ({ control, isEditing }) => {
  const {
    field: { value = "", onChange }
  } = useController({
    name: "imageUrl",
    control
  });

  const [isModalFileOpen, setIsModalFileOpen] = useState(false);

  const handleSelectImage = (file: FileItem) => {
    onChange(file.completeFilePath);
    setIsModalFileOpen(false);
  };

  const handleRemoveImage = () => {
    onChange("");
  };

  return (
    <div className="grid gap-4">
      <div>
        <FormLabel className="text-lg">Hình ảnh voucher</FormLabel>
        {isEditing && (
          <Button
            type="button"
            variant="outline"
            onClick={() => setIsModalFileOpen(true)}
            className="mt-2 mb-2 flex items-center gap-1 ml-5"
          >
            {value ? <Pencil size={16} /> : <Plus size={16} />}
            {value ? "Sửa ảnh" : "Thêm ảnh"}
          </Button>
        )}
      </div>

      {/* Hiển thị ảnh đã chọn (nếu có) */}
      {value && (
        <div className="relative w-24 h-24 ml-5">
          <img
            src={value}
            alt="Voucher preview"
            className="w-full h-full object-contain border rounded-md"
          />
          {isEditing && (
            <Button
              onClick={handleRemoveImage}
              className="absolute top-1 right-1 p-1 text-sm bg shadow-md bg-gray hover:bg-gray-200"
            >
              <IoMdClose />
            </Button>
          )}
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
  );
};