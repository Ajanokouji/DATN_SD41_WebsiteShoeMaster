import { useState, useEffect } from "react";
import { Check, ChevronsUpDown } from "lucide-react";
import { cn } from "@/lib/utils";
import {
  Command,
  CommandEmpty,
  CommandGroup,
  CommandInput,
  CommandItem,
} from "@/components/ui/command";
import {
  Popover,
  PopoverContent,
  PopoverTrigger,
} from "@/components/ui/popover";
import { Button } from "@/components/ui/button";

// Define generic type for items
export interface FilterSelectProps<T> {
  items: T[];
  onSearch?: (value: string) => void;
  onLoadMore?: () => void;
  onSelect?: (item: T) => void;
  selected?: T | null;
  hasMorePages?: boolean;
  isLoading?: boolean;
  placeholder?: string;
  searchPlaceholder?: string;
  emptyMessage?: string;
  loadMoreText?: string;
  displayKey: keyof T;
  valueKey: keyof T;
  className?: string;
}

// eslint-disable-next-line @typescript-eslint/no-explicit-any
function FilterSelect<T extends Record<string, any>>({
  items,
  onSearch,
  onLoadMore,
  onSelect,
  selected = null,
  hasMorePages = false,
  isLoading = false,
  placeholder = "Chọn...",
  searchPlaceholder = "Tìm kiếm...",
  emptyMessage = "Không tìm thấy kết quả",
  loadMoreText = "Xem thêm",
  displayKey,
  valueKey,
  className = "",
}: FilterSelectProps<T>) {
  const [open, setOpen] = useState(false);
  const [searchValue, setSearchValue] = useState("");

  // Ensure items is always an array
  const safeItems = Array.isArray(items) ? items : [];

  useEffect(() => {
    // Log warning if items is not an array
    if (!Array.isArray(items)) {
      console.warn("FilterSelect received non-array items:", items);
    }
  }, [items]);

  const handleSearch = (value: string) => {
    setSearchValue(value);
    if (onSearch) onSearch(value);
  };

  const handleSelect = (item: T) => {
    if (onSelect) onSelect(item);
    setOpen(false);
  };

  // Safe accessor functions for properties
  const getDisplayValue = (item: T | null) => {
    if (!item) return "";
    return String(item[displayKey] || "");
  };

  const getKeyValue = (item: T) => {
    return String(item[valueKey] || "");
  };

  const isSelected = (item: T) => {
    if (!selected) return false;
    return selected[valueKey] === item[valueKey];
  };

  return (
    <div className={className}>
      <Popover open={open} onOpenChange={setOpen}>
        <PopoverTrigger asChild>
          <Button
            variant="outline"
            role="combobox"
            aria-expanded={open}
            className="w-full justify-between"
          >
            {selected ? getDisplayValue(selected) : placeholder}
            <ChevronsUpDown className="ml-2 h-4 w-4 shrink-0 opacity-50" />
          </Button>
        </PopoverTrigger>
        <PopoverContent className="w-full p-0" align="start">
          <Command>
            <CommandInput
              placeholder={searchPlaceholder}
              value={searchValue}
              onValueChange={handleSearch}
            />

            {safeItems.length === 0 ? (
              <CommandEmpty>{emptyMessage}</CommandEmpty>
            ) : (
              // In your FilterSelect component:
              <CommandGroup className="max-h-60 overflow-y-auto">
                {Array.isArray(safeItems) &&
                  safeItems.map(
                    (item, index) =>
                      item && (
                        <CommandItem
                          key={`${getKeyValue(item) || index}`}
                          value={getKeyValue(item)}
                          onSelect={() => handleSelect(item)}
                        >
                          <Check
                            className={cn(
                              "mr-2 h-4 w-4",
                              isSelected(item) ? "opacity-100" : "opacity-0"
                            )}
                          />
                          {getDisplayValue(item)}
                        </CommandItem>
                      )
                  )}
              </CommandGroup>
            )}

            {hasMorePages && (
              <div className="p-2 border-t">
                <Button
                  variant="ghost"
                  className="w-full justify-center text-sm"
                  onClick={(e) => {
                    e.preventDefault();
                    if (onLoadMore) onLoadMore();
                  }}
                  disabled={isLoading}
                >
                  {isLoading ? "Đang tải..." : loadMoreText}
                </Button>
              </div>
            )}
          </Command>
        </PopoverContent>
      </Popover>
    </div>
  );
}

export default FilterSelect;
