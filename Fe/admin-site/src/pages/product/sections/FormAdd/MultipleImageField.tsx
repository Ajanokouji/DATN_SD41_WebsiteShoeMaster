import React, { useState } from "react";
import { Control, useController } from "react-hook-form";
import {
  FormLabel,
} from "@/components/ui/form";
import { Input } from "@/components/ui/input";
import { Button } from "@/components/ui/button";
import { ProductFormSchema } from "./FormSchema";
import { X, Plus } from "lucide-react";

interface MultipleImageFieldProps {
  control: Control<ProductFormSchema>;
}

export const MultipleImageField: React.FC<MultipleImageFieldProps> = ({ control }) => {
  // Use controller to properly manage the form field
  const {
    field: { value = [], onChange }
  } = useController({
    name: "mediaObjs",
    control,
    defaultValue: []
  });
  
  // Local state to manage temporarily entered URL
  const [newImageUrl, setNewImageUrl] = useState("");

  // Add a new image URL to the mediaObjs array
  const addImageUrl = () => {
    if (!newImageUrl.trim()) return;
    
    // Update the mediaObjs field using the controller
    onChange([...value, newImageUrl]);
    
    // Clear the input field
    setNewImageUrl("");
  };

  // Remove an image URL from the mediaObjs array
  const removeImageUrl = (index: number) => {
    const updatedMediaObjs = [...value];
    updatedMediaObjs.splice(index, 1);
    
    // Update the form values using the controller
    onChange(updatedMediaObjs);
  };

  // Handle Enter key press to add new image
  const handleKeyPress = (e: React.KeyboardEvent) => {
    if (e.key === 'Enter') {
      e.preventDefault();
      addImageUrl();
    }
  };

  return (
    <div className="space-y-4">
      <div className="flex items-center justify-between">
        <FormLabel className="text-base">Hình ảnh sản phẩm</FormLabel>
      </div>

      {/* Input for adding new image URL */}
      <div className="flex gap-2">
        <Input
          placeholder="https://example.com/image.jpg"
          value={newImageUrl}
          onChange={(e) => setNewImageUrl(e.target.value)}
          onKeyPress={handleKeyPress}
          className="flex-1"
        />
        <Button 
          type="button" 
          variant="outline"
          onClick={addImageUrl}
          className="flex items-center gap-1"
        >
          <Plus size={16} />
          Thêm ảnh
        </Button>
      </div>

      {/* Display and manage existing images */}
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

              {/* Image preview */}
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
    </div>
  );
};