import React, { useState } from "react";
import { Control, useController } from "react-hook-form";
import { FormLabel } from "@/components/ui/form";
import { Button } from "@/components/ui/button";
import { ProductFormSchema } from "./FormSchema";
import { X, Plus } from "lucide-react";
import FileManagerModal from "@/pages/file-manager/FileManagerModal";
import { FileItem } from "@/types/file";

interface MultipleImageFieldProps {
  control: Control<ProductFormSchema>;
}

export const MultipleImageField: React.FC<MultipleImageFieldProps> = ({ control }) => {
  const {
    field: { value = [], onChange }
  } = useController({
    name: "mediaObjs",
    control,
    defaultValue: []
  });

  const [isModalFileOpen, setIsModalFileOpen] = useState(false);

  const handleSelectImage = (file: FileItem) => {
    onChange([...value, file.completeFilePath]);
    setIsModalFileOpen(false);
  };

  const removeImageUrl = (index: number) => {
    const updated = [...value];
    updated.splice(index, 1);
    onChange(updated);
  };

  return (
    <div className="space-y-4">
      <div className="flex items-center justify-between">
        <FormLabel className="text-base">Hình ảnh sản phẩm</FormLabel>
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

      {value.length === 0 ? (
        <div className="text-sm text-muted-foreground italic">
          Chưa có hình ảnh nào. Vui lòng thêm hình ảnh sản phẩm.
        </div>
      ) : (
        <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
          {value.map((url, index) => (
            <div key={index} className="space-y-4 border p-4 rounded-md">
              <div className="flex justify-between items-start">
                <div className="flex-1 truncate">
                  <p className="text-sm font-medium mb-1">URL Hình ảnh {index + 1}</p>
                  <p className="text-sm text-muted-foreground truncate">{url}</p>
                </div>
                <Button 
                  type="button" 
                  variant="ghost" 
                  size="icon"
                  onClick={() => removeImageUrl(index)}
                >
                  <X size={16} className="text-red-500" />
                </Button>
              </div>

              <div>
                <p className="text-sm font-medium mb-2">Xem trước:</p>
                <div className="border rounded-md overflow-hidden w-full h-40 bg-slate-100">
                  <img
                    src={url}
                    alt="Product preview"
                    className="w-full h-full object-contain"
                    onError={(e) => {
                      const target = e.target as HTMLImageElement;
                      target.src = "/api/placeholder/200/200";
                      target.alt = "Image error";
                    }}
                  />
                </div>
              </div>
            </div>
          ))}
        </div>
      )}

      {/* FileManager Modal */}
      {isModalFileOpen && (
        <FileManagerModal
          isOpen={isModalFileOpen}
          onClose={() => setIsModalFileOpen(false)}
          //todo will define
          onSelectImage={handleSelectImage}
        />
      )}
    </div>
  );
};
