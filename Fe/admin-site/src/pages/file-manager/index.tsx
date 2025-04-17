import React, { useRef, useState } from "react";
import { useAppDispatch } from "@/hooks/use-app-dispatch";
import { useAppSelector } from "@/hooks/use-app-selector";
import { uploadFile } from "@/redux/apps/file/fileSlice";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Button } from "@/components/ui/button";
import { Upload, X, Copy, Check } from "lucide-react";
import type { RootState } from "@/redux/store";

const FileManager = () => {
  const dispatch = useAppDispatch();
  const fileInputRef = useRef<HTMLInputElement>(null);
  const [copiedUrl, setCopiedUrl] = useState<string | null>(null);
  
  // Cập nhật cách lấy state
  const { loading = false, uploadedFiles = [] } = useAppSelector((state: RootState) => state.file || {});

  const handleFileChange = (event: React.ChangeEvent<HTMLInputElement>) => {
    const files = event.target.files;
    if (files) {
      Array.from(files).forEach((file) => {
        dispatch(uploadFile(file));
      });
    }
  };

  const handleDragOver = (event: React.DragEvent<HTMLDivElement>) => {
    event.preventDefault();
  };

  const handleDrop = (event: React.DragEvent<HTMLDivElement>) => {
    event.preventDefault();
    const files = event.dataTransfer.files;
    if (files) {
      Array.from(files).forEach((file) => {
        if (file.type.startsWith('image/')) {
          dispatch(uploadFile(file));
        }
      });
    }
  };

  const copyToClipboard = async (url: string) => {
    try {
      await navigator.clipboard.writeText(url);
      setCopiedUrl(url);
      setTimeout(() => setCopiedUrl(null), 2000);
    } catch (err) {
      console.error('Failed to copy: ', err);
    }
  };

  return (
    <div className="container mx-auto p-6">
      <Card>
        <CardHeader>
          <CardTitle>Quản lý Upload Ảnh</CardTitle>
        </CardHeader>
        <CardContent>
          <div
            onClick={() => fileInputRef.current?.click()}
            onDragOver={handleDragOver}
            onDrop={handleDrop}
            className="border-2 border-dashed rounded-lg p-8 text-center cursor-pointer transition-colors
              hover:border-primary"
          >
            <input
              type="file"
              ref={fileInputRef}
              onChange={handleFileChange}
              accept="image/*"
              multiple
              className="hidden"
            />
            <Upload className="mx-auto h-12 w-12 text-gray-400" />
            <p className="mt-2 text-sm text-gray-600">
              Kéo thả file hoặc click để chọn file
            </p>
            <p className="text-xs text-gray-500 mt-1">
              Hỗ trợ: PNG, JPG, JPEG, GIF, WEBP
            </p>
          </div>

          {/* Danh sách ảnh đã upload */}
          {uploadedFiles.length > 0 && (
            <div className="mt-8">
              <h3 className="text-lg font-semibold mb-4">Ảnh đã upload</h3>
              <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
                {uploadedFiles.map((url, index) => (
                  <div
                    key={index}
                    className="relative group border rounded-lg overflow-hidden"
                  >
                    <img
                      src={url}
                      alt={`Uploaded ${index + 1}`}
                      className="w-full h-48 object-cover"
                    />
                    <div className="absolute inset-0 bg-black/50 opacity-0 group-hover:opacity-100 transition-opacity flex items-center justify-center gap-2">
                      <Button
                        size="sm"
                        variant="secondary"
                        onClick={() => copyToClipboard(url)}
                      >
                        {copiedUrl === url ? (
                          <Check className="h-4 w-4" />
                        ) : (
                          <Copy className="h-4 w-4" />
                        )}
                      </Button>
                      <Button
                        size="sm"
                        variant="destructive"
                        onClick={() => {
                          // Thêm logic xóa ảnh nếu cần
                        }}
                      >
                        <X className="h-4 w-4" />
                      </Button>
                    </div>
                    {copiedUrl === url && (
                      <div className="absolute bottom-2 left-2 right-2 bg-green-500 text-white text-xs py-1 px-2 rounded">
                        Đã copy URL!
                      </div>
                    )}
                  </div>
                ))}
              </div>
            </div>
          )}

          {loading && (
            <div className="mt-4 text-center text-gray-600">
              Đang upload...
            </div>
          )}
        </CardContent>
      </Card>
    </div>
  );
};

export default FileManager; 