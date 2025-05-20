import React, { useState } from "react";
import { IoMdClose } from "react-icons/io";
import { ImagePlus } from "lucide-react";
import FileManagerModal from "@/pages/file-manager/FileManagerModal";
import type { FileItem } from "@/types/file";

interface ImageIconSelectorProps {
  value?: string;
  onChange?: (value: string) => void;
  className?: string;
}

export const ImageIconSelector: React.FC<ImageIconSelectorProps> = (
  {
  value = "",
  onChange,
  className,
}) => {
  const [isModalOpen, setIsModalOpen] = useState(false);

  const handleSelectImage = (file: FileItem) => {
    alert(file.completeFilePath)
    onChange?.(file.completeFilePath);
    setIsModalOpen(false);
  };

  const handleRemoveImage = () => {
    onChange?.("");
  };

  return (
    <div className={className}>
      {/* Icon + để mở modal chọn ảnh */}
      <div title="Chọn ảnh">
        <ImagePlus
          size={20}
          className="cursor-pointer text-gray-600 hover:text-gray-900"
          onClick={() => setIsModalOpen(true)}
        />
      </div>

      {/* Hiển thị ảnh nếu có */}
      {value && (
        <div className="relative w-24 h-24 mt-2 inline-block">
          <img
            src={value}
            alt="Selected"
            className="w-full h-full object-contain border rounded-md"
          />
          <button
            type="button"
            onClick={handleRemoveImage}
            className="absolute top-1 right-1 p-1 text-sm bg-gray-200 rounded hover:bg-gray-300"
            title="Xóa ảnh"
          >
            <IoMdClose />
          </button>
        </div>
      )}

      {/* Modal chọn ảnh */}
      {isModalOpen && (
        <FileManagerModal
          isOpen={isModalOpen}
          onClose={() => setIsModalOpen(false)}
          onSelectImage={handleSelectImage}
        />
      )}
    </div>
  );
};
