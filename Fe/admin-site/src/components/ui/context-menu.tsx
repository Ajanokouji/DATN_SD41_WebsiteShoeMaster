import * as React from "react";
import { cn } from "@/lib/utils";

interface ContextMenuProps {
  children?: React.ReactNode;
  items: {
    icon?: React.ReactNode;
    label: string;
    onClick?: () => void;
    divider?: boolean;
    className?: string;
  }[];
  x: number;
  y: number;
  onClose: () => void;
}

const ContextMenu: React.FC<ContextMenuProps> = ({ items, x, y, onClose }) => {
  const menuRef = React.useRef<HTMLDivElement>(null);

  React.useEffect(() => {
    const handleClickOutside = (event: MouseEvent) => {
      if (menuRef.current && !menuRef.current.contains(event.target as Node)) {
        onClose();
      }
    };

    document.addEventListener("mousedown", handleClickOutside);
    return () => document.removeEventListener("mousedown", handleClickOutside);
  }, [onClose]);

  return (
    <div
      ref={menuRef}
      className="fixed bg-popover text-popover-foreground shadow-md rounded-md py-1 z-50 min-w-[200px]"
      style={{ 
        top: y, 
        left: x,
        maxHeight: '80vh',
        overflowY: 'auto'
      }}
    >
      {items.map((item, index) => (
        <React.Fragment key={index}>
          {item.divider ? (
            <div className="h-px bg-border my-1" />
          ) : (
            <button
              className={cn(
                "w-full px-3 py-2 text-left text-sm flex items-center space-x-2",
                "hover:bg-accent hover:text-accent-foreground",
                "focus:outline-none focus:bg-accent focus:text-accent-foreground",
                item.className
              )}
              onClick={() => {
                item.onClick?.();
                onClose();
              }}
            >
              {item.icon && <span className="w-4 h-4">{item.icon}</span>}
              <span>{item.label}</span>
            </button>
          )}
        </React.Fragment>
      ))}
    </div>
  );
};

export { ContextMenu }; 