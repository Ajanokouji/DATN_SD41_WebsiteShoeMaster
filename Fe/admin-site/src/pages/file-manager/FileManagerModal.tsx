import { Dialog, DialogContent, DialogTitle } from "@/components/ui/dialog";
import FileManager from "@/pages/file-manager/index";
import type { FileItem } from "@/types/file";

const FileManagerModal = ({
  isOpen,
  onClose,
  onSelectImage,
}: {
  isOpen: boolean;
  onClose: () => void;
  onSelectImage?: (file: FileItem) => void; // optional
}) => {
  return (
    <Dialog open={isOpen} onOpenChange={onClose}>
      <DialogContent className="max-w-6xl w-full p-0">
        <DialogTitle className="sr-only">File Manager</DialogTitle>
        <FileManager onSelectImage={onSelectImage} />
      </DialogContent>
    </Dialog>
  );
};

export default FileManagerModal;
