import { X } from "lucide-react";

interface CloseIconProps {
  onClick: () => void;
  className?: string;
}

export const CloseIcon: React.FC<CloseIconProps> = ({ onClick, className = "" }) => {
  return (
    <div
      onClick={onClick}
      className={`absolute top-1 right-1 cursor-pointer text-gray-500 hover:text-red-500 ${className}`}
    >
      <X size={14} />
    </div>
  );
};

 
