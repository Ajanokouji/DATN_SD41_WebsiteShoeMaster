import React from "react";
import { Dialog, DialogContent, DialogHeader, DialogTitle } from "@/components/ui/dialog";
import FileManager from "@/pages/file-manager/index";

interface FileManagerWrapperProps {
  isOpen: boolean;
  onClose: () => void;
  onSelectFile: (fileUrl: string) => void; // Callback khi chọn file
}

const FileManagerWrapper: React.FC<FileManagerWrapperProps> = ({ isOpen, onClose, onSelectFile }) => {
  const handleFileSelect = (fileUrl: string) => {
    onSelectFile(fileUrl); // Gọi callback với URL file được chọn
    onClose(); // Đóng modal
  };

  return (
    <Dialog open={isOpen} onOpenChange={onClose}>
      <DialogContent className="max-w-4xl">
        <DialogHeader>
          <DialogTitle>Quản lý file</DialogTitle>
        </DialogHeader>
        <FileManager
          onFileSelect={handleFileSelect} // Truyền hàm xử lý khi chọn file
        />
      </DialogContent>
    </Dialog>
  );
};

export default FileManagerWrapper;