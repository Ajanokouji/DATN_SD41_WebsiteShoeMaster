import React, { useState } from "react";
import { Control, useController } from "react-hook-form";
import { ProductFormSchema } from "./FormSchema";
import FileManagerModal from "@/pages/file-manager/FileManagerModal";
import { Button } from "@/components/ui/button";
import { Plus } from "lucide-react";
import { IoMdClose } from "react-icons/io";
import { FormLabel } from "@/components/ui/form";
import type { FileItem } from "@/types/file";

interface ImageFieldProps {
  control: Control<ProductFormSchema>;
}

export const ImageField: React.FC<ImageFieldProps> = ({ control }) => {
  const {
    field: { value = "", onChange }
  } = useController({
    name: "imageUrl", // Trường imageUrl trong form
    control
  });

  const handleSelectImage = (file: FileItem) => {
    onChange(file.completeFilePath); // Gán giá trị vào trường imageUrl trong form
    setIsModalFileOpen(false);
  };

  const handleRemoveImage = () => {
    onChange(""); // Xóa giá trị trong trường imageUrl
  };

  const [isModalFileOpen, setIsModalFileOpen] = useState(false);

  return (
    <>
      <div className="grid gap-4">
        <div className="flex items-center justify-between">
          <FormLabel className="text-base">Hình ảnh chính</FormLabel>
        </div>

        <div className="flex gap-2">
          <Button
            type="button"
            variant="outline"
            onClick={() => setIsModalFileOpen(true)}
            className="flex items-center gap-1"
          >
            <Plus size={16} />
            Thêm ảnh
          </Button>
        </div>

        {/* Hiển thị ảnh đã chọn (nếu có) */}
        {value && (
          <div className="relative w-24 h-24">
            <img
              src={value}
              alt="Product preview"
              className="w-full h-full object-contain border rounded-md" // Giới hạn kích thước 100x100 px
            />
            <Button
              onClick={handleRemoveImage}
              className="absolute top-1 right-1 p-1 text-sm bg shadow-md  bg-gray hover:bg-gray-200"
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
    </>
  );
};
