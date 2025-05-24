import React, { useState, useEffect } from "react";
import { IoMdClose } from "react-icons/io";
import { ImagePlus } from "lucide-react";
import FileManagerModal from "@/pages/file-manager/FileManagerModal";
import type { FileItem } from "@/types/file";

interface ImageIconSelectorProps {
  value?: string;
  onChange?: (value: string) => void;
  className?: string;
}

export const ImageIconSelector: React.FC<ImageIconSelectorProps> = ({
  value = "",
  onChange,
  className,
}) => {
  const [isModalOpen, setIsModalOpen] = useState(false);
  const [imageUrl, setImageUrl] = useState(value);

  useEffect(() => {
    console.log("Value changed:", value);
    setImageUrl(value);
  }, [value]);

  const handleSelectImage = (file: FileItem) => {
    console.log("Selected file:", file);
    const newUrl = file.completeFilePath;
    console.log("New URL:", newUrl);
    setImageUrl(newUrl);
    onChange?.(newUrl);
    setIsModalOpen(false);
  };

  const handleRemoveImage = () => {
    setImageUrl("");
    onChange?.("");
  };

  return (
    <div className={className}>
      {/* Icon + để mở modal chọn ảnh - chỉ hiển thị khi không có ảnh */}
      {!imageUrl && (
        <div title="Chọn ảnh">
          <ImagePlus
            size={20}
            className="cursor-pointer text-gray-600 hover:text-gray-900"
            onClick={() => setIsModalOpen(true)}
          />
        </div>
      )}

      {/* Hiển thị ảnh nếu có */}
      {imageUrl && (
        <div className="relative w-24 h-24 mt-2 inline-block">
          <img
            src={imageUrl}
            alt="Selected"
            className="w-full h-full object-contain border rounded-md"
            onError={(e) => {
              console.error("Image load error:", imageUrl);
              e.currentTarget.style.display = "none";
            }}
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
