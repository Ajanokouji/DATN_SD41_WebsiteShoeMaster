import { Dialog, DialogContent } from "@/components/ui/dialog";
import FileManager from "@/pages/file-manager/index";

const FileManagerModal = ({
  isOpen,
  onClose,
}: {
  isOpen: boolean;
  onClose: () => void;
}) => {
  return (
    <Dialog open={isOpen} onOpenChange={onClose}>
      <DialogContent className="max-w-6xl w-full p-0">
        <FileManager />
      </DialogContent>
    </Dialog>
  );
};

export default FileManagerModal;